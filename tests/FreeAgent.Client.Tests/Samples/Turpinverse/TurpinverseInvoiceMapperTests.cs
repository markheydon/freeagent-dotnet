#if NET10_0
using FreeAgent.Client.Samples.Shared.Turpinverse;

namespace FreeAgent.Client.Tests.Samples.Turpinverse;

public class TurpinverseInvoiceMapperTests
{
    private static readonly DateOnly MinimumDate = new(2026, 1, 1);

    [Theory]
    [InlineData("Draft", false)]
    [InlineData("Authorised", true)]
    [InlineData("Paid", true)]
    [InlineData("Overdue", true)]
    [InlineData("Void", false)]
    public void ShouldMarkAsSent_MapsCanonStatuses(string canonStatus, bool expected)
    {
        Assert.Equal(expected, TurpinverseInvoiceMapper.ShouldMarkAsSent(canonStatus));
    }

    [Theory]
    [InlineData("Void", true)]
    [InlineData("Draft", false)]
    [InlineData("Paid", false)]
    public void ShouldMarkAsCancelled_MapsVoidOnly(string canonStatus, bool expected)
    {
        Assert.Equal(expected, TurpinverseInvoiceMapper.ShouldMarkAsCancelled(canonStatus));
    }

    [Fact]
    public void ToFreeAgentInvoice_ClampsHistoricalDatesAndPreservesPaymentTerms()
    {
        var invoice = TurpinverseInvoiceMapper.ToFreeAgentInvoice(
            new TurpinverseInvoice
            {
                InvoiceId = "inv-003",
                InvoiceNumber = "INV-1737-0042",
                IssueDate = "1737-11-05",
                DueDate = "1737-12-05",
                Currency = "GBP",
                Lines = []
            },
            contactId: 1,
            projectId: null,
            minimumDocumentDate: MinimumDate);

        Assert.Equal(MinimumDate, invoice.DatedOn);
        Assert.Equal(MinimumDate, invoice.DueOn);
        Assert.Equal(0, invoice.PaymentTermsInDays);
    }
}

#endif
