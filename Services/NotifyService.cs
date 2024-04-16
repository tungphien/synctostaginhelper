using Newtonsoft.Json;
using RestSharp;
using SyncToStaging.Helper.Models;

namespace SyncToStaging.Helper.Services
{
    public class NotifyService
    {
        public async Task<NotifyOutput> NotifyToMobile(NotifyInput input, string notificationBaseAPI = "https://fmcg-notification-api.rdos.online/api/v1/")
        {
            string url = $"{notificationBaseAPI}notification/expushnotificationoutlet";
            NotifyOutput output = new();
            RestClient restClient = new RestClient(url);
            RestRequest request = new RestRequest();
            request.Method = Method.POST;
            string paramBody = JsonConvert.SerializeObject(input);
            output.NotifyMobileParamLog = paramBody;
            output.NotifyMobileUriLog = url;
            request.AddJsonBody(paramBody);
            try
            {
                var response = await restClient.ExecuteAsync<NotifyOutput>(request);
                if (response?.Data != null)
                {
                    output = response.Data;
                }
                else
                {
                    output.Messages.Add(response.StatusCode.ToString());
                    output.Messages.Add(response.StatusDescription);
                }
                output.NotifyMobileParamLog = paramBody;
                output.NotifyMobileUriLog = url;
                return output;
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Messages.Add(ex.Message);
                return output;
            }
        }
    }
}
