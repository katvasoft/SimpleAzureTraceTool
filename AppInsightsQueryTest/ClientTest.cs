using NUnit.Framework;
using KatvaSoft.SimpleAppInsightQuerier.AppInsightClient;
using System.Threading.Tasks;

namespace AppInsightsQueryTest
{
    public class ClientTests
    {
        public string appId = "<some appid>";

        public string apiKey = "<some api key>";

        public string eventType = "traces";

        [Test]
        public async Task QueryResult()
        {
            var querier = new AppInsightClient(appId, apiKey);



            var result = await querier.QueryAppInsights(eventType, null, null);

            Assert.IsNotNull(result);

            
        }
    }
}