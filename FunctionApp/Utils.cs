namespace AzureStocksAnalyzerDemo.FunctionApp
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    internal static class Utils
    {
        public static HttpResponseMessage CreateResponse<T>(HttpRequestMessage request, HttpStatusCode statusCode, T value)
        {
            return FixResponse(request, request.CreateResponse(statusCode, value));
        }

        public static HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, HttpStatusCode statusCode, string value)
        {
            return FixResponse(request, request.CreateErrorResponse(statusCode, value));
        }

        // To make GitHub demo work, we need to set CORS credentials manually (https://github.com/Azure/azure-webjobs-sdk-script/issues/620)
        private static HttpResponseMessage FixResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (request.Headers.Contains("Origin"))
            {
                response.Headers.Add("Access-Control-Allow-Origin", request.Headers.GetValues("Origin").First());
                response.Headers.Add("Access-Control-Allow-Credentials", "true");
                response.Headers.Add("AzureStocksAnalyzerDemo-Origin-Set", "true");
            }
            else
            {
                response.Headers.Add("AzureStocksAnalyzerDemo-Origin-Set", "false");
            }

            return response;
        }
    }
}
