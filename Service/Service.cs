namespace AzureStocksAnalyzerDemo.Service
{
    using System;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using AzureStocksAnalyzerDemo.Contracts;
    using AzureStocksAnalyzerDemo.Contracts.Service;

    public class Service : IService
    {
        public Service(IPrincipal principal)
        {
            this.Principal = principal;
        }

        private IPrincipal Principal { get; }

        private string UserId => this.Principal.Identity.Name;

        public Task<StockStatistics> Get95th(string stockName, PriceType priceType, PriceTypes priceTypes)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetAllStockNames()
        {
            throw new NotImplementedException();
        }

        public Task<StockStatistics> GetMax(string stockName, PriceType priceType, PriceTypes priceTypes)
        {
            throw new NotImplementedException($"{this.Principal.Identity.Name} requested max for {stockName}: priceType is {priceType}, priceTypes are {priceTypes}");
        }

        public Task<StockStatistics> GetMedian(string stockName, PriceType priceType, PriceTypes priceTypes)
        {
            throw new NotImplementedException();
        }

        public Task<StockStatistics> GetMin(string stockName, PriceType priceType, PriceTypes priceTypes)
        {
            throw new NotImplementedException();
        }

        public Task UploadData(string stockName, string csvContent)
        {
            throw new NotImplementedException();
        }
    }
}
