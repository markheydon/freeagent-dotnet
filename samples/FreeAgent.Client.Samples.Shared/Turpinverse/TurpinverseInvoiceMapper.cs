using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Maps Turpinverse invoice canon entries to FreeAgent invoice payloads.
/// </summary>
internal static class TurpinverseInvoiceMapper
{
    public const string ReferencePrefix = "turpinverse:";

    public static Invoice ToFreeAgentInvoice(
        TurpinverseInvoice invoice,
        long contactId,
        long? projectId,
        DateOnly minimumDocumentDate)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        var issueDate = TurpinverseSalesDateSupport.ParseCanonDate(invoice.IssueDate, nameof(invoice.IssueDate));
        var dueDate = TurpinverseSalesDateSupport.ParseCanonDate(invoice.DueDate, nameof(invoice.DueDate));
        var (datedOn, dueOn) = TurpinverseSalesDateSupport.ClampInvoiceDates(issueDate, dueDate, minimumDocumentDate);
        var paymentTermsInDays = Math.Max(0, dueOn.DayNumber - datedOn.DayNumber);

        return new Invoice
        {
            ContactId = contactId,
            ProjectId = projectId,
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

    /// <summary>
    /// Returns whether a canon invoice should be marked as sent in FreeAgent.
    /// Canon <c>Paid</c> entries are included so drafts transition to open; payment recording is out of scope.
    /// </summary>
    public static bool ShouldMarkAsSent(string status) =>
        string.Equals(status, "Authorised", StringComparison.OrdinalIgnoreCase)
        || string.Equals(status, "Paid", StringComparison.OrdinalIgnoreCase)
        || string.Equals(status, "Overdue", StringComparison.OrdinalIgnoreCase);

    public static bool ShouldMarkAsCancelled(string status) =>
        string.Equals(status, "Void", StringComparison.OrdinalIgnoreCase);

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

}
