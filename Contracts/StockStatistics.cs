using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStocksAnalyzerDemo.Contracts
{
    public class StockStatistics
    {
        public string StockName { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volume { get; set; }

        public DateTime Timestamp { get; set; }

        public int DataPointsCount { get; set; }
    }
}
