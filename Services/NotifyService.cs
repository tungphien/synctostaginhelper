using Newtonsoft.Json;
using RestSharp;
using SyncToStaging.Helper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncToStaging.Helper.Services
{
    public class NotifyService
    {
        public async Task<NotifyOutput> NotifyToMobile(NotifyInput input)
        {
            NotifyOutput output = new();
            RestClient restClient = new RestClient("https://fmcg-notification-api.rdos.online/api/v1/notification/expushnotificationoutlet");
            RestRequest request = new RestRequest();
            request.Method = Method.POST;
            request.AddJsonBody(JsonConvert.SerializeObject(input));
            try
            {
                var response = await restClient.ExecuteAsync<NotifyOutput>(request);
                output = response.Data;
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
