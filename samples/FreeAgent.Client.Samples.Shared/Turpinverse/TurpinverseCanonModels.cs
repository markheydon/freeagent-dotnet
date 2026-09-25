using System.Text.Json.Serialization;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

public sealed class TurpinversePersona
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("organisationIds")]
    public IReadOnlyList<string> OrganisationIds { get; set; } = [];

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    public TurpinverseAddress? Address { get; set; }
}

public sealed class TurpinverseAddress
{
    [JsonPropertyName("address1")]
    public string? Address1 { get; set; }

    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    [JsonPropertyName("address3")]
    public string? Address3 { get; set; }

    [JsonPropertyName("town")]
    public string? Town { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public sealed class TurpinverseProject
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("organisationId")]
    public string OrganisationId { get; set; } = string.Empty;

    [JsonPropertyName("contactId")]
    public string? ContactId { get; set; }
}

public sealed class TurpinverseInvoice
{
    [JsonPropertyName("invoiceId")]
    public string InvoiceId { get; set; } = string.Empty;

    [JsonPropertyName("invoiceNumber")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [JsonPropertyName("contactId")]
    public string? ContactId { get; set; }

    [JsonPropertyName("dealId")]
    public string? DealId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("issueDate")]
    public string IssueDate { get; set; } = string.Empty;

    [JsonPropertyName("dueDate")]
    public string DueDate { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("taxTotal")]
    public decimal TaxTotal { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("amountDue")]
    public decimal AmountDue { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("terms")]
    public string? Terms { get; set; }

    [JsonPropertyName("lines")]
    public IReadOnlyList<TurpinverseInvoiceLine> Lines { get; set; } = [];
}

public sealed class TurpinverseInvoiceLine
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("taxRateId")]
    public string? TaxRateId { get; set; }

    [JsonPropertyName("lineTotal")]
    public decimal LineTotal { get; set; }

    [JsonPropertyName("productId")]
    public string? ProductId { get; set; }

    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }

    [JsonPropertyName("quoteId")]
    public string? QuoteId { get; set; }

    [JsonPropertyName("salesOrderId")]
    public string? SalesOrderId { get; set; }
}

public sealed class TurpinverseQuote
{
    [JsonPropertyName("quoteId")]
    public string QuoteId { get; set; } = string.Empty;

    [JsonPropertyName("quoteNumber")]
    public string QuoteNumber { get; set; } = string.Empty;

    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [JsonPropertyName("contactId")]
    public string? ContactId { get; set; }

    [JsonPropertyName("dealId")]
    public string? DealId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("issueDate")]
    public string IssueDate { get; set; } = string.Empty;

    [JsonPropertyName("expiryDate")]
    public string ExpiryDate { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("taxTotal")]
    public decimal TaxTotal { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("terms")]
    public string? Terms { get; set; }

    [JsonPropertyName("lines")]
    public IReadOnlyList<TurpinverseQuoteLine> Lines { get; set; } = [];
}

public sealed class TurpinverseQuoteLine
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("taxRateId")]
    public string? TaxRateId { get; set; }

    [JsonPropertyName("lineTotal")]
    public decimal LineTotal { get; set; }

    [JsonPropertyName("productId")]
    public string? ProductId { get; set; }

    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }
}

public sealed class TurpinverseOrganisation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("tradingName")]
    public string TradingName { get; set; } = string.Empty;

    [JsonPropertyName("primaryContactId")]
    public string? PrimaryContactId { get; set; }

    [JsonPropertyName("memberPersonaIds")]
    public IReadOnlyList<string> MemberPersonaIds { get; set; } = [];

    [JsonPropertyName("registeredOffice")]
    public TurpinverseAddress? RegisteredOffice { get; set; }
}

public sealed class TurpinverseCreditNote
{
    [JsonPropertyName("creditNoteId")]
    public string CreditNoteId { get; set; } = string.Empty;

    [JsonPropertyName("creditNoteNumber")]
    public string CreditNoteNumber { get; set; } = string.Empty;

    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [JsonPropertyName("contactId")]
    public string? ContactId { get; set; }

    [JsonPropertyName("invoiceId")]
    public string? InvoiceId { get; set; }

    [JsonPropertyName("issueDate")]
    public string IssueDate { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }

    [JsonPropertyName("taxTotal")]
    public decimal TaxTotal { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("lines")]
    public IReadOnlyList<TurpinverseCreditNoteLine> Lines { get; set; } = [];
}

public sealed class TurpinverseCreditNoteLine
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("taxRateId")]
    public string? TaxRateId { get; set; }

    [JsonPropertyName("lineTotal")]
    public decimal LineTotal { get; set; }

    [JsonPropertyName("productId")]
    public string? ProductId { get; set; }
}
