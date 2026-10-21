using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Self assessment returns examples.
/// </summary>
[ConsoleSamples(Category = "Self assessment returns")]
internal sealed class SelfAssessmentReturnsSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List self assessment returns")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListSelfAssessmentReturnsAsync(CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(cancellationToken);
        var returns = await context.Client.SelfAssessmentReturns.ListAsync(userId, cancellationToken);

        SampleOutput.WriteHeader("Self assessment returns");
        SampleOutput.WriteField("User id", userId);
        SampleOutput.WriteField("Count", returns.Count);
        if (returns.Count == 0)
        {
            SampleOutput.WriteField(
                "Note",
                "No self assessment returns — expected when the connected user or company does not file self assessment.");
            return;
        }

        SampleOutput.WriteField("Latest period ends", returns[0].PeriodEndsOn);
        SampleOutput.WriteField("Filing status", returns[0].FilingStatus);
    }

    [ConsoleSample(Name = "Get self assessment return detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetSelfAssessmentReturnAsync(CancellationToken cancellationToken)
    {
        var userId = await ResolveCurrentUserIdAsync(cancellationToken);
        var returns = await context.Client.SelfAssessmentReturns.ListAsync(userId, cancellationToken);
        if (returns.Count == 0)
        {
            SampleOutput.WriteHeader("Self assessment return detail");
            SampleOutput.WriteField(
                "Note",
                "Skipped — no self assessment returns for the connected user.");
            return;
        }

        var periodEndsOn = returns[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed self assessment return is missing period_ends_on.");

        var detail = await context.Client.SelfAssessmentReturns.GetAsync(userId, periodEndsOn, cancellationToken);

        SampleOutput.WriteHeader("Self assessment return detail");
        SampleOutput.WriteField("Period ends", detail.PeriodEndsOn);
        SampleOutput.WriteField("Payments", detail.Payments?.Count ?? 0);
        SampleOutput.WriteField("Filing status", detail.FilingStatus);
    }

    [ConsoleSample(Name = "Mark self assessment return as filed (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkSelfAssessmentReturnAsFiledAsync(CancellationToken cancellationToken) =>
        await RunReturnTransitionAsync(
            "Mark as filed",
            (userId, period, ct) => context.Client.SelfAssessmentReturns.MarkAsFiledAsync(userId, period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark self assessment return as unfiled (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkSelfAssessmentReturnAsUnfiledAsync(CancellationToken cancellationToken) =>
        await RunReturnTransitionAsync(
            "Mark as unfiled",
            (userId, period, ct) => context.Client.SelfAssessmentReturns.MarkAsUnfiledAsync(userId, period, ct),
            static r => r.FilingStatus,
            cancellationToken);

    [ConsoleSample(Name = "Mark self assessment payment as paid (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkIncomeTaxPaymentAsPaidAsync(CancellationToken cancellationToken) =>
        await RunPaymentTransitionAsync(
            "Mark payment as paid",
            (userId, period, paymentDate, ct) =>
                context.Client.SelfAssessmentReturns.MarkPaymentAsPaidAsync(userId, period, paymentDate, ct),
            cancellationToken);

    [ConsoleSample(Name = "Mark self assessment payment as unpaid (sandbox)", MutatesData = true, ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkIncomeTaxPaymentAsUnpaidAsync(CancellationToken cancellationToken) =>
        await RunPaymentTransitionAsync(
            "Mark payment as unpaid",
            (userId, period, paymentDate, ct) =>
                context.Client.SelfAssessmentReturns.MarkPaymentAsUnpaidAsync(userId, period, paymentDate, ct),
            cancellationToken);

    private async Task RunReturnTransitionAsync<TStatus>(
        string header,
        Func<long, DateOnly, CancellationToken, Task<SelfAssessmentReturn>> transition,
        Func<SelfAssessmentReturn, TStatus?> readStatus,
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
        Func<long, DateOnly, DateOnly, CancellationToken, Task<SelfAssessmentReturn>> transition,
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
        var returns = await context.Client.SelfAssessmentReturns.ListAsync(userId, cancellationToken);
        if (returns.Count == 0)
        {
            SampleContext.Skip("no self assessment returns available for the connected user");
        }

        return returns[0].PeriodEndsOn
            ?? throw new InvalidOperationException("Listed self assessment return is missing period_ends_on.");
    }

    private async Task<DateOnly> ResolvePaymentDueOnAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken)
    {
        var detail = await context.Client.SelfAssessmentReturns.GetAsync(userId, periodEndsOn, cancellationToken);
        var payment = detail.Payments?
            .FirstOrDefault(p => p.DueOn is not null && p.Status is SelfAssessmentReturnPaymentStatus.Unpaid);

        if (payment?.DueOn is null)
        {
            SampleContext.Skip("no unpaid payment with a due date on the selected return");
        }

        return payment.DueOn.Value;
    }
}
