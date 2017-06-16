namespace AzureStocksAnalyzerDemo.DataAzureTable
{
    using Microsoft.WindowsAzure.Storage.Table;

    internal class StockDataEntity : TableEntity
    {
        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public long Volume { get; set; }
    }
}
