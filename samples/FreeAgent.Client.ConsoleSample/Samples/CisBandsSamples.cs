using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.CisBands;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// CIS bands endpoint examples.
/// </summary>
[ConsoleSamples(Category = "CIS bands")]
internal sealed class CisBandsSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List CIS bands")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCisBandsAsync(CancellationToken cancellationToken)
    {
        var company = await context.Client.Company.GetCompanyAsync(cancellationToken);
        if (company.CisSubcontractor != true)
        {
            SampleContext.Skip("company is not enrolled in CIS for subcontractors");
        }

        var bands = await context.Client.CisBands.ListAsync(cancellationToken);

        SampleOutput.WriteHeader($"CIS bands ({bands.Count})");
        SampleOutput.WriteRows(
            bands,
            band => $"{band.Name,-14}  rate={band.DeductionRate}  nominal={band.NominalCode}  {band.IncomeDescription}");
    }
}
