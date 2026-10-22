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
    /// <param name="periodsWithPayslips">Payroll periods that returned payslips (reordered by pay date ascending).</param>
    /// <param name="payments">Liability payments from the tax year list response.</param>
    /// <returns>A payment suitable for <c>mark_as_paid</c>, or <see langword="null"/> when none match.</returns>
    public static PayrollPayment? TrySelectPreviousPeriodLiabilityPayment(
        IReadOnlyList<PayrollPeriod> periodsWithPayslips,
        IReadOnlyList<PayrollPayment>? payments)
    {
        var orderedPeriods = OrderPeriodsByPayDate(periodsWithPayslips);
        if (orderedPeriods.Count < MinimumPeriodsWithPayslips || payments is null || payments.Count == 0)
        {
            return null;
        }

        var previousPeriod = orderedPeriods[^2];
        var latestPeriod = orderedPeriods[^1];
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

    /// <summary>
    /// Explains why <see cref="TrySelectPreviousPeriodLiabilityPayment"/> returned no payment.
    /// </summary>
    public static string DescribeWhyNoPaymentSelected(
        IReadOnlyList<PayrollPeriod> periodsWithPayslips,
        IReadOnlyList<PayrollPayment>? payments)
    {
        var orderedPeriods = OrderPeriodsByPayDate(periodsWithPayslips);
        if (orderedPeriods.Count < MinimumPeriodsWithPayslips)
        {
            return
                $"need at least {MinimumPeriodsWithPayslips} payroll periods with payslips to mark a previous liability payment";
        }

        if (payments is null || payments.Count == 0)
        {
            return "no payroll liability payments returned for the current tax year end";
        }

        var previousPeriod = orderedPeriods[^2];
        var latestPeriod = orderedPeriods[^1];
        if (previousPeriod.DatedOn is null || latestPeriod.DatedOn is null)
        {
            return "payroll periods with payslips are missing pay dates (dated_on)";
        }

        var unpaid = payments
            .Where(static payment => payment.DueOn is not null && payment.Status == PayrollPaymentStatus.Unpaid)
            .ToList();
        if (unpaid.Count == 0)
        {
            return "no unpaid payroll liability payments are available for this tax year";
        }

        return
            "no unpaid liability payment due on or after the previous payslip period pay date and before the latest payslip period pay date (or earlier fallback)";
    }

    private static List<PayrollPeriod> OrderPeriodsByPayDate(IReadOnlyList<PayrollPeriod> periodsWithPayslips) =>
        periodsWithPayslips
            .OrderBy(static period => period.DatedOn ?? DateOnly.MinValue)
            .ThenBy(static period => period.PeriodNumber ?? int.MaxValue)
            .ToList();
}
