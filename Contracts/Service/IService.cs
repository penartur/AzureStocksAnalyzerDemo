namespace AzureStocksAnalyzerDemo.Contracts.Service
{
    using System.Threading.Tasks;

    public interface IService
    {
        // CSV files exported by google finance are relatively small (containing at most 365 rows), so it makes sense to work with strings instead of streams
        Task UploadData(string stockName, string csvContent);

        Task<string[]> GetAllStockNames();

        Task<StockStatistics> GetMin(string stockName, PriceType priceType, PriceTypes priceTypes);

        Task<StockStatistics> GetMax(string stockName, PriceType priceType, PriceTypes priceTypes);

        Task<StockStatistics> GetMedian(string stockName, PriceType priceType, PriceTypes priceTypes);

        Task<StockStatistics> Get95th(string stockName, PriceType priceType, PriceTypes priceTypes);
    }
}
