using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Income tax returns examples.
/// </summary>
[ConsoleSamples(Category = "Income tax returns")]
internal sealed class IncomeTaxReturnsSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List income tax returns")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListIncomeTaxReturnsAsync(CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(cancellationToken);
        var returns = await context.Client.IncomeTaxReturns.ListAsync(userId, cancellationToken);

        SampleOutput.WriteHeader("Income tax returns");
        SampleOutput.WriteField("User id", userId);
        SampleOutput.WriteField("Count", returns.Count);
        if (returns.Count == 0)
        {
            SampleOutput.WriteField(
                "Note",
                "No income tax returns — expected when the connected user or company does not file self assessment.");
            return;
        }

        SampleOutput.WriteField("Latest period ends", returns[0].PeriodEndsOn);
        SampleOutput.WriteField("Filing status", returns[0].FilingStatus);
    }

    [ConsoleSample(Name = "Get income tax return detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetIncomeTaxReturnAsync(CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(cancellationToken);
        var returns = await context.Client.IncomeTaxReturns.ListAsync(userId, cancellationToken);
        if (returns.Count == 0)
        {
            SampleOutput.WriteHeader("Income tax return detail");
            SampleOutput.WriteField(
                "Note",
                "Skipped — no income tax returns for the connected user.");
            return;
        }

        var periodEndsOn = returns[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed income tax return is missing period_ends_on.");

        var detail = await context.Client.IncomeTaxReturns.GetAsync(userId, periodEndsOn, cancellationToken);

        SampleOutput.WriteHeader("Income tax return detail");
        SampleOutput.WriteField("Period ends", detail.PeriodEndsOn);
        SampleOutput.WriteField("Payments", detail.Payments?.Count ?? 0);
        SampleOutput.WriteField("Filing status", detail.FilingStatus);
    }

    [ConsoleSample(Name = "Mark income tax return as filed (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkIncomeTaxReturnAsFiledAsync(CancellationToken cancellationToken) =>
        await RunReturnTransitionAsync(
            "Mark as filed",
            (userId, period, ct) => context.Client.IncomeTaxReturns.MarkAsFiledAsync(userId, period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark income tax return as unfiled (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkIncomeTaxReturnAsUnfiledAsync(CancellationToken cancellationToken) =>
        await RunReturnTransitionAsync(
            "Mark as unfiled",
            (userId, period, ct) => context.Client.IncomeTaxReturns.MarkAsUnfiledAsync(userId, period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark income tax payment as paid (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkIncomeTaxPaymentAsPaidAsync(CancellationToken cancellationToken) =>
        await RunPaymentTransitionAsync(
            "Mark payment as paid",
            (userId, period, paymentDate, ct) =>
                context.Client.IncomeTaxReturns.MarkPaymentAsPaidAsync(userId, period, paymentDate, ct),
            cancellationToken);

    [ConsoleSample(Name = "Mark income tax payment as unpaid (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkIncomeTaxPaymentAsUnpaidAsync(CancellationToken cancellationToken) =>
        await RunPaymentTransitionAsync(
            "Mark payment as unpaid",
            (userId, period, paymentDate, ct) =>
                context.Client.IncomeTaxReturns.MarkPaymentAsUnpaidAsync(userId, period, paymentDate, ct),
            cancellationToken);

    private async Task RunReturnTransitionAsync<TStatus>(
        string header,
        Func<long, DateOnly, CancellationToken, Task<IncomeTaxReturn>> transition,
        Func<IncomeTaxReturn, TStatus?> readStatus,
        CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(cancellationToken);
        var periodEndsOn = await ResolvePeriodEndsOnAsync(userId, cancellationToken);
        var updated = await transition(userId, periodEndsOn, cancellationToken);

        SampleOutput.WriteHeader(header);
        SampleOutput.WriteField("Period ends", updated.PeriodEndsOn);
        SampleOutput.WriteField("Status", readStatus(updated));
    }

    private async Task RunPaymentTransitionAsync(
        string header,
        Func<long, DateOnly, DateOnly, CancellationToken, Task<IncomeTaxReturn>> transition,
        CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(cancellationToken);
        var periodEndsOn = await ResolvePeriodEndsOnAsync(userId, cancellationToken);
        var paymentDate = await ResolvePaymentDueOnAsync(userId, periodEndsOn, cancellationToken);
        var updated = await transition(userId, periodEndsOn, paymentDate, cancellationToken);

        SampleOutput.WriteHeader(header);
        SampleOutput.WriteField("Period ends", updated.PeriodEndsOn);
        SampleOutput.WriteField("Payment due on", paymentDate);
    }

    private async Task<long> ResolveCurrentUserIdAsync(CancellationToken cancellationToken)
    {
        var user = await context.Client.Users.GetCurrentUserAsync(cancellationToken);
        if (!user.TryGetResourceId(out var userId))
        {
            SampleContext.Skip("connected user is missing a parseable resource id");
        }

        return userId;
    }

    private async Task<DateOnly> ResolvePeriodEndsOnAsync(long userId, CancellationToken cancellationToken)
    {
        var returns = await context.Client.IncomeTaxReturns.ListAsync(userId, cancellationToken);
        if (returns.Count == 0)
        {
            SampleContext.Skip("no income tax returns available for the connected user");
        }

        return returns[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed income tax return is missing period_ends_on.");
    }

    private async Task<DateOnly> ResolvePaymentDueOnAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken)
    {
        var detail = await context.Client.IncomeTaxReturns.GetAsync(userId, periodEndsOn, cancellationToken);
        var payment = detail.Payments?
            .FirstOrDefault(p => p.DueOn is not null && p.Status is IncomeTaxReturnPaymentStatus.Unpaid);

        if (payment?.DueOn is null)
        {
            SampleContext.Skip("no unpaid payment with a due date on the selected return");
        }

        return payment.DueOn.Value;
    }
}
