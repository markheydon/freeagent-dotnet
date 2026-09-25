using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.RecurringInvoices;

namespace FreeAgent.Client.Tests.Models.RecurringInvoices;

public class RecurringInvoiceModelSerializationTests
{
    [Fact]
    public void DeserializeRecurringInvoicesResponse_MapsEnvelope()
    {
        const string json = """
            {
              "recurring_invoices": [
                {
                  "url": "https://api.freeagent.com/v2/recurring_invoices/1",
                  "reference": "002",
                  "frequency": "Weekly",
                  "recurring_status": "Draft"
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<RecurringInvoicesResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response);
        Assert.NotNull(response!.RecurringInvoices);
        Assert.Single(response.RecurringInvoices);
        Assert.Equal(RecurringInvoiceFrequency.Weekly, response.RecurringInvoices[0].Frequency);
        Assert.Equal(RecurringInvoiceStatus.Draft, response.RecurringInvoices[0].RecurringStatus);
    }

    [Fact]
    public void DeserializeRecurringInvoiceResponse_MapsEnvelope()
    {
        const string json = """
            {
              "recurring_invoice": {
                "url": "https://api.freeagent.com/v2/recurring_invoices/3",
                "reference": "003"
              }
            }
            """;

        var response = JsonSerializer.Deserialize<RecurringInvoiceResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response);
        Assert.NotNull(response!.RecurringInvoice);
        Assert.Equal(3, response.RecurringInvoice!.ResourceId);
    }

    [Fact]
    public void DeserializeRecurringInvoice_MapsDatesEnumsAndLineItems()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/recurring_invoices/1",
              "contact": "https://api.freeagent.com/v2/contacts/1",
              "contact_name": "Nathan Barley",
              "dated_on": "2012-02-29",
              "frequency": "Two Weekly",
              "next_recurs_on": "2012-03-07",
              "recurring_end_date": "2012-05-16",
              "recurring_status": "Draft",
              "reference": "002",
              "currency": "GBP",
              "exchange_rate": "1.0",
              "net_value": "2.0",
              "sales_tax_value": "0.4",
              "total_value": "2.4",
              "omit_header": false,
              "always_show_bic_and_iban": false,
              "payment_terms_in_days": 30,
              "created_at": "2012-02-29T00:00:00Z",
              "updated_at": "2012-02-29T00:00:00Z",
              "invoice_items": [
                {
                  "url": "https://api.freeagent.com/v2/invoice_items/1",
                  "position": 1,
                  "description": "Item",
                  "item_type": "Hours",
                  "price": "2.0",
                  "quantity": "1.0",
                  "sales_tax_rate": "20.0",
                  "sales_tax_status": "TAXABLE",
                  "category": "https://api.freeagent.com/v2/categories/001"
                }
              ],
              "payment_methods": {
                "paypal": true,
                "stripe": false
              }
            }
            """;

        var recurringInvoice = JsonSerializer.Deserialize<RecurringInvoice>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(recurringInvoice);
        Assert.Equal(1, recurringInvoice!.ResourceId);
        Assert.Equal(1, recurringInvoice.ContactId);
        Assert.Equal("Nathan Barley", recurringInvoice.ContactName);
        Assert.Equal(new DateOnly(2012, 2, 29), recurringInvoice.DatedOn);
        Assert.Equal(RecurringInvoiceFrequency.TwoWeekly, recurringInvoice.Frequency);
        Assert.Equal(RecurringInvoiceStatus.Draft, recurringInvoice.RecurringStatus);
        Assert.Equal(new DateOnly(2012, 3, 7), recurringInvoice.NextRecursOn);
        Assert.Equal(new DateOnly(2012, 5, 16), recurringInvoice.RecurringEndDate);
        Assert.Equal(2.0m, recurringInvoice.NetValue);
        Assert.Equal(2.4m, recurringInvoice.TotalValue);
        Assert.Single(recurringInvoice.InvoiceItems!);
        Assert.Equal(InvoiceItemType.Hours, recurringInvoice.InvoiceItems![0].ItemType);
        Assert.Equal(InvoiceSalesTaxStatus.Taxable, recurringInvoice.InvoiceItems[0].SalesTaxStatus);
        Assert.True(recurringInvoice.PaymentMethods!.PayPal);
        Assert.False(recurringInvoice.PaymentMethods.Stripe);
    }

    [Theory]
    [InlineData("2-Yearly", RecurringInvoiceFrequency.TwoYearly)]
    [InlineData("Biannually", RecurringInvoiceFrequency.Biannually)]
    [InlineData("Four Weekly", RecurringInvoiceFrequency.FourWeekly)]
    public void DeserializeRecurringInvoice_MapsFrequencyWireValues(string wireValue, RecurringInvoiceFrequency expected)
    {
        var json = $$"""
            {
              "url": "https://api.freeagent.com/v2/recurring_invoices/1",
              "frequency": "{{wireValue}}"
            }
            """;

        var recurringInvoice = JsonSerializer.Deserialize<RecurringInvoice>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(recurringInvoice);
        Assert.Equal(expected, recurringInvoice!.Frequency);
    }
}
