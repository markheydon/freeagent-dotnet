using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class ShowProjectNameWriteSupportTests
{
    [Fact]
    public void ValidateInvoiceUpdate_ShowProjectNameWithoutStatus_Throws()
    {
        var invoice = new Invoice
        {
            ShowProjectName = true,
            Status = null
        };

        var exception = Assert.Throws<ArgumentException>(() => ShowProjectNameWriteSupport.ValidateInvoiceUpdate(invoice));

        Assert.Equal("invoice", exception.ParamName);
        Assert.Contains("Status is Draft", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateInvoiceUpdate_ShowProjectNameWithDraftStatus_DoesNotThrow()
    {
        var invoice = new Invoice
        {
            ShowProjectName = true,
            Status = InvoiceStatus.Draft
        };

        var exception = Record.Exception(() => ShowProjectNameWriteSupport.ValidateInvoiceUpdate(invoice));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateInvoiceUpdate_ShowProjectNameWithOpenStatus_DoesNotThrow()
    {
        var invoice = new Invoice
        {
            ShowProjectName = true,
            Status = InvoiceStatus.Open
        };

        var exception = Record.Exception(() => ShowProjectNameWriteSupport.ValidateInvoiceUpdate(invoice));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateInvoiceUpdate_UnsetShowProjectNameWithoutStatus_DoesNotThrow()
    {
        var invoice = new Invoice { Status = null };

        var exception = Record.Exception(() => ShowProjectNameWriteSupport.ValidateInvoiceUpdate(invoice));

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(InvoiceStatus.Draft, true)]
    [InlineData(InvoiceStatus.Open, false)]
    [InlineData(null, false)]
    public void ShouldIncludeOnInvoiceUpdate_RespectsDraftStatus(InvoiceStatus? status, bool expected)
    {
        Assert.Equal(expected, ShowProjectNameWriteSupport.ShouldIncludeOnInvoiceUpdate(status));
    }

    [Fact]
    public void ValidateCreditNoteUpdate_ShowProjectNameWithoutStatus_Throws()
    {
        var creditNote = new CreditNote
        {
            ShowProjectName = false,
            Status = null
        };

        var exception = Assert.Throws<ArgumentException>(() => ShowProjectNameWriteSupport.ValidateCreditNoteUpdate(creditNote));

        Assert.Equal("creditNote", exception.ParamName);
    }

    [Theory]
    [InlineData(CreditNoteStatus.Draft, true)]
    [InlineData(CreditNoteStatus.Open, false)]
    [InlineData(null, false)]
    public void ShouldIncludeOnCreditNoteUpdate_RespectsDraftStatus(CreditNoteStatus? status, bool expected)
    {
        Assert.Equal(expected, ShowProjectNameWriteSupport.ShouldIncludeOnCreditNoteUpdate(status));
    }
}
