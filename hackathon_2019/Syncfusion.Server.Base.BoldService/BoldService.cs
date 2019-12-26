namespace Syncfusion.Server.Base.BoldWebService
{
    using Newtonsoft.Json;
    using System.Net.Http;
    using hackathon_2019;

    public class BoldService
    {
        private BoldApiClient boldApiClient;

        public BoldService(HttpClient httpClient)
        {
            boldApiClient = new BoldApiClient(httpClient);
        }

        public SyncfusionLoginResponse SyncfusionLogin(SyncfusionLoginRequest loginRequestData)
        {
            var resultObject = boldApiClient.RequestExecutor(HttpMethod.Post, BoldApiEndPoints.SyncfusionLogin, loginRequestData);
            var result = resultObject as HttpResponseMessage;

            return JsonConvert.DeserializeObject<SyncfusionLoginResponse>(result.Content.ReadAsStringAsync().Result);
        }
    }
}
