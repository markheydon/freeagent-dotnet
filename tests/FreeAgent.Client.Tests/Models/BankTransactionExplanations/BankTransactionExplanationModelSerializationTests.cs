using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankTransactionExplanations;
using FreeAgent.Client.Models.Bills;

namespace FreeAgent.Client.Tests.Models.BankTransactionExplanations;

public class BankTransactionExplanationModelSerializationTests
{
    [Fact]
    public void Deserialize_ReadsAttachmentAndAttachmentsShapes()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/bank_transaction_explanations/20",
          "dated_on": "2019-12-01",
          "gross_value": "-90.0",
          "updated_at": "2020-02-06T11:08:28.000Z",
          "attachment": {
            "url": "https://api.freeagent.com/v2/attachments/3",
            "file_name": "About Stacks.pdf"
          },
          "attachments": [
            {
              "url": "https://api.freeagent.com/v2/attachments/4",
              "file_name": "second.pdf"
            }
          ]
        }
        """;

        var explanation = JsonSerializer.Deserialize<BankTransactionExplanation>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(explanation);
        Assert.Equal(new DateOnly(2019, 12, 1), explanation.DatedOn);
        Assert.Equal(-90m, explanation.GrossValue);
        Assert.Equal("About Stacks.pdf", explanation.Attachment?.FileName);
        Assert.NotNull(explanation.Attachments);
        Assert.Single(explanation.Attachments);
        Assert.Equal("second.pdf", explanation.Attachments[0].FileName);
    }

    [Fact]
    public void Deserialize_ReadsCategoryNominalCodeFromUriLink()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
          "category": "https://api.freeagent.com/v2/categories/285"
        }
        """;

        var explanation = JsonSerializer.Deserialize<BankTransactionExplanation>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(explanation);
        Assert.Equal("285", explanation.CategoryNominalCode);
    }

    [Fact]
    public void Deserialize_ReadsPaidInvoiceAndCreditNoteIdsFromPaidInvoiceLink()
    {
        const string invoiceJson = """
        {
          "url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
          "paid_invoice": "https://api.freeagent.com/v2/invoices/3"
        }
        """;

        var invoiceExplanation = JsonSerializer.Deserialize<BankTransactionExplanation>(invoiceJson, FreeAgentJsonSerializer.Options);

        Assert.NotNull(invoiceExplanation);
        Assert.Equal(3, invoiceExplanation.PaidInvoiceId);
        Assert.Null(invoiceExplanation.PaidCreditNoteId);

        const string creditNoteJson = """
        {
          "url": "https://api.freeagent.com/v2/bank_transaction_explanations/2",
          "paid_invoice": "https://api.freeagent.com/v2/credit_notes/5"
        }
        """;

        var creditNoteExplanation = JsonSerializer.Deserialize<BankTransactionExplanation>(creditNoteJson, FreeAgentJsonSerializer.Options);

        Assert.NotNull(creditNoteExplanation);
        Assert.Null(creditNoteExplanation.PaidInvoiceId);
        Assert.Equal(5, creditNoteExplanation.PaidCreditNoteId);
    }

    [Fact]
    public void Deserialize_ReadsEcStatusAndRebillTypeWireValues()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
          "ec_status": "EC VAT MOSS",
          "sales_tax_status": "TAXABLE",
          "rebill_type": "markup"
        }
        """;

        var explanation = JsonSerializer.Deserialize<BankTransactionExplanation>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(explanation);
        Assert.Equal(BankTransactionExplanationEcStatus.EcVatMoss, explanation.EcStatus);
        Assert.Equal(BillSalesTaxStatus.Taxable, explanation.SalesTaxStatus);
        Assert.Equal(BillRebillType.Markup, explanation.RebillType);
    }

    [Fact]
    public void Deserialize_ReadsExplanationTypeWireValue()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/bank_transaction_explanations/1",
          "type": "Payment"
        }
        """;

        var explanation = JsonSerializer.Deserialize<BankTransactionExplanation>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(explanation);
        Assert.Equal(BankTransactionExplanationType.Payment, explanation.Type);
    }
}
