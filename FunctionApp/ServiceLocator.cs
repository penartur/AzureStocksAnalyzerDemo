namespace AzureStocksAnalyzerDemo.FunctionApp
{
    using System.Configuration;
    using System.Security.Claims;
    using AzureStocksAnalyzerDemo.Contracts.Database;
    using AzureStocksAnalyzerDemo.Contracts.Service;
    using AzureStocksAnalyzerDemo.DataAzureTable;

    internal class ServiceLocator
    {
        private static ServiceLocatorImplementation Instance { get; } = new ServiceLocatorImplementation();

        public static IService GetService() => Instance.GetService();

        private class ServiceLocatorImplementation
        {
            public ServiceLocatorImplementation()
            {
                var connectionString = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ToString();
                this.Repository = new Repository(connectionString);
            }

            private IRepository Repository { get; }

            public IService GetService()
            {
                return new Service.Service(ClaimsPrincipal.Current, this.Repository);
            }
        }
    }
}
