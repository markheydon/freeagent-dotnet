#if NET10_0
using FreeAgent.Client.Samples.Shared.Turpinverse;

namespace FreeAgent.Client.Tests.Samples.Turpinverse;

public class TurpinverseSalesDateSupportTests
{
    private static readonly DateOnly MinimumDate = new(2026, 1, 1);

    [Fact]
    public void ClampInvoiceDates_RaisesHistoricalDatesToMinimum()
    {
        var (datedOn, dueOn) = TurpinverseSalesDateSupport.ClampInvoiceDates(
            new DateOnly(1737, 11, 5),
            new DateOnly(1737, 12, 5),
            MinimumDate);

        Assert.Equal(MinimumDate, datedOn);
        Assert.Equal(MinimumDate, dueOn);
    }

    [Fact]
    public void ClampInvoiceDates_PreservesFutureDates()
    {
        var issueDate = new DateOnly(2026, 6, 15);
        var dueDate = new DateOnly(2026, 7, 15);

        var (datedOn, dueOn) = TurpinverseSalesDateSupport.ClampInvoiceDates(issueDate, dueDate, MinimumDate);

        Assert.Equal(issueDate, datedOn);
        Assert.Equal(dueDate, dueOn);
    }

    [Fact]
    public void ClampInvoiceDates_EnsuresDueOnIsNotBeforeDatedOn()
    {
        var (datedOn, dueOn) = TurpinverseSalesDateSupport.ClampInvoiceDates(
            new DateOnly(2026, 8, 1),
            new DateOnly(1737, 12, 5),
            MinimumDate);

        Assert.Equal(new DateOnly(2026, 8, 1), datedOn);
        Assert.Equal(new DateOnly(2026, 8, 1), dueOn);
    }

    [Fact]
    public void ParseCanonDate_ParsesIsoDates()
    {
        var parsed = TurpinverseSalesDateSupport.ParseCanonDate("2026-09-01", "issueDate");

        Assert.Equal(new DateOnly(2026, 9, 1), parsed);
    }
}

#endif
