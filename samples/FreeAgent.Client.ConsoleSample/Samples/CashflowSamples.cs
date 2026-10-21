using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Cashflow report examples.
/// </summary>
[ConsoleSamples(Category = "Cashflow")]
internal sealed class CashflowSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "Get cashflow summary")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCashflowAsync(CancellationToken cancellationToken)
    {
        var to = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var from = to.AddMonths(-3);

        var cashflow = await context.Client.Cashflow.GetCashflowAsync(from, to, cancellationToken);

        SampleOutput.WriteHeader("Cashflow");
        SampleOutput.WriteField("From", cashflow.From);
        SampleOutput.WriteField("To", cashflow.To);
        SampleOutput.WriteField("Balance", cashflow.Balance);
        SampleOutput.WriteField("Incoming total", cashflow.Incoming?.Total);
        SampleOutput.WriteField("Outgoing total", cashflow.Outgoing?.Total);
    }
}
