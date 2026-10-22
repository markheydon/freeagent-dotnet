using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Samples.Shared;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Sales tax periods endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Sales tax periods")]
internal sealed class SalesTaxPeriodSamples(SampleContext context) : IConsoleSampleProvider
{
    private const string ProbeName = "SDK-CONSOLE-STP";

    [ConsoleSample(Name = "List sales tax periods")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListSalesTaxPeriodsAsync(CancellationToken cancellationToken)
    {
        await EnsureSalesTaxPeriodsSupportedAsync(cancellationToken);
        var periods = await context.Client.SalesTaxPeriods.ListAsync(cancellationToken);

        SampleOutput.WriteHeader($"Sales tax periods ({periods.Count})");
        SampleOutput.WriteRows(periods, period =>
            $"{period.ResourceId,8}  {period.SalesTaxName}  {period.EffectiveDate:yyyy-MM-dd}  {period.SalesTaxRegistrationStatus}");
    }

    [ConsoleSample(Name = "Get sales tax period detail with probe create and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetSalesTaxPeriodDetailAsync(CancellationToken cancellationToken)
    {
        await EnsureSalesTaxPeriodsSupportedAsync(cancellationToken);
        var (probeId, createdForProbe) = await ResolveProbeSalesTaxPeriodIdAsync(cancellationToken);
        try
        {
            var detail = await context.Client.SalesTaxPeriods.GetSalesTaxPeriodAsync(probeId, cancellationToken);

            SampleOutput.WriteHeader("Sales tax period detail");
            SampleOutput.WriteField("Id", detail.ResourceId);
            SampleOutput.WriteField("Name", detail.SalesTaxName);
            SampleOutput.WriteField("Effective date", detail.EffectiveDate);
            SampleOutput.WriteField("Registration status", detail.SalesTaxRegistrationStatus);
            SampleOutput.WriteField("Rate 1", detail.SalesTaxRate1);
            SampleOutput.WriteField("Locked", detail.IsLocked);
        }
        finally
        {
            if (createdForProbe)
            {
                await context.Client.SalesTaxPeriods.DeleteSalesTaxPeriodAsync(probeId, cancellationToken);
            }
        }
    }

    [ConsoleSample(Name = "Update sales tax period rate", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateSalesTaxPeriodAsync(CancellationToken cancellationToken)
    {
        await EnsureSalesTaxPeriodsSupportedAsync(cancellationToken);
        var (probeId, createdForProbe) = await ResolveProbeSalesTaxPeriodIdAsync(cancellationToken);
        try
        {
            var existing = await context.Client.SalesTaxPeriods.GetSalesTaxPeriodAsync(probeId, cancellationToken);
            var updated = await context.Client.SalesTaxPeriods.UpdateSalesTaxPeriodAsync(
                probeId,
                new UpdateSalesTaxPeriodRequest
                {
                    SalesTaxRate1 = (existing.SalesTaxRate1 ?? 2m) + 0.1m
                },
                cancellationToken);

            SampleOutput.WriteHeader("Updated sales tax period");
            SampleOutput.WriteField("Id", updated.ResourceId);
            SampleOutput.WriteField("Rate 1", updated.SalesTaxRate1);
        }
        finally
        {
            if (createdForProbe)
            {
                await context.Client.SalesTaxPeriods.DeleteSalesTaxPeriodAsync(probeId, cancellationToken);
            }
        }
    }

    private async Task<(long ProbeId, bool CreatedForProbe)> ResolveProbeSalesTaxPeriodIdAsync(CancellationToken cancellationToken)
    {
        var periods = await context.Client.SalesTaxPeriods.ListAsync(cancellationToken);
        var match = periods.FirstOrDefault(period => string.Equals(period.SalesTaxName, ProbeName, StringComparison.Ordinal));
        if (match is not null)
        {
            return (match.ResourceId, false);
        }

        var created = await context.Client.SalesTaxPeriods.CreateSalesTaxPeriodAsync(
            new CreateSalesTaxPeriodRequest
            {
                SalesTaxName = ProbeName,
                SalesTaxRegistrationStatus = SalesTaxPeriodRegistrationStatus.Registered,
                SalesTaxRate1 = 2m,
                SalesTaxIsValueAdded = true,
                EffectiveDate = new DateOnly(2016, 9, 21)
            },
            cancellationToken);

        return (created.ResourceId, true);
    }

    private async Task EnsureSalesTaxPeriodsSupportedAsync(CancellationToken cancellationToken)
    {
        var company = await context.Client.Company.GetCompanyAsync(cancellationToken);
        if (!CompanySalesTaxPeriodSupport.SupportsSalesTaxPeriods(company.Type))
        {
            SampleContext.Skip(
                "sales tax periods API applies to US and universal companies only (connected company is not supported)");
        }
    }
}
