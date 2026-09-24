using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Maps Turpinverse invoice canon entries to FreeAgent invoice payloads.
/// </summary>
internal static class TurpinverseInvoiceMapper
{
    public const string ReferencePrefix = "turpinverse:";

    public static Invoice ToFreeAgentInvoice(
        TurpinverseInvoice invoice,
        ContactReference contact,
        ProjectReference? project)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        var datedOn = ParseDate(invoice.IssueDate, nameof(invoice.IssueDate));
        var dueOn = ParseDate(invoice.DueDate, nameof(invoice.DueDate));
        var paymentTermsInDays = Math.Max(0, dueOn.DayNumber - datedOn.DayNumber);

        return new Invoice
        {
            BillingContact = contact,
            LinkedProject = project,
            Reference = BuildReference(invoice.InvoiceId),
            PoReference = invoice.InvoiceNumber,
            DatedOn = datedOn,
            DueOn = dueOn,
            PaymentTermsInDays = paymentTermsInDays,
            Currency = ParseCurrency(invoice.Currency),
            Comments = invoice.Notes,
            PaymentTerms = invoice.Terms,
            SendNewInvoiceEmails = false,
            SendReminderEmails = false,
            SendThankYouEmails = false,
            InvoiceItems = invoice.Lines
                .Select(static (line, index) => ToInvoiceItem(line, index + 1))
                .ToList()
        };
    }

    public static string BuildReference(string invoiceId) =>
        $"{ReferencePrefix}{invoiceId}";

    public static bool ShouldMarkAsSent(string status) =>
        !string.Equals(status, "Draft", StringComparison.OrdinalIgnoreCase);

    private static InvoiceItem ToInvoiceItem(TurpinverseInvoiceLine line, int position)
    {
        var (salesTaxRate, salesTaxStatus) = MapTax(line.TaxRateId);

        return new InvoiceItem
        {
            Position = position,
            Description = line.Description,
            ItemType = InvoiceItemType.Products,
            Quantity = line.Quantity,
            Price = line.UnitPrice,
            SalesTaxRate = salesTaxRate,
            SalesTaxStatus = salesTaxStatus
        };
    }

    private static (decimal Rate, InvoiceSalesTaxStatus Status) MapTax(string? taxRateId) =>
        taxRateId switch
        {
            "tax-reduced" => (5m, InvoiceSalesTaxStatus.Taxable),
            "tax-zero" => (0m, InvoiceSalesTaxStatus.Exempt),
            _ => (20m, InvoiceSalesTaxStatus.Taxable)
        };

    private static CurrencyCode ParseCurrency(string currency) =>
        Enum.TryParse<CurrencyCode>(currency, ignoreCase: true, out var parsed)
            ? parsed
            : CurrencyCode.GBP;

    private static DateOnly ParseDate(string value, string fieldName)
    {
        if (DateOnly.TryParse(value, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"Turpinverse invoice field '{fieldName}' is not a valid date: '{value}'.");
    }
}
