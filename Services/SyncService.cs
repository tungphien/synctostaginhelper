﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SyncToStaging.Helper.Models;
using System.Reflection;
using static SyncToStaging.Helper.Constants.SyncToStagingHelperConsts;
using SyncToStaging.Helper.Constants;

namespace SyncToStaging.Helper.Services
{
    public static class SyncService
    {
        private static readonly List<string> REQUIRED_TABLES = new List<string>()
        {
            SyncToStagingHelperConsts.ENTITY_TABLE.Osusers,
            SyncToStagingHelperConsts.ENTITY_TABLE.Osoutlets,
            SyncToStagingHelperConsts.ENTITY_TABLE.OsoutletLinkeds,
            SyncToStagingHelperConsts.ENTITY_TABLE.OdsyncDataSettings,
            SyncToStagingHelperConsts.ENTITY_TABLE.Services,
            SyncToStagingHelperConsts.ENTITY_TABLE.StagingSyncDataHistories
        };

        /// <summary>
        /// Hỗ trợ sync data từ OD qua Staging system.
        /// DbContext must be define 6 DbSet: Osusers, Osoutlets, OsoutletLinkeds, OdsyncDataSettings, Services, StagingSyncDataHistories
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task<BaseSyncOutput<ODSyncOutput>> OneShopStagingSync(ODSyncInput input, DbContext dbContext)
        {

            BaseSyncOutput<ODSyncOutput> output = new();

            // validate dbContext
            var dbsets = GetRequiredDbSetProperties(dbContext);
            if (dbsets?.Count != REQUIRED_TABLES.Count)
            {
                throw new Exception($"DbContext required 6 DbSet: {string.Join(", ", REQUIRED_TABLES)}");
            }
           
            var setting = await GetOdsyncDataSetting(input.DataType, dbContext);
            // nếu setting không active sẽ không sync
            if (setting == default)
            {
                output.Messages.Add($"OdsyncDataSetting was not found for data type {input.DataType}");
                return output;
            }

            // get service url
            Dictionary<string, ServiceUrlModel> serviceUrlDic = await GetServiceUrlAsDic(new List<string>() { input.StagingBaseAPICode, input.NotificationBaseAPICode }, dbContext);
            if (serviceUrlDic?.Count < 2)
            {
                output.Messages.Add($"StagingBaseAPI or NotificationBaseAPI was not found");
                return output;
            }

            RestClient restClient = new RestClient($"{serviceUrlDic[input.StagingBaseAPICode].URL}{input.StagingRequestPath}");

            restClient.Authenticator = new JwtAuthenticator($"{input.Token.Split(" ").Last()}");

            RestRequest request = new();
            request.AddHeader("Content-Type", "application/json");
            StagingBaseInputModel<object> stagingInput = new();
            Guid tempId = Guid.NewGuid();
            output.TempId = tempId;
            stagingInput.DataType = setting.OsdataType;
            stagingInput.IsCreateDataChange = setting.IsCreateDataChange;
            stagingInput.isUrgent = setting.IsNotiUrgent;
            stagingInput.TempId = tempId;
            stagingInput.Data = input.Data;


            request.AddJsonBody(JsonConvert.SerializeObject(stagingInput));

            switch (input.RequestType)
            {
                case "INSERT":
                    request.Method = Method.POST;
                    break;
                case "UPDATE":
                    request.Method = Method.PUT;
                    break;
                case "DELETE":
                    request.Method = Method.DELETE;
                    break;
                case "BULKINSERT":
                    request.Method = Method.POST;
                    stagingInput.DeletedIds = input.DeletedIds;
                    break;
            }

            var logPropertyType = dbsets.Where(d => d.Name == SyncToStagingHelperConsts.ENTITY_TABLE.StagingSyncDataHistories).FirstOrDefault();

            try
            {
                var response = await restClient.ExecuteAsync<BaseSyncOutput<ODSyncOutput>>(request);
                if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response?.Data != default)
                    {
                        output = response.Data;
                    }
                    if (response?.Data?.Success == true && stagingInput.IsCreateDataChange && input.IsSendNotification)
                    {
                        // notify khi sync thành công
                        List<string> outletCodes = await GetOutletCodes(dbContext, input.OwnerType, input.OwnerCode);
                        NotifyService notifyService = new NotifyService();
                        NotifyInput notifyInput = new NotifyInput();
                        notifyInput.Title = string.Empty;
                        notifyInput.Body = string.Empty;
                        notifyInput.NotiType = stagingInput.isUrgent == true ? NOTI_TYPE.URGENT : NOTI_TYPE.NORMAL;
                        notifyInput.NavigatePath = input.OwnerCode;
                        notifyInput.OutletCodeList = outletCodes;
                        notifyInput.Purpose = $"SYNC_{stagingInput.DataType}";
                        notifyInput.OwnerType = input.OwnerType;
                        notifyInput.OwnerCode = input.OwnerCode;
                        try
                        {
                            var notifyOutput = await notifyService.NotifyToMobile(notifyInput, serviceUrlDic[input.NotificationBaseAPICode].URL);
                            if (notifyOutput != default)
                            {
                                output.NotifyMobileUriLog = notifyOutput.NotifyMobileUriLog;
                                output.NotifyMobileParamLog = notifyOutput.NotifyMobileParamLog;
                            }
                        }
                        catch (Exception _ex)
                        {
                            throw _ex;
                        }
                    }
                    if (response?.Data?.Success == false && response?.Data?.Messages?.Count > 0)
                        await LogStagingSyncDataHistory(input, tempId, string.Join(",", response.Data.Messages), dbContext, logPropertyType);
                    return output;

                }
                output.Success = false;
                output.Messages.Add(response.StatusCode.ToString());
                output.Messages.Add(response.ErrorMessage);
                output.Messages.Add(response.Content);

