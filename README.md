# AzureStocksAnalyzerDemo

## Assignment

Develop an API (service) hosted on Azure (consider using InfluxDB). The API should:

- enable uploading stock market data in the CSV format (as exported by google finance, Date, Open, High, Low, Close, Volume) for individual stock symbols, e.g. GOOG. Data can be uploaded continuously, i.e. more data can be uploaded for the same stock when it becomes available.
- provide query capabilities
  - to list available stocks with the most recent update timestamp and
  - to query individual stock symbol for the following stats: min, avg, max, median, 95th percentile for each price type (OHLC) with an ability to specify a subset of price types that must be included in the result (e.g. if only open and close must be returned), the statistics should also be timestamped (i.e. statistics should be attributed with the most recent data point available).

API should have authentication mechanism to allow for multiple clients to upload their data and being able to query only their data.

We expect the code to be published in the GitHub repository with the short description of how to use the API - inputs, outputs, error handling, authentication.
And provide endpoint of the Azure deployment with several pre-created accounts that we can test against.

## Build and deploy

You will need Visual Studio 15.3 or newer, with [Azure Function Tools](https://marketplace.visualstudio.com/items?itemName=AndrewBHall-MSFT.AzureFunctionToolsforVisualStudio2017)

For deployment to Azure:

* Create Function App in Azure Portal;
* Configure it to use App Service Authentication with Azure Active Directory configured; disallow anonymous requests (Platform Features, Authentication / Authorization);
* (Optionally, to use API from browser) Enable CORS (Platform Features, CORS), and delete all allowed origins (see https://github.com/Azure/azure-webjobs-sdk-script/issues/620 for details);
* Add ConnectionString `StorageConnectionString` pointing to the Azure Storage Account with read/write access to `StocksData` and `UserStocks` tables (Settings, Manage Application Settings);
* Deploy from VS (right click on FunctionApp project, "Deploy").

## API methods

* https://example.org/api/UploadStockData/GOOG - POST content of CSV file (as exported by Google Finance).
The request body should only contain file contents (with content-length equal to the file size).
* https://example.org/api/GetAllStocks - GET list of stock names.
* https://example.org/api/GetStockStatistics/GOOG/Max - GET information on maximum OHLC prices / volumes for GOOG stock for its entire logged history.
Besides `Max`, you may also supply `Min`, `Average`, `Median` or `Percentile95`.
Additionally, you may pass `?priceTypes=Open,Close`, so that only `Open` and `Close` maximum values will be returned.
Supported values: `Open`, `High`, `Low`, `Close`, `Volume`, `All`.

## Example

https://penartur.github.io/AzureStocksAnalyzerDemo

Note that there seem to be some cross-origin related problems in Chrome, so the demo might not work there.
Additionally, for some reason Azure returns 403 error for POST requests with well-known browser user-agents (e.g. Firefox, Edge).
The demo is tested and is fully functional in Firefox with user-agent set to any other string (go to `about:config`, create new string preference with name `general.useragent.override` and value `test`).

To use: enter the base app url (e.g. `https://yourfunctionapp.azurewebsites.net`), log in, play!

## Implementation notes

This is my first experience with Azure Functions or Azure Active Directory.

I've chosen to use Azure Active Directory for authentication; in order to use the API, clients will need to supply authorization token (e.g. via "AppServiceAuthSession" cookie); user ID is obtained from this token.

I've chosen to use Azure Table Storage in this prototype instead of InfluxDB. It's totally unoptimized for this kind of task (and is painfully slow for uploading, with ~5s for every year worth of data).
However, I haven't had enough time to get InfluxDB up and running in Azure.
The database backend in code could be easily changed from Azure to InfluxDB.

Most of the material code is in the Azure Table Storage interoperation library; it contains the implementation of operations which are, as I understand, implemented natively in InfluxDB.
Other than that, my solution is really simple, and there is not much to be unit tested, so I opted to go without unit tests in this case.
In the real life, some complex business logic could appear with time; the way I developed this project, it will all go to the Service layer, and will be easily testable.
