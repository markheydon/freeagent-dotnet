#if NET10_0
using FreeAgent.Client.Samples.Shared;

namespace FreeAgent.Client.Tests.Samples;

public sealed class PayrollLiabilityPaymentProbeSupportTests
{
    [Fact]
    public void TrySelectPreviousPeriodLiabilityPayment_RequiresTwoPeriodsWithPayslips()
    {
        var payments = new[] { new PayrollPayment { DueOn = new DateOnly(2026, 5, 22), Status = PayrollPaymentStatus.Unpaid } };

        Assert.Null(PayrollLiabilityPaymentProbeSupport.TrySelectPreviousPeriodLiabilityPayment(
            [Period(0, new DateOnly(2026, 4, 30))],
            payments));
    }

    [Fact]
    public void TrySelectPreviousPeriodLiabilityPayment_PrefersUnpaidDueBetweenPreviousAndLatestPayDates()
    {
        var periods = new[]
        {
            Period(0, new DateOnly(2026, 4, 30)),
            Period(1, new DateOnly(2026, 5, 31)),
        };

        var payments = new[]
        {
            new PayrollPayment { DueOn = new DateOnly(2026, 5, 10), Status = PayrollPaymentStatus.Unpaid },
            new PayrollPayment { DueOn = new DateOnly(2026, 5, 22), Status = PayrollPaymentStatus.Unpaid },
            new PayrollPayment { DueOn = new DateOnly(2026, 6, 22), Status = PayrollPaymentStatus.Unpaid },
        };

        var selected = PayrollLiabilityPaymentProbeSupport.TrySelectPreviousPeriodLiabilityPayment(periods, payments);

        Assert.NotNull(selected);
        Assert.Equal(new DateOnly(2026, 5, 22), selected.DueOn);
    }

    [Fact]
    public void TrySelectPreviousPeriodLiabilityPayment_FallsBackToLatestUnpaidBeforeLatestPayDate()
    {
        var periods = new[]
        {
            Period(1, new DateOnly(2026, 4, 30)),
            Period(0, new DateOnly(2026, 5, 31)),
        };

        var payments = new[]
        {
            new PayrollPayment { DueOn = new DateOnly(2026, 4, 15), Status = PayrollPaymentStatus.Unpaid },
        };

        var selected = PayrollLiabilityPaymentProbeSupport.TrySelectPreviousPeriodLiabilityPayment(periods, payments);

        Assert.NotNull(selected);
        Assert.Equal(new DateOnly(2026, 4, 15), selected.DueOn);
    }

    [Fact]
    public void TrySelectPreviousPeriodLiabilityPayment_ReturnsNullWhenPayDatesMissing()
    {
        var periods = new[]
        {
            new PayrollPeriod { PeriodNumber = 0, Payslips = [new PayrollPayslip()] },
            new PayrollPeriod { PeriodNumber = 1, DatedOn = new DateOnly(2026, 5, 31), Payslips = [new PayrollPayslip()] },
        };

        var payments = new[]
        {
            new PayrollPayment { DueOn = new DateOnly(2026, 5, 22), Status = PayrollPaymentStatus.Unpaid },
        };

        Assert.Null(PayrollLiabilityPaymentProbeSupport.TrySelectPreviousPeriodLiabilityPayment(periods, payments));
    }

    [Fact]
    public void DescribeWhyNoPaymentSelected_MentionsMissingPayDates()
    {
        var periods = new[]
        {
            new PayrollPeriod { PeriodNumber = 0, Payslips = [new PayrollPayslip()] },
            new PayrollPeriod { PeriodNumber = 1, DatedOn = new DateOnly(2026, 5, 31), Payslips = [new PayrollPayslip()] },
        };

        var payments = new[]
        {
            new PayrollPayment { DueOn = new DateOnly(2026, 5, 22), Status = PayrollPaymentStatus.Unpaid },
        };

        var reason = PayrollLiabilityPaymentProbeSupport.DescribeWhyNoPaymentSelected(periods, payments);

        Assert.Contains("pay dates", reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TrySelectPreviousPeriodLiabilityPayment_SkipsNonUnpaidPayments()
    {
        var periods = new[]
        {
            Period(0, new DateOnly(2026, 4, 30)),
            Period(1, new DateOnly(2026, 5, 31)),
        };

        var payments = new[]
        {
            new PayrollPayment { DueOn = new DateOnly(2026, 5, 22), Status = PayrollPaymentStatus.MarkedAsPaid },
        };

        Assert.Null(PayrollLiabilityPaymentProbeSupport.TrySelectPreviousPeriodLiabilityPayment(periods, payments));
    }

    private static PayrollPeriod Period(int number, DateOnly payDate) =>
        new()
        {
            PeriodNumber = number,
            DatedOn = payDate,
            Payslips = [new PayrollPayslip()],
        };
}
#endif
