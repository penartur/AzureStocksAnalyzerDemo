namespace AzureStocksAnalyzerDemo.DataAzureTable
{
    using Microsoft.WindowsAzure.Storage.Table;

    internal class StockDataEntity : TableEntity
    {
        // Azure Table Storage does not work correctly with decimals
        public string Open { get; set; }

        public string High { get; set; }

        public string Low { get; set; }

        public string Close { get; set; }

        public string Volume { get; set; }
    }
}
