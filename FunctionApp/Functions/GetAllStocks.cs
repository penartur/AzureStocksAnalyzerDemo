namespace AzureStocksAnalyzerDemo.FunctionApp.Functions
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;

    public static class GetAllStocks
    {
        [FunctionName("GetAllStocks")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var service = ServiceLocator.GetService();
            var result = await service.GetAllStockNames();

            return Utils.CreateResponse(req, HttpStatusCode.OK, result);
        }
    }
}