namespace AzureStocksAnalyzerDemo.FunctionApp.Functions
{
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

    public static class GetStockStatistics
    {
        [FunctionName("GetStockStatistics")]

        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetStockStatistics/{stockName}/{requestTypeString}")]
            HttpRequestMessage req,
            string stockName,
            string requestTypeString,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var priceTypesString = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Equals(q.Key, "priceTypes", StringComparison.OrdinalIgnoreCase)).Value;

            Enum.TryParse(requestTypeString, out RequestType requestType);
            var priceTypes = PriceTypes.All;
            if (!string.IsNullOrEmpty(priceTypesString))
            {
                Enum.TryParse(priceTypesString, out priceTypes);
            }

            var service = ServiceLocator.GetService();
            var statistics = await GetStatistics(service, stockName, requestType, priceTypes);

            if (statistics == null)
            {
                return Utils.CreateErrorResponse(req, HttpStatusCode.NotFound, "Stock not found");
            }

            return Utils.CreateResponse(req, HttpStatusCode.OK, statistics);
        }

        private static Task<StockStatistics> GetStatistics(IService service, string stockName, RequestType requestType, PriceTypes priceTypes)
        {
            switch (requestType)
            {
                case RequestType.Min:
                    return service.GetMin(stockName, priceTypes);
                case RequestType.Average:
                    return service.GetAverage(stockName, priceTypes);
                case RequestType.Max:
                    return service.GetMax(stockName, priceTypes);
                case RequestType.Median:
                    return service.GetMedian(stockName, priceTypes);
                case RequestType.Percentile95:
                    return service.Get95Percentile(stockName, priceTypes);
                default:
                    throw new ArgumentOutOfRangeException(nameof(requestType), requestType, "Value should be one of Min, Average, Max, Median, Percentile95");
            }
        }

        public enum RequestType
        {
            Min,
            Average,
            Max,
            Median,
            Percentile95,
        }
    }
}