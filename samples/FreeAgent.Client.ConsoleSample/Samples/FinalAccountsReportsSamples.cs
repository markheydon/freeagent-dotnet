using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Company;
using FreeAgent.Client.Models.FinalAccountsReports;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Final accounts reports examples.
/// </summary>
[ConsoleSamples(Category = "Final accounts reports")]
internal sealed class FinalAccountsReportsSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List final accounts reports")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListFinalAccountsReportsAsync(CancellationToken cancellationToken)
    {
        var reports = await context.Client.FinalAccountsReports.ListAsync(cancellationToken);

        SampleOutput.WriteHeader("Final accounts reports");
        SampleOutput.WriteField("Count", reports.Count);
        if (reports.Count == 0)
        {
            SampleOutput.WriteField(
                "Note",
                "No final accounts reports — expected when the sandbox company does not file final accounts (for example sole trader).");
            return;
        }

        SampleOutput.WriteField("Latest period ends", reports[0].PeriodEndsOn);
        SampleOutput.WriteField("Filing status", reports[0].FilingStatus);
    }

    [ConsoleSample(Name = "Get final accounts report detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetFinalAccountsReportAsync(CancellationToken cancellationToken)
    {
        var reports = await context.Client.FinalAccountsReports.ListAsync(cancellationToken);
        if (reports.Count == 0)
        {
            SampleOutput.WriteHeader("Final accounts report detail");
            SampleOutput.WriteField(
                "Note",
                "Skipped — no final accounts reports for this company (limited company required).");
            return;
        }

        var periodEndsOn = reports[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed final accounts report is missing period_ends_on.");

        var detail = await context.Client.FinalAccountsReports.GetAsync(periodEndsOn, cancellationToken);

        SampleOutput.WriteHeader("Final accounts report detail");
        SampleOutput.WriteField("Period ends", detail.PeriodEndsOn);
        SampleOutput.WriteField("Filing due on", detail.FilingDueOn);
        SampleOutput.WriteField("Filing status", detail.FilingStatus);
    }

    [ConsoleSample(Name = "Mark final accounts report as filed (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkFinalAccountsReportAsFiledAsync(CancellationToken cancellationToken) =>
        await RunMarkTransitionAsync(
            "Mark as filed",
            (period, ct) => context.Client.FinalAccountsReports.MarkAsFiledAsync(period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark final accounts report as unfiled (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkFinalAccountsReportAsUnfiledAsync(CancellationToken cancellationToken) =>
        await RunMarkTransitionAsync(
            "Mark as unfiled",
            (period, ct) => context.Client.FinalAccountsReports.MarkAsUnfiledAsync(period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    private async Task RunMarkTransitionAsync<TStatus>(
        string header,
        Func<DateOnly, CancellationToken, Task<FinalAccountsReport>> transition,
        Func<FinalAccountsReport, TStatus?> readStatus,
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
            SampleContext.Skip("connected company is not a UK limited company — final accounts reports do not apply");
        }
    }

    private async Task<DateOnly> ResolvePeriodEndsOnAsync(CancellationToken cancellationToken)
    {
        var reports = await context.Client.FinalAccountsReports.ListAsync(cancellationToken);
        if (reports.Count == 0)
        {
            SampleContext.Skip("no final accounts reports available in this sandbox company");
        }

        return reports[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed final accounts report is missing period_ends_on.");
    }
}
