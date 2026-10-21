using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Payroll;

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
        var taxYear = await context.Client.Payroll.ListAllPeriodsForTaxYearAsync(taxYearEnd, cancellationToken);

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
        var taxYear = await context.Client.Payroll.ListAllPeriodsForTaxYearAsync(taxYearEnd, cancellationToken);
        if (taxYear.Periods!.Count == 0)
        {
            SampleContext.Skip("no payroll periods returned for the current tax year end");
        }

        var periodNumber = taxYear.Periods[0].PeriodNumber ?? 0;
        var period = await context.Client.Payroll.ListAllPayslipsForPeriodAsync(
            taxYearEnd,
            periodNumber,
            cancellationToken);

        SampleOutput.WriteHeader($"Payslips for period {periodNumber} ({taxYearEnd})");
        SampleOutput.WriteRows(
            period.Payslips ?? Array.Empty<PayrollPayslip>(),
            payslip => $"user={payslip.UserId}  tax_code={payslip.TaxCode}  basic_pay={payslip.BasicPay}");
    }

    [ConsoleSample(Name = "Mark first payroll payment paid then unpaid", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task MarkFirstPaymentPaidThenUnpaidAsync(CancellationToken cancellationToken)
    {
        var taxYearEnd = ResolveCurrentUkTaxYearEnd();
        var taxYear = await context.Client.Payroll.ListAllPeriodsForTaxYearAsync(taxYearEnd, cancellationToken);
        var payment = taxYear.Payments?.FirstOrDefault(p => p.DueOn is not null);
        if (payment?.DueOn is null)
        {
            SampleContext.Skip("no payroll liability payments returned for the current tax year end");
        }

        var dueOn = payment.DueOn!.Value;
        var paid = await context.Client.Payroll.MarkPaymentAsPaidAsync(taxYearEnd, dueOn, cancellationToken);
        SampleOutput.WriteHeader("Marked payment as paid");
        SampleOutput.WriteField("Status", paid.Payments![0].Status);

        var unpaid = await context.Client.Payroll.MarkPaymentAsUnpaidAsync(taxYearEnd, dueOn, cancellationToken);
        SampleOutput.WriteHeader("Marked payment as unpaid");
        SampleOutput.WriteField("Status", unpaid.Payments![0].Status);
    }

    private static int ResolveCurrentUkTaxYearEnd()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return today.Month >= 4 ? today.Year + 1 : today.Year;
    }
}
