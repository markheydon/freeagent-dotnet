using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Invoices;

public class InvoiceModelSerializationTests
{
    [Fact]
    public void DeserializeInvoice_MapsStatusDatesAndEnums()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/invoices/3",
              "status": "Scheduled To Email",
              "contact": "https://api.freeagent.com/v2/contacts/2",
              "dated_on": "2011-08-29",
              "due_on": "2011-09-02",
              "payment_terms_in_days": 5,
              "ec_status": "EC Goods",
              "created_at": "2011-08-29T00:00:00Z",
              "updated_at": "2011-08-29T00:00:00Z",
              "invoice_items": [
                {
                  "description": "Consulting",
                  "item_type": "Hours",
                  "price": "100.0",
                  "quantity": "2.0",
                  "sales_tax_status": "TAXABLE",
                  "category": "https://api.freeagent.com/v2/categories/001"
                }
              ]
            }
            """;

        var invoice = JsonSerializer.Deserialize<Invoice>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(invoice);
        Assert.Equal(3, invoice!.ResourceId);
        Assert.Equal(InvoiceStatus.ScheduledToEmail, invoice.Status);
        Assert.Equal(new DateOnly(2011, 8, 29), invoice.DatedOn);
        Assert.Equal(InvoiceEcStatus.EcGoods, invoice.EcStatus);
        Assert.Equal(2, invoice.ContactId);
        Assert.Single(invoice.InvoiceItems!);
        Assert.Equal(InvoiceItemType.Hours, invoice.InvoiceItems![0].ItemType);
        Assert.Equal(InvoiceSalesTaxStatus.Taxable, invoice.InvoiceItems[0].SalesTaxStatus);
        Assert.Equal("001", invoice.InvoiceItems[0].CategoryNominalCode);
    }

    [Fact]
    public void SerializeCreatePayload_IncludesRequiredFields()
    {
        var invoice = new Invoice
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    Description = "Consulting",
                    ItemType = InvoiceItemType.Hours,
                    Quantity = 2,
                    Price = 100,
                    Category = CategoryReference.ForEnvironment(FreeAgentEnvironment.Production, "001")
                }
            ]
        };

        var payload = InvoiceWritePayload.FromInvoice(invoice);
        var json = JsonSerializer.Serialize(new InvoiceRequest { Invoice = payload }, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", json, StringComparison.Ordinal);
        Assert.Contains("\"dated_on\":\"2024-03-18\"", json, StringComparison.Ordinal);
        Assert.Contains("\"payment_terms_in_days\":14", json, StringComparison.Ordinal);
        Assert.Contains("categories/001", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeUpdatePayload_IncludesDestroyFlag()
    {
        var invoice = new Invoice
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/2"),
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    ItemId = 42,
                    Destroy = 1
                }
            ]
        };

        var payload = InvoiceWritePayload.FromInvoice(invoice);
        var json = JsonSerializer.Serialize(new InvoiceRequest { Invoice = payload }, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"id\":42", json, StringComparison.Ordinal);
        Assert.Contains("\"_destroy\":1", json, StringComparison.Ordinal);
    }
}
