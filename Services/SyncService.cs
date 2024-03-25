﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SyncToStaging.Helper.Models;
using static SyncToStaging.Helper.Constants.SyncToStagingHelperConsts;

namespace SyncToStaging.Helper.Services
{
    public static class SyncService
    {
        /// <summary>
        /// Hỗ trợ sync data từ OD qua Staging system
        /// </summary>
        /// <typeparam name="T">IOsuser</typeparam>
        /// <typeparam name="T1">IOSOutlets</typeparam>
        /// <typeparam name="T2">IOSOutletLinked</typeparam>
        /// <typeparam name="T3">IOdsyncDataSetting</typeparam>
        /// <typeparam name="T4">IStagingSyncDataHistory</typeparam>
        /// <typeparam name="T5">IServiceUrl</typeparam>
        /// <param name="input"></param>
        /// <param name="dbContext">DbContext must be define 5 DbSet OSUsers, OSOutlets, OSOutletLinked, OdsyncDataSetting, StagingSyncDataHistory, EcoService</param>
        /// <returns></returns>
        public static async Task<BaseSyncOutput<ODSyncOutput>> Sync<T, T1, T2, T3, T4, T5>(ODSyncInput input, DbContext dbContext)
            where T : class, IOsuser
            where T1 : class, IOsoutlet
            where T2 : class, IOsoutletLinked
            where T3 : class, IOdsyncDataSetting
            where T4 : class, IStagingSyncDataHistory
            where T5 : class, IServiceUrl

        {

            BaseSyncOutput<ODSyncOutput> output = new();
            var setting = await GetOdsyncDataSetting<T3>(input.DataType, dbContext);
            // nếu setting không active sẽ không sync
            if (setting == default)
            {
                output.Messages.Add($"OdsyncDataSetting was not found for data type {input.DataType}");
                return output;
            }
            // get service url
            Dictionary<string, T5> serviceUrlDic = await GetServiceUrlAsDic<T5>(new List<string>() { input.StagingBaseAPICode, input.NotificationBaseAPICode }, dbContext);
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
                        List<string> outletCodes = await GetOutletCodes<T, T1, T2>(input, dbContext);
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
                        await LogStagingSyncDataHistory<T4>(input, tempId, string.Join(",", response.Data.Messages), dbContext);
                    return output;

                }
                output.Success = false;
                output.Messages.Add(response.StatusCode.ToString());
                output.Messages.Add(response.ErrorMessage);
                output.Messages.Add(response.Content);

                await LogStagingSyncDataHistory<T4>(input, tempId, response.ErrorMessage, dbContext);
                return output;
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Messages.Add(ex.Message);
                await LogStagingSyncDataHistory<T4>(input, tempId, ex.Message, dbContext);
                return output;
            }
        }
        private static async Task LogStagingSyncDataHistory<T4>(ODSyncInput input, Guid tempId, string message, DbContext dbContext)
           where T4 : class, IStagingSyncDataHistory
        {
            try
            {
                var logData = (T4)Activator.CreateInstance(typeof(T4));
                logData.Id = Guid.NewGuid();
                logData.DataType = input.DataType;
                logData.RequestType = input.RequestType;
                logData.InsertStatus = LOG_HISTORY_STATUS.FAILED;
                logData.TimeRunAdhoc = DateTime.Now;
                logData.StartDate = DateTime.Now;
                logData.EndDate = DateTime.Now;
                logData.CreatedBy = "system";
                logData.CreatedDate = DateTime.Now;
                logData.UpdatedBy = "system";
                logData.UpdatedDate = DateTime.Now;
                logData.ErrorMessage = message;
                logData.TempId = tempId.ToString();

                dbContext.Set<T4>().Add(logData);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Trả về danh sách service url dạng dictionary<code, service_entity>
        /// </summary>
        /// <typeparam name="T5"></typeparam>
        /// <param name="codes"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, T5>> GetServiceUrlAsDic<T5>(List<string> codes, DbContext dbContext)
            where T5 : class, IServiceUrl
        {
            return await dbContext.Set<T5>().Where(d => codes.Contains(d.Code)).ToDictionaryAsync(d => d.Code, m => m);
        }
        /// <summary>
        /// Trả về OdsyncDataSetting theo OdDataType
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="odDataType"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task<T3> GetOdsyncDataSetting<T3>(string odDataType, DbContext dbContext)
            where T3 : class, IOdsyncDataSetting
        {
            return await dbContext.Set<T3>().Where(d => d.OddataType == odDataType && d.Status == "ACTIVE").FirstOrDefaultAsync();
        }

        public static async Task<List<string>> GetOutletCodes<T, T1, T2>(ODSyncInput input, DbContext dbContext)
            where T : class, IOsuser
            where T1 : class, IOsoutlet
            where T2 : class, IOsoutletLinked
        {
            List<string> outletCodes = new();
            try
            {
                switch (input.OwnerType)
                {
                    case "OUTLET":
                        outletCodes = new List<string> { input.OwnerCode };
                        break;
                    case "DISTRIBUTOR":
                        outletCodes = await (from oso in dbContext.Set<T1>()
                                             join osu in dbContext.Set<T>()
                                             on oso.PhoneNumber equals osu.UserName
                                             join osl in dbContext.Set<T2>()
                                             on oso.OutletCode equals osl.OutletCode
                                             where osu.UserName != null &&
                                             oso.Status == "ACTIVE" &&
                                             osl.Status == "ACTIVE" &&
                                             osl.OutletType == "OFFICIAL" &&
                                             osl.DistributorCode == input.OwnerCode
                                             select oso.OutletCode).ToListAsync();
                        break;
                    default:
                        outletCodes = await (from oso in dbContext.Set<T1>()
                                             join osu in dbContext.Set<T>()
                                             on oso.PhoneNumber equals osu.UserName
                                             where osu.UserName != null && oso.Status == "ACTIVE"
                                             select oso.OutletCode).ToListAsync();
                        break;
                }

            }
            catch (Exception ex)
            {

            }
            return outletCodes;
        }
    }
}
