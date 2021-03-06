﻿namespace AzureStocksAnalyzerDemo.DataAzureTable
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureStocksAnalyzerDemo.Contracts;
    using AzureStocksAnalyzerDemo.Contracts.Database;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class Repository : IRepository
    {
        private const string StocksDataTableName = "StocksData";

        private const string UserStocksTableName = "UserStocks";

        public Repository(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private string ConnectionString { get; }

        private CloudTableClient CloudTableClient => CloudStorageAccount.Parse(this.ConnectionString).CreateCloudTableClient();

        private CloudTable StocksDataTable => this.CloudTableClient.GetTableReference(StocksDataTableName);

        private CloudTable UserStocksTable => this.CloudTableClient.GetTableReference(UserStocksTableName);

        public Task<StockStatistics> Get95Percentile(string userId, string stockName, PriceTypes priceTypes)
        {
            return GetStatistics(userId, stockName, priceTypes, data => data.Quantile(0.95));
        }

        public async Task<string[]> GetAllStockNames(string userId)
        {
            var query = new TableQuery<UserStockEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId));
            var result = await this.UserStocksTable.ExecuteQueryAsync(query);
            return result.Select(entity => entity.RowKey).ToArray();
        }

        public Task<StockStatistics> GetAverage(string userId, string stockName, PriceTypes priceTypes)
        {
            return GetStatistics(userId, stockName, priceTypes, data => data.Average());
        }

        public Task<StockStatistics> GetMax(string userId, string stockName, PriceTypes priceTypes)
        {
            return GetStatistics(userId, stockName, priceTypes, data => data.Max());
        }

        public Task<StockStatistics> GetMedian(string userId, string stockName, PriceTypes priceTypes)
        {
            return GetStatistics(userId, stockName, priceTypes, data => data.Quantile(0.5));
        }

        public Task<StockStatistics> GetMin(string userId, string stockName, PriceTypes priceTypes)
        {
            return GetStatistics(userId, stockName, priceTypes, data => data.Min());
        }

        public async Task UploadData(IReadOnlyCollection<StockEntry> stockEntries)
        {
            var stocksDataOperations = stockEntries
                .Select(entry => new StockDataEntity
                {
                    PartitionKey = GetPartitionKey(entry.UserId, entry.StockName),
                    RowKey = GetRowKey(entry.Timestamp),
                    Timestamp = entry.Timestamp,
                    Open = entry.Open.ToString(CultureInfo.InvariantCulture),
                    High = entry.High.ToString(CultureInfo.InvariantCulture),
                    Low = entry.Low.ToString(CultureInfo.InvariantCulture),
                    Close = entry.Close.ToString(CultureInfo.InvariantCulture),
                    Volume = entry.Volume.ToString(CultureInfo.InvariantCulture),
                })
                .Select(entity => TableOperation.InsertOrReplace(entity));

            var stocksDataTable = this.StocksDataTable;
            foreach (var operation in stocksDataOperations)
            {
                await stocksDataTable.ExecuteAsync(operation);
            }

            var userStocksOperations = stockEntries
                .GroupBy(entry => entry.UserId)
                .Select(group => new
                {
                    userId = group.Key,
                    stockNames = group.GroupBy(entry => entry.StockName).Select(groupStocks => groupStocks.Key),
                })
                .SelectMany(tuple => tuple.stockNames.Select(stockName => new { tuple.userId, stockName }))
                .Select(tuple => new UserStockEntity
                {
                    PartitionKey = tuple.userId,
                    RowKey = tuple.stockName,
                })
                .Select(entity => TableOperation.InsertOrReplace(entity));
            var userStocksTable = this.UserStocksTable;
            foreach (var operation in userStocksOperations)
            {
                await userStocksTable.ExecuteAsync(operation);
            }
        }

        private static string GetPartitionKey(string userId, string stockName)
        {
            return userId + ":" + stockName;
        }

        private static string GetRowKey(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        private static DateTime GetDateTime(string rowKey)
        {
            return DateTime.ParseExact(rowKey, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        private async Task<StockStatistics> GetStatistics(string userId, string stockName, PriceTypes priceTypes, Func<IEnumerable<decimal>, decimal> aggregator)
        {
            var stocksData = await GetAllStockData(userId, stockName);
            if (!stocksData.Any())
            {
                return null;
            }

            Func<PriceTypes, Func<StockDataEntity, string>, decimal?> priceTypeProcessor = (priceType, selector) =>
            {
                if (!priceTypes.HasFlag(priceType))
                {
                    return null;
                }

                return aggregator(stocksData.Select(entity => decimal.Parse(selector(entity), CultureInfo.InvariantCulture)));
            };

            return new StockStatistics
            {
                DataPointsCount = stocksData.Count,
                Timestamp = stocksData.Max(entity => GetDateTime(entity.RowKey)),
                Open = priceTypeProcessor(PriceTypes.Open, entity => entity.Open),
                High = priceTypeProcessor(PriceTypes.High, entity => entity.High),
                Low = priceTypeProcessor(PriceTypes.Low, entity => entity.Low),
                Close = priceTypeProcessor(PriceTypes.Close, entity => entity.Close),
                Volume = priceTypeProcessor(PriceTypes.Volume, entity => entity.Volume),
            };
        }

        private async Task<List<StockDataEntity>> GetAllStockData(string userId, string stockName)
        {
            var query = new TableQuery<StockDataEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GetPartitionKey(userId, stockName)));
            var result = await this.StocksDataTable.ExecuteQueryAsync(query);
            return result.ToList();
        }
    }
}
