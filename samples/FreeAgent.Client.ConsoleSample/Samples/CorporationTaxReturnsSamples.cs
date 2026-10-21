using System.Diagnostics.CodeAnalysis;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Corporation tax returns examples.
/// </summary>
[ConsoleSamples(Category = "Corporation tax returns")]
internal sealed class CorporationTaxReturnsSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List corporation tax returns")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCorporationTaxReturnsAsync(CancellationToken cancellationToken)
    {
        var returns = await context.Client.CorporationTaxReturns.ListAsync(cancellationToken);

        SampleOutput.WriteHeader("Corporation tax returns");
        SampleOutput.WriteField("Count", returns.Count);
        if (returns.Count == 0)
        {
            SampleOutput.WriteField(
                "Note",
                "No corporation tax returns — expected when the sandbox company does not file CT (for example sole trader).");
            return;
        }

        SampleOutput.WriteField("Latest period ends", returns[0].PeriodEndsOn);
        SampleOutput.WriteField("Filing status", returns[0].FilingStatus);
    }

    [ConsoleSample(Name = "Get corporation tax return detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCorporationTaxReturnAsync(CancellationToken cancellationToken)
    {
        var returns = await context.Client.CorporationTaxReturns.ListAsync(cancellationToken);
        if (returns.Count == 0)
        {
            SampleOutput.WriteHeader("Corporation tax return detail");
            SampleOutput.WriteField(
                "Note",
                "Skipped — no corporation tax returns for this company (limited company required).");
            return;
        }

        var periodEndsOn = returns[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed corporation tax return is missing period_ends_on.");

        var detail = await context.Client.CorporationTaxReturns.GetAsync(periodEndsOn, cancellationToken);

        SampleOutput.WriteHeader("Corporation tax return detail");
        SampleOutput.WriteField("Period ends", detail.PeriodEndsOn);
        SampleOutput.WriteField("Amount due", detail.AmountDue);
        SampleOutput.WriteField("Filing status", detail.FilingStatus);
        SampleOutput.WriteField("Payment status", detail.PaymentStatus);
    }

    [ConsoleSample(Name = "Mark corporation tax return as filed (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkCorporationTaxReturnAsFiledAsync(CancellationToken cancellationToken) =>
        await RunMarkTransitionAsync(
            "Mark as filed",
            (period, ct) => context.Client.CorporationTaxReturns.MarkAsFiledAsync(period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark corporation tax return as unfiled (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkCorporationTaxReturnAsUnfiledAsync(CancellationToken cancellationToken) =>
        await RunMarkTransitionAsync(
            "Mark as unfiled",
            (period, ct) => context.Client.CorporationTaxReturns.MarkAsUnfiledAsync(period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark corporation tax return as paid (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkCorporationTaxReturnAsPaidAsync(CancellationToken cancellationToken) =>
        await RunMarkTransitionAsync(
            "Mark as paid",
            (period, ct) => context.Client.CorporationTaxReturns.MarkAsPaidAsync(period, ct),
            static r => r.PaymentStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark corporation tax return as unpaid (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkCorporationTaxReturnAsUnpaidAsync(CancellationToken cancellationToken) =>
        await RunMarkTransitionAsync(
            "Mark as unpaid",
            (period, ct) => context.Client.CorporationTaxReturns.MarkAsUnpaidAsync(period, ct),
            static r => r.PaymentStatus,
            cancellationToken);

    private async Task RunMarkTransitionAsync<TStatus>(
        string header,
        Func<DateOnly, CancellationToken, Task<CorporationTaxReturn>> transition,
        Func<CorporationTaxReturn, TStatus?> readStatus,
        CancellationToken cancellationToken)
    {
        await EnsureLimitedCompanyAsync(cancellationToken);

        var periodEndsOn = await ResolvePeriodEndsOnAsync(cancellationToken);
        var updated = await transition(periodEndsOn, cancellationToken);

        SampleOutput.WriteHeader(header);
        SampleOutput.WriteField("Period ends", updated.PeriodEndsOn);
        SampleOutput.WriteField("Status", readStatus(updated));
    }

    private async Task EnsureLimitedCompanyAsync(CancellationToken cancellationToken)
    {
        var company = await context.Client.Company.GetCompanyAsync(cancellationToken);
        if (company.Type is not CompanyType.UkLimitedCompany and not CompanyType.UkLimitedLiabilityPartnership)
        {
            SampleContext.Skip("connected company is not a UK limited company — corporation tax returns do not apply");
        }
    }

    private async Task<DateOnly> ResolvePeriodEndsOnAsync(CancellationToken cancellationToken)
    {
        var returns = await context.Client.CorporationTaxReturns.ListAsync(cancellationToken);
        if (returns.Count == 0)
        {
            SampleContext.Skip("no corporation tax returns available in this sandbox company");
        }

        return returns[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed corporation tax return is missing period_ends_on.");
    }
}
