using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AzureStocksAnalyzerDemo.Contracts.Service;

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
            }

            public IService GetService()
            {
                return new Service.Service(ClaimsPrincipal.Current);
            }
        }
    }
}
