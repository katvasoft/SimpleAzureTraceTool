using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

namespace KatvaSoft.SimpleAppInsightQuerier.AppInsightClient
{
    public class AppInsightClient
    {

        public AppInsightClient(string appId, string apiKey)
        {
            this._appId = appId;
            this._apiKey = apiKey;
        }

        private string _baseUrl = "https://api.applicationinsights.io";

        private string _version = "v1/apps";

        private string _queryType = "events";

        private string _appId;

        private string _apiKey;

        private int _top = 1000;

        private AzureAIMapper _aiMapper = new AzureAIMapper();

        public void SetBaseUrl(string baseUrl)
        {
            this._baseUrl = baseUrl;
        }

        public void SetVersion(string version)
        {
            this._version = version;
        }

        public async Task<List<LogRow>> QueryAppInsights(string eventType, TimeSpan? timeSpan, Int32? topVal)
        {
            var iResult = await CallAppInsights(eventType, timeSpan);
            if(iResult.WasSuccesful)
            {
                var logResults = this._aiMapper.ConvertResultToLogRows(iResult.Result);
                if(topVal != null && topVal.HasValue)
                {
                    this._top = topVal.Value;
                } 
                return logResults;
            } else
            {
                throw new Exception(iResult.Result);
            }
        }

        private async Task<IntermediateResult> CallAppInsights(string eventType, TimeSpan? timeSpan)
        {
            var client = CreateHttpClient();
            var url = CreateUrl(eventType, timeSpan);
            var response = await client.GetAsync(url);
            if(response.IsSuccessStatusCode)
            {
                var res = new IntermediateResult();
                res.Result = response.Content.ReadAsStringAsync().Result;
                res.WasSuccesful = true;
                return res;
            } else
            {
                var res = new IntermediateResult();
                res.Result = response.ReasonPhrase;
                res.WasSuccesful = false;
                return res;
                
            }

        }

        

        private HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", this._apiKey);
            return client;
        }

        private string CreateUrl(string eventType, TimeSpan? timeSpan)
        {
            string url;
            if(timeSpan == null)
            {
                url = $"{_baseUrl}/{_version}/{_appId}/{_queryType}/{eventType}?$top={_top}";
                
            }else
            {
                var timeSpanStr = XmlConvert.ToString(timeSpan.Value);
                url = $"{_baseUrl}/{_version}/{_appId}/{_queryType}/{eventType}?timespan={timeSpanStr}&$top={_top}";
            }
            return url;
        }

    }
}
