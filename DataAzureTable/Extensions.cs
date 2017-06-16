namespace AzureStocksAnalyzerDemo.DataAzureTable
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Table;

    internal static class Extensions
    {
        public static decimal Quantile(this IEnumerable<decimal> source, double lowerFraction)
        {
            var ordered = source.OrderBy(value => value).ToList();
            var index = (int)(lowerFraction * ordered.Count);
            if (index >= ordered.Count)
            {
                return decimal.MaxValue;
            }

            return ordered[(int)(lowerFraction * ordered.Count)];
        }


        public static async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(this CloudTable table, TableQuery<TEntity> query)
             where TEntity : ITableEntity, new()
        {
            List<TEntity> result = new List<TEntity>();

            TableContinuationToken continuationToken = null;
            while (true)
            {
                var dbResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                if (!dbResult.Results.Any())
                {
                    return result;
                }

                continuationToken = dbResult.ContinuationToken;
                foreach (var entity in dbResult.Results)
                {
                    result.Add(entity);
                }
            }
        }
    }
}
