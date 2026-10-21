using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Bills;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Bills;

public class BillModelSerializationTests
{
    [Fact]
    public void DeserializeBill_MapsStatusDatesAndEnums()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/bills/3",
              "status": "Overdue",
              "contact": "https://api.freeagent.com/v2/contacts/2",
              "dated_on": "2011-08-29",
              "due_on": "2011-09-02",
              "paid_on": "2011-10-01",
              "ec_status": "EC Goods",
              "is_paid_by_hire_purchase": true,
              "created_at": "2011-08-29T00:00:00Z",
              "updated_at": "2011-08-29T00:00:00Z",
              "bill_items": [
                {
                  "description": "Office supplies",
                  "total_value": "120.0",
                  "sales_tax_status": "TAXABLE",
                  "category": "https://api.freeagent.com/v2/categories/285"
                }
              ]
            }
            """;

        var bill = JsonSerializer.Deserialize<Bill>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(bill);
        Assert.Equal(3, bill!.ResourceId);
        Assert.Equal(BillStatus.Overdue, bill.Status);
        Assert.Equal(new DateOnly(2011, 8, 29), bill.DatedOn);
        Assert.Equal(new DateOnly(2011, 10, 1), bill.PaidOn);
        Assert.Equal(BillEcStatus.EcGoods, bill.EcStatus);
        Assert.True(bill.IsPaidByHirePurchase);
        Assert.Equal(2, bill.ContactId);
        Assert.Single(bill.BillItems!);
        Assert.Equal(BillSalesTaxStatus.Taxable, bill.BillItems![0].SalesTaxStatus);
        Assert.Equal("285", bill.BillItems[0].CategoryNominalCode);
    }

    [Fact]
    public void DeserializeBill_MapsRebilledInvoiceAndItemIdsFromUri()
    {
        const string invoiceItemJson = """
            {
              "url": "https://api.freeagent.com/v2/bills/1",
              "rebilled_on_invoice_item": "https://api.freeagent.com/v2/invoice_items/17"
            }
            """;
        const string invoiceJson = """
            {
              "url": "https://api.freeagent.com/v2/bills/2",
              "rebilled_on_invoice_item": "https://api.freeagent.com/v2/invoices/9"
            }
            """;

        var fromItem = JsonSerializer.Deserialize<Bill>(invoiceItemJson, FreeAgentJsonSerializer.Options);
        var fromInvoice = JsonSerializer.Deserialize<Bill>(invoiceJson, FreeAgentJsonSerializer.Options);

        Assert.NotNull(fromItem);
        Assert.Equal(17, fromItem!.RebilledOnInvoiceItemId);
        Assert.Null(fromItem.RebilledOnInvoiceId);

        Assert.NotNull(fromInvoice);
        Assert.Equal(9, fromInvoice!.RebilledOnInvoiceId);
        Assert.Null(fromInvoice.RebilledOnInvoiceItemId);
    }

    [Fact]
    public void DeserializeBillItem_MapsBillIdFromBillLink()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/bill_items/17",
              "bill": "https://api.freeagent.com/v2/bills/9",
              "description": "Supplier line",
              "total_value": "50.0"
            }
            """;

        var item = JsonSerializer.Deserialize<BillItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(9, item!.BillId);
        Assert.Null(item.Bill);
    }

    [Fact]
    public void DeserializeBillItem_MapsItemIdFromUrlWhenIdOmitted()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/bill_items/17",
              "description": "Supplier line",
              "total_value": "50.0"
            }
            """;

        var item = JsonSerializer.Deserialize<BillItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(17, item!.ItemId);
    }

    [Fact]
    public void DeserializeBillItem_MapsOutOfScopeSalesTaxStatus()
    {
        const string json = """
            {
              "description": "Out of scope purchase",
              "sales_tax_status": "OUT_OF_SCOPE"
            }
            """;

        var item = JsonSerializer.Deserialize<BillItem>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(BillSalesTaxStatus.OutOfScope, item!.SalesTaxStatus);
    }

    [Fact]
    public void WritePayload_FromBill_SerialisesContactAndItems()
    {
        var bill = new Bill
        {
            ContactId = 2,
            DatedOn = new DateOnly(2024, 3, 18),
            IsPaidByHirePurchase = false,
            BillItems =
            [
                new BillItem
                {
                    Description = "Supplier charge",
                    TotalValue = 120m,
                    SalesTaxStatus = BillSalesTaxStatus.Taxable,
                    CategoryNominalCode = "285"
                }
            ]
        };

        var json = JsonSerializer.Serialize(
            BillWritePayload.FromBill(bill, FreeAgentEnvironment.Production),
            FreeAgentJsonSerializer.Options);

        Assert.Contains("\"contact\":\"https://api.freeagent.com/v2/contacts/2\"", json, StringComparison.Ordinal);
        Assert.Contains("\"dated_on\":\"2024-03-18\"", json, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"Supplier charge\"", json, StringComparison.Ordinal);
        Assert.Contains("\"category\":\"https://api.freeagent.com/v2/categories/285\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void WritePayload_FromBill_SerialisesHirePurchaseFlag()
    {
        var bill = new Bill
        {
            ContactId = 1,
            DatedOn = new DateOnly(2024, 1, 1),
            DueOn = new DateOnly(2024, 1, 31),
            IsPaidByHirePurchase = true,
            Reference = "HP-001",
            BillItems =
            [
                new BillItem
                {
                    Description = "Asset",
                    TotalValue = 1000m,
                    CategoryNominalCode = "285"
                }
            ]
        };

        var json = JsonSerializer.Serialize(
            BillWritePayload.FromBill(bill, FreeAgentEnvironment.Production),
            FreeAgentJsonSerializer.Options);

        Assert.Contains("\"is_paid_by_hire_purchase\":true", json, StringComparison.Ordinal);
    }
}
