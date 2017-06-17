namespace AzureStocksAnalyzerDemo.Contracts
{
    using System;

    [Flags]
    public enum PriceTypes
    {
        Open = 1 << PriceType.Open,
        High = 1 << PriceType.High,
        Low = 1 << PriceType.Low,
        Close = 1 << PriceType.Close,
        Volume = 1 << PriceType.Volume,
        All = Open | High | Low | Close | Volume,
    }
}
