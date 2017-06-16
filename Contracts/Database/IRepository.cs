namespace AzureStocksAnalyzerDemo.Contracts.Database
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRepository
    {
        Task UploadData(IReadOnlyCollection<StockEntry> stockEntries);

        Task<string[]> GetAllStockNames(string userId);

        Task<StockStatistics> GetMin(string userId, string stockName, PriceTypes priceTypes);

        Task<StockStatistics> GetAverage(string userId, string stockName, PriceTypes priceTypes);

        Task<StockStatistics> GetMax(string userId, string stockName, PriceTypes priceTypes);

        Task<StockStatistics> GetMedian(string userId, string stockName, PriceTypes priceTypes);

        Task<StockStatistics> Get95Percentile(string userId, string stockName, PriceTypes priceTypes);
    }
}
