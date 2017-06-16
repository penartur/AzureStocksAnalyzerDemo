using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureStocksAnalyzerDemo.FunctionApp.Functions
{
    public static class UploadStockData
    {
        [FunctionName("UploadStockData")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadStockData/{stockName}")]
            HttpRequestMessage req,
            string stockName,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Get request body
            var csvContent = await req.Content.ReadAsByteArrayAsync();
            log.Info($"Read {csvContent.Length} characters");

            if (csvContent.Length == 0)
            {
                return req.CreateErrorResponse(HttpStatusCode.BadRequest, "request is empty");
            }

            var service = ServiceLocator.GetService();
            await service.UploadData(stockName, csvContent);

            return req.CreateResponse(HttpStatusCode.OK, $"CSV with length={csvContent.Length} successfully processed");
        }
    }
}