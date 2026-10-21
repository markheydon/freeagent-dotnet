using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Trial balance report examples.
/// </summary>
[ConsoleSamples(Category = "Trial balance")]
internal sealed class TrialBalanceSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "Get trial balance summary")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetTrialBalanceSummaryAsync(CancellationToken cancellationToken)
    {
        var lines = await context.Client.TrialBalance.ListSummaryAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Trial balance summary");
        SampleOutput.WriteField("Line count", lines.Count);
        if (lines.Count > 0)
        {
            SampleOutput.WriteField("First line name", lines[0].Name);
            SampleOutput.WriteField("First line total", lines[0].Total);
        }
    }

    [ConsoleSample(Name = "Get trial balance opening balances")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetTrialBalanceOpeningBalancesAsync(CancellationToken cancellationToken)
    {
        var lines = await context.Client.TrialBalance.ListOpeningBalancesAsync(cancellationToken);

        SampleOutput.WriteHeader("Trial balance opening balances");
        SampleOutput.WriteField("Line count", lines.Count);
    }
}
