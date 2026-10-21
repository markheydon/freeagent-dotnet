using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// VAT returns examples.
/// </summary>
[ConsoleSamples(Category = "VAT returns")]
internal sealed class VatReturnsSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List VAT returns")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListVatReturnsAsync(CancellationToken cancellationToken)
    {
        var returns = await context.Client.VatReturns.ListAsync(cancellationToken);

        SampleOutput.WriteHeader("VAT returns");
        SampleOutput.WriteField("Count", returns.Count);
        if (returns.Count > 0)
        {
            SampleOutput.WriteField("Latest period ends", returns[0].PeriodEndsOn);
            SampleOutput.WriteField("Filing status", returns[0].FilingStatus);
        }
    }

    [ConsoleSample(Name = "Get VAT return detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetVatReturnAsync(CancellationToken cancellationToken)
    {
        var returns = await context.Client.VatReturns.ListAsync(cancellationToken);
        if (returns.Count == 0)
        {
            SampleOutput.WriteHeader("VAT return detail");
            SampleOutput.WriteField("Note", "No VAT returns available in this sandbox company.");
            return;
        }

        var periodEndsOn = returns[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed VAT return is missing period_ends_on.");

        var detail = await context.Client.VatReturns.GetVatReturnAsync(periodEndsOn, cancellationToken);

        SampleOutput.WriteHeader("VAT return detail");
        SampleOutput.WriteField("Period ends", detail.PeriodEndsOn);
        SampleOutput.WriteField("Breakdown title", detail.Breakdown?.Title);
        SampleOutput.WriteField("Breakdown rows", detail.Breakdown?.Rows?.Count);
    }
}
