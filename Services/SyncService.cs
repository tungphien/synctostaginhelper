using Microsoft.EntityFrameworkCore;
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
        /// <typeparam name="T">OSUsers</typeparam>
        /// <typeparam name="T1">OSOutlets</typeparam>
        /// <typeparam name="T2">OSOutletLinked</typeparam>
        /// <param name="input"></param>
        /// <param name="dbContext">DbContext must be define 3 DbSet OSUsers, OSOutlets, OSOutletLinked</param>
        /// <returns></returns>
        public static async Task<BaseSyncOutput<ODSyncOutput>> Sync<T, T1, T2>(ODSyncInput input, DbContext dbContext)
            where T : class, IOsuser
            where T1 : class, IOsoutlet
            where T2 : class, IOsoutletLinked
        {

            BaseSyncOutput<ODSyncOutput> output = new();
            RestClient restClient = new RestClient(input.Url);

            restClient.Authenticator = new JwtAuthenticator($"{input.Token.Split(" ").Last()}");

            RestRequest request = new();
            request.AddHeader("Content-Type", "application/json");
            StagingBaseInputModel<object> stagingInput = new();
            var requestId = Guid.NewGuid();
            output.RequestId = requestId;
            stagingInput.DataType = input.DataType;
            stagingInput.RequestId = requestId;
            stagingInput.Data = input.Data;
            stagingInput.IsCreateDataChange = input.IsCreateDataChange;
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
                    if (response?.Data?.Success == true && input.IsCreateDataChange && input.IsSendNotification)
                    {
                        // notify khi sync thành công
                        List<string> outletCodes = await GetOutletCodes<T, T1, T2>(input, dbContext);
                        NotifyService notifyService = new NotifyService();
                        NotifyInput notifyInput = new NotifyInput();
                        notifyInput.Title = string.Empty;
                        notifyInput.Body = string.Empty;
                        notifyInput.NotiType = input.IsUrgent == true ? NOTI_TYPE.URGENT : NOTI_TYPE.NORMAL;
                        notifyInput.NavigatePath = input.OwnerCode;
                        notifyInput.OutletCodeList = outletCodes;
                        notifyInput.Purpose = $"SYNC_{input.DataType}";
                        notifyInput.OwnerType = input.OwnerType;
                        notifyInput.OwnerCode = input.OwnerCode;
                        try
                        {
                            var notifyOutput = await notifyService.NotifyToMobile(notifyInput, input.NotificationBaseAPI);
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
                    return output;

                }
                output.Success = false;
                output.Messages.Add(response.StatusCode.ToString());
                output.Messages.Add(response.StatusDescription.ToString());
                output.Messages.Add(response.Content);
                return output;
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Messages.Add(ex.Message);
                return output;
            }

        }

        private static async Task<List<string>> GetOutletCodes<T, T1, T2>(ODSyncInput input, DbContext dbContext)
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
