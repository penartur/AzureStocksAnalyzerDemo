namespace AzureStocksAnalyzerDemo.Service
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using AzureStocksAnalyzerDemo.Contracts;
    using AzureStocksAnalyzerDemo.Contracts.Database;
    using AzureStocksAnalyzerDemo.Contracts.Service;
    using CsvHelper;
    using CsvHelper.Configuration;

    public class Service : IService
    {
        public Service(IPrincipal principal, IRepository repository)
        {
            this.Principal = principal;
            this.Repository = repository;

            if (string.IsNullOrEmpty(this.UserId))
            {
                throw new ArgumentException("UserId should not be empty", nameof(principal));
            }
        }

        private IPrincipal Principal { get; }

        private IRepository Repository { get; }

        private string UserId => this.Principal.Identity.Name;

        public Task<StockStatistics> Get95Percentile(string stockName, PriceTypes priceTypes)
        {
            return this.Repository.Get95Percentile(this.UserId, stockName, priceTypes);
        }

        public Task<string[]> GetAllStockNames()
        {
            return this.Repository.GetAllStockNames(this.UserId);
        }

        public Task<StockStatistics> GetMax(string stockName, PriceTypes priceTypes)
        {
            return this.Repository.GetMax(this.UserId, stockName, priceTypes);
        }

        public Task<StockStatistics> GetAverage(string stockName, PriceTypes priceTypes)
        {
            return this.Repository.GetAverage(this.UserId, stockName, priceTypes);
        }

        public Task<StockStatistics> GetMedian(string stockName, PriceTypes priceTypes)
        {
            return this.Repository.GetMedian(this.UserId, stockName, priceTypes);
        }

        public Task<StockStatistics> GetMin(string stockName, PriceTypes priceTypes)
        {
            return this.Repository.GetMin(this.UserId, stockName, priceTypes);
        }

        public Task UploadData(string stockName, byte[] csvContent)
        {
            return this.Repository.UploadData(ParseUploadedData(stockName, csvContent).ToList());
        }

        private IEnumerable<StockEntry> ParseUploadedData(string stockName, byte[] csvContent)
        {
            if (!stockName.All(char.IsLetterOrDigit))
            {
                throw new ArgumentException("Only letters and digits are allowed in stock name", nameof(stockName));
            }

            using (var stream = new MemoryStream(csvContent))
            {
                using (var textReader = new StreamReader(stream))
                {
                    var csvReader = new CsvReader(textReader, new CsvConfiguration { HasHeaderRecord = true });
                    while (csvReader.Read())
                    {
                        yield return new StockEntry
                        {
                            UserId = this.UserId,
                            StockName = stockName,
                            Timestamp = DateTime.ParseExact(csvReader["Date"], "d-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
                            Open = decimal.Parse(csvReader["Open"], CultureInfo.InvariantCulture),
                            High = decimal.Parse(csvReader["High"], CultureInfo.InvariantCulture),
                            Low = decimal.Parse(csvReader["Low"], CultureInfo.InvariantCulture),
                            Close = decimal.Parse(csvReader["Close"], CultureInfo.InvariantCulture),
                            Volume = long.Parse(csvReader["Volume"], CultureInfo.InvariantCulture),
                        };
                    }
                }
            }
        }
    }
}
