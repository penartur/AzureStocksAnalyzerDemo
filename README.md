# AzureStocksAnalyzerDemo

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