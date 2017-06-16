using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureStocksAnalyzerDemo.Contracts;
using AzureStocksAnalyzerDemo.Contracts.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AzureStocksAnalyzerDemo.FunctionApp.Functions
{
    public static class GetStockStatistics
    {
        [FunctionName("GetStockStatistics")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetStockStatistics/{stockName}/{priceTypeString}/{requestTypeString}")]
            HttpRequestMessage req,
            string stockName,
            string priceTypeString,
            string requestTypeString,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var priceTypesString = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Equals(q.Key, "priceTypes", StringComparison.OrdinalIgnoreCase)).Value;

            Enum.TryParse(requestTypeString, out RequestType requestType);
            Enum.TryParse(priceTypeString, out PriceType priceType);
            var priceTypes = PriceTypes.All;
            if (!string.IsNullOrEmpty(priceTypesString))
            {
                Enum.TryParse(priceTypesString, out priceTypes);
            }

            var service = ServiceLocator.GetService();
            var statistics = await GetStatistics(service, stockName, requestType, priceType, priceTypes);

            if (statistics == null)
            {
                return req.CreateErrorResponse(HttpStatusCode.NotFound, "Stock not found");
            }

            return req.CreateResponse(HttpStatusCode.OK, statistics);
        }

        private static async Task<StockStatistics> GetStatistics(IService service, string stockName, RequestType requestType, PriceType priceType, PriceTypes priceTypes)
        {
            switch (requestType)
            {
                case RequestType.Max:
                    return await service.GetMax(stockName, priceType, priceTypes);
                case RequestType.Min:
                    return await service.GetMin(stockName, priceType, priceTypes);
                case RequestType.Median:
                    return await service.GetMedian(stockName, priceType, priceTypes);
                case RequestType.Percentile95:
                    return await service.Get95th(stockName, priceType, priceTypes);
                default:
                    throw new ArgumentOutOfRangeException(nameof(requestType), requestType, "Value should be one of Max, Min, Median, Percentile95");
            }
        }

        public enum RequestType
        {
            Min,
            Max,
            Median,
            Percentile95,
        }
    }
}