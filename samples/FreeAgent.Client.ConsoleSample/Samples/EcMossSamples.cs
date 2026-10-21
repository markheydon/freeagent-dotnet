using System.Diagnostics.CodeAnalysis;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// EC VAT MOSS sales tax rates lookup examples.
/// </summary>
[ConsoleSamples(Category = "EC VAT MOSS")]
internal sealed class EcMossSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List MOSS sales tax rates for Austria")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListMossSalesTaxRatesAsync(CancellationToken cancellationToken)
    {
        var rates = await context.Client.EcMoss.ListSalesTaxRatesAsync(
            EcMossPlaceOfSupply.Austria,
            DateOnly.FromDateTime(DateTime.UtcNow),
            cancellationToken);

        SampleOutput.WriteHeader($"EC VAT MOSS rates for Austria ({rates.Count})");
        SampleOutput.WriteRows(rates, rate => $"{rate.Band,-12}  {rate.Percentage}%");
    }
}
