using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AzureStocksAnalyzerDemo.Contracts.Database;
using AzureStocksAnalyzerDemo.Contracts.Service;
using AzureStocksAnalyzerDemo.DataAzureTable;

namespace AzureStocksAnalyzerDemo.FunctionApp
{
    internal class ServiceLocator
    {
        private static ServiceLocatorImplementation Instance { get; } = new ServiceLocatorImplementation();

        public static IService GetService() => Instance.GetService();

        private class ServiceLocatorImplementation
        {
            public ServiceLocatorImplementation()
            {
                this.Repository = new Repository(ConfigurationManager.AppSettings["StorageConnectionString"]);
            }

            private IRepository Repository { get; }

            public IService GetService()
            {
                return new Service.Service(ClaimsPrincipal.Current, this.Repository);
            }
        }
    }
}
