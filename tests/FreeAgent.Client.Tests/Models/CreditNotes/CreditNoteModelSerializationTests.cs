using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.CreditNotes;

public class CreditNoteModelSerializationTests
{
    [Fact]
    public void DeserializeCreditNote_MapsStatusDatesAndEnums()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/credit_notes/3",
              "status": "Written-off",
              "contact": "https://api.freeagent.com/v2/contacts/2",
              "dated_on": "2011-08-29",
              "due_on": "2011-09-02",
              "refunded_on": "2011-10-01",
              "written_off_date": "2011-11-01",
              "payment_terms_in_days": 5,
              "ec_status": "EC Goods",
              "created_at": "2011-08-29T00:00:00Z",
              "updated_at": "2011-08-29T00:00:00Z",
              "credit_note_items": [
                {
                  "description": "Refund",
                  "item_type": "Hours",
                  "price": "-100.0",
                  "quantity": "1.0",
                  "sales_tax_status": "TAXABLE",
                  "suffers_cis_deduction": false,
                  "category": "https://api.freeagent.com/v2/categories/001"
                }
              ]
            }
            """;

        var creditNote = JsonSerializer.Deserialize<CreditNote>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(creditNote);
        Assert.Equal(3, creditNote!.ResourceId);
        Assert.Equal(CreditNoteStatus.WrittenOff, creditNote.Status);
        Assert.Equal(new DateOnly(2011, 8, 29), creditNote.DatedOn);
        Assert.Equal(new DateOnly(2011, 10, 1), creditNote.RefundedOn);
        Assert.Equal(InvoiceEcStatus.EcGoods, creditNote.EcStatus);
        Assert.Equal(2, creditNote.ContactId);
        Assert.Single(creditNote.CreditNoteItems!);
        Assert.Equal(InvoiceItemType.Hours, creditNote.CreditNoteItems![0].ItemType);
        Assert.Equal(InvoiceSalesTaxStatus.Taxable, creditNote.CreditNoteItems[0].SalesTaxStatus);
        Assert.False(creditNote.CreditNoteItems[0].SuffersCisDeduction);
        Assert.Equal("001", creditNote.CreditNoteItems[0].CategoryNominalCode);
    }

    [Fact]
    public void DeserializeCreditNoteItem_MapsItemIdFromUrlWhenIdOmitted()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/invoice_items/17",
              "description": "Refund",
              "item_type": "Hours",
              "price": "-100.0"
            }
            """;

        var item = JsonSerializer.Deserialize<CreditNoteItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(17, item!.ItemId);
    }

    [Fact]
    public void WritePayload_FromCreditNote_SerialisesContactAndItems()
    {
        var creditNote = new CreditNote
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            CreditNoteItems =
            [
                new CreditNoteItem
                {
                    Description = "Refund",
                    ItemType = InvoiceItemType.Services,
                    Quantity = 1,
                    Price = -50,
                    Category = CategoryReference.Parse("https://api.freeagent.com/v2/categories/001")
                }
            ]
        };

        var json = JsonSerializer.Serialize(
            CreditNoteWritePayload.FromCreditNote(creditNote),
            FreeAgentJsonSerializer.Options);

        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", json, StringComparison.Ordinal);
        Assert.Contains("\"dated_on\":\"2024-03-18\"", json, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"Refund\"", json, StringComparison.Ordinal);
        Assert.Contains("\"item_type\":\"Services\"", json, StringComparison.Ordinal);
        Assert.Contains("\"category\":\"https://api.freeagent.com/v2/categories/001\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void WritePayload_OmitBillingContactFromWrite_ExcludesRoundTrippedContact()
    {
        var creditNote = JsonSerializer.Deserialize<CreditNote>("""
            {
              "url": "https://api.freeagent.com/v2/credit_notes/1",
              "contact": "https://api.freeagent.com/v2/contacts/8",
              "dated_on": "2024-03-18",
              "payment_terms_in_days": 14
            }
            """)!;
        creditNote.OmitBillingContactFromWrite = true;

        var payload = CreditNoteWritePayload.FromCreditNote(creditNote);

        Assert.Null(payload.Contact);
    }

    [Fact]
    public void WritePayload_OmitProjectFromWrite_ExcludesRoundTrippedProject()
    {
        var creditNote = JsonSerializer.Deserialize<CreditNote>("""
            {
              "url": "https://api.freeagent.com/v2/credit_notes/1",
              "project": "https://api.freeagent.com/v2/projects/4",
              "dated_on": "2024-03-18",
              "payment_terms_in_days": 14
            }
            """)!;
        creditNote.OmitProjectFromWrite = true;

        var payload = CreditNoteWritePayload.FromCreditNote(creditNote);

        Assert.Null(payload.Project);
    }

    [Fact]
    public void WritePayload_OmitBankAccountFromWrite_ExcludesRoundTrippedBankAccount()
    {
        var creditNote = JsonSerializer.Deserialize<CreditNote>("""
            {
              "url": "https://api.freeagent.com/v2/credit_notes/1",
              "bank_account": "https://api.freeagent.com/v2/bank_accounts/2",
              "dated_on": "2024-03-18",
              "payment_terms_in_days": 14
            }
            """)!;
        creditNote.OmitBankAccountFromWrite = true;

        var payload = CreditNoteWritePayload.FromCreditNote(creditNote);

        Assert.Null(payload.BankAccount);
    }

    [Fact]
    public void WritePayload_OmitCreditNoteItemsFromWrite_ExcludesLineItems()
    {
        var creditNote = new CreditNote
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 0,
            OmitCreditNoteItemsFromWrite = true,
            CreditNoteItems =
            [
                new CreditNoteItem
                {
                    ItemId = 42,
                    Description = "Refund"
                }
            ]
        };

        var payload = CreditNoteWritePayload.FromCreditNote(creditNote);

        Assert.Null(payload.CreditNoteItems);
    }
}
