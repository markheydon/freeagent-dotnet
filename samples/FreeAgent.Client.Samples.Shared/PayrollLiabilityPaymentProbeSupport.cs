namespace FreeAgent.Client.Samples.Shared;

/// <summary>
/// Selects HMRC liability payments for console smoke probes from payroll period and payment data.
/// </summary>
public static class PayrollLiabilityPaymentProbeSupport
{
    /// <summary>
    /// Minimum payroll periods with payslips required before toggling mark-as-paid on a previous liability.
    /// </summary>
    public const int MinimumPeriodsWithPayslips = 2;

    /// <summary>
    /// Picks an unpaid liability payment tied to the payroll period before the latest period that has payslips.
    /// </summary>
    /// <param name="periodsWithPayslips">Payroll periods that returned payslips, ordered by pay date ascending.</param>
    /// <param name="payments">Liability payments from the tax year list response.</param>
    /// <returns>A payment suitable for <c>mark_as_paid</c>, or <see langword="null"/> when none match.</returns>
    public static PayrollPayment? TrySelectPreviousPeriodLiabilityPayment(
        IReadOnlyList<PayrollPeriod> periodsWithPayslips,
        IReadOnlyList<PayrollPayment>? payments)
    {
        if (periodsWithPayslips.Count < MinimumPeriodsWithPayslips || payments is null || payments.Count == 0)
        {
            return null;
        }

        var previousPeriod = periodsWithPayslips[^2];
        var latestPeriod = periodsWithPayslips[^1];
        if (previousPeriod.DatedOn is not DateOnly previousPayDate
            || latestPeriod.DatedOn is not DateOnly latestPayDate)
        {
            return null;
        }

        var unpaid = payments
            .Where(static payment => payment.DueOn is not null && payment.Status == PayrollPaymentStatus.Unpaid)
            .ToList();

        if (unpaid.Count == 0)
        {
            return null;
        }

        var betweenPeriods = unpaid
            .Where(payment => payment.DueOn >= previousPayDate && payment.DueOn < latestPayDate)
            .OrderBy(payment => payment.DueOn)
            .ToList();

        if (betweenPeriods.Count > 0)
        {
            return betweenPeriods[^1];
        }

        return unpaid
            .Where(payment => payment.DueOn < latestPayDate)
            .OrderBy(payment => payment.DueOn)
            .LastOrDefault();
    }
}
