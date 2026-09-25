using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Maps Turpinverse quote canon entries to FreeAgent estimate payloads.
/// </summary>
internal static class TurpinverseQuoteMapper
{
    public const string ReferencePrefix = "turpinverse:";

    public static Estimate ToFreeAgentEstimate(
        TurpinverseQuote quote,
        long contactId,
        long? projectId,
        DateOnly minimumDocumentDate)
    {
        ArgumentNullException.ThrowIfNull(quote);

        var datedOn = TurpinverseSalesDateSupport.ClampToMinimum(
            TurpinverseSalesDateSupport.ParseCanonDate(quote.IssueDate, nameof(quote.IssueDate)),
            minimumDocumentDate);

        return new Estimate
        {
            ContactId = contactId,
            ProjectId = projectId,
            EstimateType = EstimateType.Quote,
            Reference = BuildReference(quote.QuoteId),
            DatedOn = datedOn,
            Currency = ParseCurrency(quote.Currency),
            Notes = BuildNotes(quote),
            EstimateItems = quote.Lines
                .Select(static (line, index) => ToEstimateItem(line, index + 1))
                .ToList()
        };
    }

    public static string BuildReference(string quoteId) =>
        $"{ReferencePrefix}{quoteId}";

    public static EstimateStatus MapStatus(string status) =>
        status switch
        {
            "Accepted" => EstimateStatus.Approved,
            "Declined" => EstimateStatus.Rejected,
            "Expired" => EstimateStatus.Rejected,
            "Draft" => EstimateStatus.Draft,
            "Sent" => EstimateStatus.Sent,
            _ => EstimateStatus.Sent
        };

    private static EstimateItem ToEstimateItem(TurpinverseQuoteLine line, int position)
    {
        var (salesTaxRate, salesTaxStatus) = MapTax(line.TaxRateId);

        return new EstimateItem
        {
            Position = position,
            Description = line.Description,
            ItemType = EstimateItemType.Products,
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

    private static string? BuildNotes(TurpinverseQuote quote)
    {
        if (string.IsNullOrWhiteSpace(quote.Terms))
        {
            return quote.Notes;
        }

        if (string.IsNullOrWhiteSpace(quote.Notes))
        {
            return quote.Terms;
        }

        return $"{quote.Notes}\n\n{quote.Terms}";
    }
}
