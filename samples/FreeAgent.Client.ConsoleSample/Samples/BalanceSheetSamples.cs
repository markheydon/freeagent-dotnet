using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Balance sheet report examples.
/// </summary>
[ConsoleSamples(Category = "Balance sheet")]
internal sealed class BalanceSheetSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "Get balance sheet")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetBalanceSheetAsync(CancellationToken cancellationToken)
    {
        var sheet = await context.Client.BalanceSheet.GetBalanceSheetAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Balance sheet");
        SampleOutput.WriteField("As at", sheet.AsAtDate);
        SampleOutput.WriteField("Currency", sheet.Currency);
        SampleOutput.WriteField("Total assets", sheet.TotalAssets);
        SampleOutput.WriteField("Total owners equity", sheet.TotalOwnersEquity);
    }

    [ConsoleSample(Name = "Get balance sheet opening balances")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetBalanceSheetOpeningBalancesAsync(CancellationToken cancellationToken)
    {
        var sheet = await context.Client.BalanceSheet.GetOpeningBalancesAsync(cancellationToken);

        SampleOutput.WriteHeader("Balance sheet opening balances");
        SampleOutput.WriteField("Currency", sheet.Currency);
        SampleOutput.WriteField("Net current assets", sheet.NetCurrentAssets);
        SampleOutput.WriteField("Total assets", sheet.TotalAssets);
    }
}
