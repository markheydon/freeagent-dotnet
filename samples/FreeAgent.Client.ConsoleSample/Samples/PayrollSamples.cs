using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Samples.Shared;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Payroll endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Payroll")]
internal sealed class PayrollSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List payroll periods for current tax year end")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListPayrollTaxYearAsync(CancellationToken cancellationToken)
    {
        var taxYearEnd = ResolveCurrentUkTaxYearEnd();
        var taxYear = await context.Client.Payroll.ListAsync(taxYearEnd, cancellationToken);

        SampleOutput.WriteHeader($"Payroll tax year {taxYearEnd}");
        SampleOutput.WriteField("Periods", taxYear.Periods!.Count);
        SampleOutput.WriteField("Payments", taxYear.Payments?.Count ?? 0);
        SampleOutput.WriteRows(
            taxYear.Periods!,
            period => $"period={period.PeriodNumber}  dated={period.DatedOn:yyyy-MM-dd}  status={period.Status}");
    }

    [ConsoleSample(Name = "List payslips for first payroll period")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListFirstPeriodPayslipsAsync(CancellationToken cancellationToken)
    {
        var taxYearEnd = ResolveCurrentUkTaxYearEnd();
        var taxYear = await context.Client.Payroll.ListAsync(taxYearEnd, cancellationToken);
        if (taxYear.Periods!.Count == 0)
        {
            SampleContext.Skip("no payroll periods returned for the current tax year end");
        }

        var periodNumber = taxYear.Periods[0].PeriodNumber ?? 0;
        var period = await context.Client.Payroll.ListPayslipsAsync(
            taxYearEnd,
            periodNumber,
            cancellationToken);

        SampleOutput.WriteHeader($"Payslips for period {periodNumber} ({taxYearEnd})");
        SampleOutput.WriteRows(
            period.Payslips ?? Array.Empty<PayrollPayslip>(),
            payslip => $"user={payslip.UserId}  tax_code={payslip.TaxCode}  basic_pay={payslip.BasicPay}");
    }

    [ConsoleSample(Name = "Mark previous liability payment paid then unpaid", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkPreviousLiabilityPaymentPaidThenUnpaidAsync(CancellationToken cancellationToken)
    {
        var taxYearEnd = ResolveCurrentUkTaxYearEnd();
        var taxYear = await context.Client.Payroll.ListAsync(taxYearEnd, cancellationToken);
        if (taxYear.Periods!.Count == 0)
        {
            SampleContext.Skip("no payroll periods returned for the current tax year end");
        }

        var periodsWithPayslips = await ListPeriodsWithPayslipsAsync(taxYearEnd, taxYear.Periods!, cancellationToken);
        if (periodsWithPayslips.Count < PayrollLiabilityPaymentProbeSupport.MinimumPeriodsWithPayslips)
        {
            SampleContext.Skip(
                $"need at least {PayrollLiabilityPaymentProbeSupport.MinimumPeriodsWithPayslips} payroll periods with payslips to mark a previous liability payment");
        }

        var payment = PayrollLiabilityPaymentProbeSupport.TrySelectPreviousPeriodLiabilityPayment(
            periodsWithPayslips,
            taxYear.Payments);
        if (payment?.DueOn is null)
        {
            SampleContext.Skip(
                PayrollLiabilityPaymentProbeSupport.DescribeWhyNoPaymentSelected(
                    periodsWithPayslips,
                    taxYear.Payments));
        }

        var dueOn = payment.DueOn!.Value;
        SampleOutput.WriteHeader("Selected liability payment");
        SampleOutput.WriteField("Due on", dueOn);
        SampleOutput.WriteField("Amount due", payment.AmountDue);
        SampleOutput.WriteField("Status", payment.Status);

        var paid = await context.Client.Payroll.MarkPaymentAsPaidAsync(taxYearEnd, dueOn, cancellationToken);
        var paidRow = paid.Payments?.FirstOrDefault(p => p.DueOn == dueOn);
        if (paidRow?.Status is not PayrollPaymentStatus.MarkedAsPaid and not PayrollPaymentStatus.Paid)
        {
            throw new InvalidOperationException(
                $"MarkPaymentAsPaidAsync did not return marked_as_paid for due date {dueOn:yyyy-MM-dd} (status: {paidRow?.Status}).");
        }

        SampleOutput.WriteHeader("Marked payment as paid");
        SampleOutput.WriteField("Status", paidRow.Status);

        var unpaid = await context.Client.Payroll.MarkPaymentAsUnpaidAsync(taxYearEnd, dueOn, cancellationToken);
        var unpaidRow = unpaid.Payments?.FirstOrDefault(p => p.DueOn == dueOn);
        if (unpaidRow?.Status is not PayrollPaymentStatus.Unpaid)
        {
            throw new InvalidOperationException(
                $"MarkPaymentAsUnpaidAsync did not return unpaid for due date {dueOn:yyyy-MM-dd} (status: {unpaidRow?.Status}).");
        }

        SampleOutput.WriteHeader("Marked payment as unpaid");
        SampleOutput.WriteField("Status", unpaidRow.Status);
    }

    private async Task<List<PayrollPeriod>> ListPeriodsWithPayslipsAsync(
        int taxYearEnd,
        IReadOnlyList<PayrollPeriod> periodSummaries,
        CancellationToken cancellationToken)
    {
        var withPayslips = new List<PayrollPeriod>();
        foreach (var summary in periodSummaries
                     .Where(static period => period.PeriodNumber is not null)
                     .OrderBy(static period => period.DatedOn ?? DateOnly.MinValue)
                     .ThenBy(static period => period.PeriodNumber))
        {
            var periodNumber = summary.PeriodNumber!.Value;
            var detail = await context.Client.Payroll.ListPayslipsAsync(taxYearEnd, periodNumber, cancellationToken);
            if (detail.Payslips is { Count: > 0 })
            {
                withPayslips.Add(detail);
            }
        }

        return withPayslips;
    }

    private static int ResolveCurrentUkTaxYearEnd()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return today.Month >= 4 ? today.Year + 1 : today.Year;
    }
}