                await LogStagingSyncDataHistory(input, tempId, response.ErrorMessage, dbContext, logPropertyType);
                return output;
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Messages.Add(ex.Message);
                await LogStagingSyncDataHistory(input, tempId, ex.Message, dbContext, logPropertyType);
                return output;
            }
        }
        public static IQueryable<TEntity> QueryDataByTableName<TEntity>(DbContext dbContext, string tableName) where TEntity : class
        {
            // Use reflection to get DbSet property by name
            var dbSetProperty = dbContext.GetType().GetProperty(tableName);
            if (dbSetProperty != null && dbSetProperty.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                var dbSet = dbSetProperty.GetValue(dbContext);
                if (dbSet != null)
                {
                    // Return IQueryable data from DbSet
                    return ((IQueryable<TEntity>)dbSet);
                }
            }

            // DbSet with the given name not found or not compatible
            throw new ArgumentException($"DbSet '{tableName}' not found or not compatible.", nameof(tableName));
        }
        private static bool TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, value, null);
                return true;
            }
            return false;
        }
        public static List<PropertyInfo> GetRequiredDbSetProperties(this DbContext context)
        {
            return context.GetType().GetProperties()
                .Where(property => property.PropertyType.IsGenericType &&
                typeof(DbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()) &&
                REQUIRED_TABLES.Contains(property.Name)
                ).Select(property => property).ToList();
        }
        private static async Task LogStagingSyncDataHistory(ODSyncInput input, Guid tempId, string message, DbContext dbContext, PropertyInfo? propertyInfo)
        {
            try
            {
                if (propertyInfo?.PropertyType?.IsGenericType == true && propertyInfo?.PropertyType?.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    Type entityType = propertyInfo.PropertyType.GetGenericArguments()[0];
                    dynamic logData = Activator.CreateInstance(entityType); // Create new instance using reflection
                    DateTime now = DateTime.Now;
                    TrySetProperty(logData, "Id", Guid.NewGuid());
                    TrySetProperty(logData, "DataType", input.DataType);
                    TrySetProperty(logData, "RequestType", input.RequestType);
                    TrySetProperty(logData, "InsertStatus", LOG_HISTORY_STATUS.FAILED);
                    TrySetProperty(logData, "TimeRunAdhoc", now);
                    TrySetProperty(logData, "StartDate", now);
                    TrySetProperty(logData, "EndDate", now);
                    TrySetProperty(logData, "CreatedBy", "system");
                    TrySetProperty(logData, "CreatedDate", now);
                    TrySetProperty(logData, "UpdatedBy", "system");
                    TrySetProperty(logData, "UpdatedDate", now);
                    TrySetProperty(logData, "ErrorMessage", message);
                    TrySetProperty(logData, "TempId", tempId.ToString());

                    dynamic dbSet = propertyInfo.GetValue(dbContext); // Get DbSet
                    dbSet.Add(logData); // Add new entity to DbSet
                    dbContext.SaveChanges(); // Save changes to the database
                }
                else
                {
                    throw new ArgumentException($"Property {propertyInfo.Name} is not a DbSet.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Trả về danh sách service url dạng dictionary<code, service_entity>
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="codes"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, ServiceUrlModel>> GetServiceUrlAsDic(List<string> codes, DbContext dbContext)
        {
            return await QueryDataByTableName<object>(dbContext, SyncToStagingHelperConsts.ENTITY_TABLE.Services).Where(d => codes.Contains(EF.Property<string>(d, "Code"))).Select(x => new ServiceUrlModel
            {
                Code = EF.Property<string>(x, "Code"),
                URL = EF.Property<string>(x, "URL"),
            }).ToDictionaryAsync(d => d.Code, m => m);
        }

        /// <summary>
        /// Trả về OdsyncDataSetting theo OdDataType
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="odDataType"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task<OdsyncDataSettingModel> GetOdsyncDataSetting(string odDataType, DbContext dbContext)
        {
            return await QueryDataByTableName<object>(dbContext, SyncToStagingHelperConsts.ENTITY_TABLE.OdsyncDataSettings).Where(d => EF.Property<string>(d, "OddataType") == odDataType && EF.Property<string>(d, "Status") == STATUS.ACTIVE).Select(x => new OdsyncDataSettingModel
            {
                OddataType = EF.Property<string>(x, "OddataType"),
                OsdataType = EF.Property<string>(x, "OsdataType"),
                IsCreateDataChange = EF.Property<bool>(x, "IsCreateDataChange"),
                IsNotiUrgent = EF.Property<bool>(x, "IsNotiUrgent")
            }).FirstOrDefaultAsync();
        }

        public static async Task<List<string>> GetOutletCodes(DbContext dbContext, string ownerType, string ownerCode)
        {
            List<string> outletCodes = new();
            try
            {
                switch (ownerType)
                {
                    case "OUTLET":
                        outletCodes = new List<string> { ownerCode };
                        break;
                    case "DISTRIBUTOR":
                        outletCodes = await (from oso in QueryDataByTableName<object>(dbContext, SyncToStagingHelperConsts.ENTITY_TABLE.OdsyncDataSettings)
                                             join osu in QueryDataByTableName<object>(dbContext, SyncToStagingHelperConsts.ENTITY_TABLE.Osusers)
                                             on EF.Property<string>(oso, "PhoneNumber") equals EF.Property<string>(osu, "UserName")
                                             join osl in QueryDataByTableName<object>(dbContext, SyncToStagingHelperConsts.ENTITY_TABLE.OsoutletLinkeds)
                                             on EF.Property<string>(oso, "OutletCode") equals EF.Property<string>(osl, "OutletCode")
                                             where EF.Property<string>(osu, "UserName") != null &&
                                             EF.Property<string>(oso, "Status") == STATUS.ACTIVE &&
                                             EF.Property<string>(osl, "Status") == STATUS.ACTIVE &&
                                             EF.Property<string>(osl, "OutletType") == "OFFICIAL" &&
                                             EF.Property<string>(osl, "DistributorCode") == ownerCode
                                             select EF.Property<string>(oso, "OutletCode")).ToListAsync();
                        break;
                    default:
                        outletCodes = await (from oso in QueryDataByTableName<object>(dbContext, SyncToStagingHelperConsts.ENTITY_TABLE.Osoutlets)
                                             join osu in QueryDataByTableName<object>(dbContext, SyncToStagingHelperConsts.ENTITY_TABLE.Osusers)
                                             on EF.Property<string>(oso, "PhoneNumber") equals EF.Property<string>(osu, "UserName")
                                             where EF.Property<string>(osu, "UserName") != null && EF.Property<string>(oso, "Status") == STATUS.ACTIVE
                                             select EF.Property<string>(oso, "OutletCode")).ToListAsync();
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return outletCodes;
        }
    }
}
