namespace AzureStocksAnalyzerDemo.Contracts.Database
{
    using System;

    public class StockEntry
    {
        public string UserId { get; set; }

        public string StockName { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public long Volume { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
