$(() => {
    "use strict";

    const knownElements = {
        getApiBaseUrl: () => $("#apiBaseUrl"),
        getLoginButton: () => $("#loginButton"),
        getCsvFile: () => $("#csvFile"),
        getCsvStockName: () => $("#csvStockName"),
        getUploadButton: () => $("#uploadButton"),
        getStockSelect: () => $("#stockSelect"),
        getStockSelectUpdate: () => $("#stockSelectUpdate"),
        getStatisticsTypeSelect: () => $("#statisticsTypeSelect"),
        getColumnsFilter: () => $("[name='columnsFilter']"),
        getStatisticsButton: () => $("#statisticsButton"),
        getResultPlaceholder: () => $("#resultPlaceholder")
    };

    const getApiUrl = () => knownElements.getApiBaseUrl().val();

    const getFile = () => knownElements.getCsvFile().prop("files")[0];

    const login = () => document.location = `${getApiUrl()}/.auth/login/aad?post_login_redirect_url=${encodeURIComponent(document.location)}`;

    const onAjaxError = (jqXhr, textStatus, errorThrown) => alert(`An error occured: ${textStatus}, ${errorThrown}`);

    const onFileSelected = () => {
        const filename = getFile().name;
        const stockName = filename.substr(0, filename.indexOf(".")).toUpperCase();
        knownElements.getCsvStockName().val(stockName);
    };

    const updateStocksList = () => $.ajax(`${getApiUrl()}/api/GetAllStocks`, {
        error: onAjaxError,
        method: "GET",
        success: (data) => {
            knownElements.getStockSelect().empty();
            data.forEach((stockName) => $("<option/>").val(stockName).text(stockName).appendTo(knownElements.getStockSelect()));
        },
        xhrFields: { withCredentials: true }
    });

    const loadStatistics = () => {
        knownElements.getResultPlaceholder().text("");
        $.ajax(`${getApiUrl()}/api/GetStockStatistics/${knownElements.getStockSelect().val()}/${knownElements.getStatisticsTypeSelect().val()}`, {
            data: {
                priceTypes: knownElements.getColumnsFilter().filter(":checked").get().map((el) => $(el).val()).join(",")
            },
            error: onAjaxError,
            method: "GET",
            success: (data) => {
                const text = Object.keys(data).filter((key) => data[key]).map((key) => `${key}: ${data[key]}`).join("\r\n");
                knownElements.getResultPlaceholder().text(text);
            },
            xhrFields: { withCredentials: true }
        });
    };

    const uploadFile = () => {
        const reader = new FileReader();
        reader.onload = () => {
            const buffer = reader.result;
            alert(`Read ${buffer.byteLength} bytes; uploading`);
            $.ajax(`${getApiUrl()}/api/UploadStockData/${knownElements.getCsvStockName().val()}`, {
                contentType: false,
                data: buffer,
                error: onAjaxError,
                method: "POST",
                processData: false,
                success: () => {
                    alert("File successfully uploaded");
                    updateStocksList();
                },
                xhrFields: { withCredentials: true }
            });
        };
        reader.readAsArrayBuffer(getFile());
    };

    const rawHash = document.location.hash || "";
    const hash = rawHash.startsWith("#") ? rawHash.substr(1) : rawHash;
    knownElements.getApiBaseUrl().val(hash);

    if (getApiUrl()) updateStocksList();

    knownElements.getLoginButton().click(login);
    knownElements.getCsvFile().change(onFileSelected);
    knownElements.getUploadButton().click(uploadFile);
    knownElements.getStockSelectUpdate().click(updateStocksList);
    knownElements.getStatisticsButton().click(loadStatistics);
});