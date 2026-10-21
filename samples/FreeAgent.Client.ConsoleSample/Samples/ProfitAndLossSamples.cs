using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Profit and loss report examples.
/// </summary>
[ConsoleSamples(Category = "Profit and loss")]
internal sealed class ProfitAndLossSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "Get profit and loss summary")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetProfitAndLossSummaryAsync(CancellationToken cancellationToken)
    {
        var summary = await context.Client.ProfitAndLoss.GetSummaryAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Profit and loss summary");
        SampleOutput.WriteField("From", summary.From);
        SampleOutput.WriteField("To", summary.To);
        SampleOutput.WriteField("Income", summary.Income);
        SampleOutput.WriteField("Expenses", summary.Expenses);
        SampleOutput.WriteField("Operating profit", summary.OperatingProfit);
        SampleOutput.WriteField("Retained profit", summary.RetainedProfit);
    }
}
