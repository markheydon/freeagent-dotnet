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
    public void DeserializeInvoice_MapsRecurringInvoiceUriLinkWithoutExpandingNestedObject()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/invoices/9",
              "recurring_invoice": "https://api.freeagent.com/v2/recurring_invoices/123"
            }
            """;

        var invoice = JsonSerializer.Deserialize<Invoice>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(invoice);
        Assert.Equal(123, invoice!.RecurringInvoiceId);
        Assert.Null(invoice.RecurringInvoice);
    }

    [Fact]
    public void DeserializeInvoiceItem_MapsItemIdFromUrlWhenIdOmitted()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/invoice_items/17",
              "description": "Consulting",
              "item_type": "Hours",
              "price": "100.0"
            }
            """;

        var item = JsonSerializer.Deserialize<InvoiceItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(17, item!.ItemId);
    }

    [Fact]
    public void DeserializeInvoiceItem_MapsItemIdAndBlankItemType()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/invoice_items/42",
              "id": 42,
              "description": "Consulting",
              "item_type": "",
              "price": "100.0"
            }
            """;

        var item = JsonSerializer.Deserialize<InvoiceItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(42, item!.ItemId);
        Assert.Null(item.ItemType);
    }

    [Fact]
    public void WritePayload_SetContactId_OverridesRoundTrippedContact()
    {
        var invoice = JsonSerializer.Deserialize<Invoice>("""
            {
              "url": "https://api.freeagent.com/v2/invoices/1",
              "contact": "https://api.freeagent.com/v2/contacts/8",
              "dated_on": "2024-03-18",
              "payment_terms_in_days": 14
            }
            """)!;
        invoice.ContactId = 3;

        var payload = InvoiceWritePayload.FromInvoice(invoice, FreeAgentEnvironment.Production);

        Assert.Equal("https://api.freeagent.com/v2/contacts/3", payload.Contact!.Value.Uri);
    }

    [Fact]
    public void WritePayload_OmitLineItems_ExcludesLineItems()
    {
        var invoice = new Invoice
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    ItemId = 42,
                    Description = "Consulting"
                }
            ]
        };

        var payload = InvoiceWritePayload.FromInvoice(invoice, FreeAgentEnvironment.Production, omitLineItems: true);

        Assert.Null(payload.InvoiceItems);
    }

    [Fact]
    public void SerializeUpdatePayload_UsesItemIdNotUrl()
    {
        var invoice = new Invoice
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    Url = "https://api.freeagent.com/v2/invoice_items/42",
                    ItemId = 42,
                    Description = "Consulting"
                }
            ]
        };

        var payload = InvoiceWritePayload.FromInvoice(invoice, FreeAgentEnvironment.Production);
        var json = JsonSerializer.Serialize(new InvoiceRequest { Invoice = payload }, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"id\":42", json, StringComparison.Ordinal);
        Assert.DoesNotContain("invoice_items/42", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeCreatePayload_IncludesRequiredFields()
    {
        var invoice = new Invoice
        {
            ContactId = 2,
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
                    CategoryNominalCode = "001"
                }
            ]
        };

        var payload = InvoiceWritePayload.FromInvoice(invoice, FreeAgentEnvironment.Production);
        var json = JsonSerializer.Serialize(new InvoiceRequest { Invoice = payload }, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", json, StringComparison.Ordinal);
        Assert.Contains("\"dated_on\":\"2024-03-18\"", json, StringComparison.Ordinal);
        Assert.Contains("\"payment_terms_in_days\":14", json, StringComparison.Ordinal);
        Assert.Contains("categories/001", json, StringComparison.Ordinal);
    }

    [Fact]
    public void DeserializeInvoiceItem_MapsStockItemUriLink()
    {
        const string json = """
            {
              "description": "Widget",
              "item_type": "Stock",
              "stock_item": "https://api.freeagent.com/v2/stock_items/3"
            }
            """;

        var item = JsonSerializer.Deserialize<InvoiceItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(3, item!.StockItemId);
        Assert.Null(item.StockItemResource);
    }

    [Fact]
    public void DeserializeInvoiceItem_MapsNestedStockItemWithoutExpandingUri()
    {
        const string json = """
            {
              "description": "Widget",
              "item_type": "Stock",
              "stock_item": {
                "url": "https://api.freeagent.com/v2/stock_items/5",
                "description": "Widget stock"
              }
            }
            """;

        var item = JsonSerializer.Deserialize<InvoiceItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(5, item!.StockItemId);
        Assert.Equal("Widget stock", item.StockItemResource?.Description);
    }

    [Fact]
    public void WritePayload_RoundTripsStockItemFromUriLink()
    {
        var item = JsonSerializer.Deserialize<InvoiceItem>("""
            {
              "description": "Widget",
              "item_type": "Stock",
              "stock_item": "https://api.freeagent.com/v2/stock_items/3"
            }
            """, FreeAgentJsonSerializer.Options)!;

        var payload = InvoiceItemWritePayload.FromInvoiceItem(item, FreeAgentEnvironment.Production);
        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"stock_item\":\"https://api.freeagent.com/v2/stock_items/3\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeCreatePayload_IncludesStockItemReference()
    {
        var invoice = new Invoice
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            PaymentTermsInDays = 14,
            InvoiceItems =
            [
                new InvoiceItem
                {
                    Description = "Widget",
                    ItemType = InvoiceItemType.Stock,
                    Quantity = 1,
                    Price = 10,
                    StockItemId = 3
                }
            ]
        };

        var payload = InvoiceWritePayload.FromInvoice(invoice, FreeAgentEnvironment.Production);
        var json = JsonSerializer.Serialize(new InvoiceRequest { Invoice = payload }, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"stock_item\":\"https://api.freeagent.com/v2/stock_items/3\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeUpdatePayload_IncludesDestroyFlag()
    {
        var invoice = new Invoice
        {
            ContactId = 2,
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

        var payload = InvoiceWritePayload.FromInvoice(invoice, FreeAgentEnvironment.Production);
        var json = JsonSerializer.Serialize(new InvoiceRequest { Invoice = payload }, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"id\":42", json, StringComparison.Ordinal);
        Assert.Contains("\"_destroy\":1", json, StringComparison.Ordinal);
    }
}
