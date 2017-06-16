using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureStocksAnalyzerDemo.FunctionApp.Functions
{
    public static class GetAllStocks
    {
        [FunctionName("GetAllStocks")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var service = ServiceLocator.GetService();
            var result = await service.GetAllStockNames();

            return req.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}