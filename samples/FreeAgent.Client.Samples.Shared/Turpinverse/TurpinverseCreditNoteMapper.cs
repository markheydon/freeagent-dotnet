using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Maps Turpinverse credit note canon entries to FreeAgent credit note payloads.
/// </summary>
internal static class TurpinverseCreditNoteMapper
{
    public const string ReferencePrefix = "turpinverse:";

    public static CreditNote ToFreeAgentCreditNote(
        TurpinverseCreditNote creditNote,
        long contactId)
    {
        ArgumentNullException.ThrowIfNull(creditNote);

        var datedOn = ParseDate(creditNote.IssueDate, nameof(creditNote.IssueDate));

        return new CreditNote
        {
            ContactId = contactId,
            Reference = BuildReference(creditNote.CreditNoteId),
            PoReference = creditNote.CreditNoteNumber,
            DatedOn = datedOn,
            DueOn = datedOn,
            PaymentTermsInDays = 0,
            Currency = ParseCurrency(creditNote.Currency),
            Comments = creditNote.Notes,
            CreditNoteItems = creditNote.Lines
                .Where(static line => line.UnitPrice != 0)
                .Select(static (line, index) => ToCreditNoteItem(line, index + 1))
                .ToList()
        };
    }

    public static string BuildReference(string creditNoteId) =>
        $"{ReferencePrefix}{creditNoteId}";

    private static CreditNoteItem ToCreditNoteItem(TurpinverseCreditNoteLine line, int position)
    {
        var (salesTaxRate, salesTaxStatus) = MapTax(line.TaxRateId);

        return new CreditNoteItem
        {
            Position = position,
            Description = line.Description,
            ItemType = InvoiceItemType.Products,
            Quantity = line.Quantity,
            Price = -Math.Abs(line.UnitPrice),
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

        throw new InvalidOperationException($"Turpinverse credit note field '{fieldName}' is not a valid date: '{value}'.");
    }
}
