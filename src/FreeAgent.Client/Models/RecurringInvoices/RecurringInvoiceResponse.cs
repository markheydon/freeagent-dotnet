using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.RecurringInvoices;

/// <summary>
/// Response envelope for single recurring invoice operations.
/// </summary>
public sealed class RecurringInvoiceResponse
{
    /// <summary>
    /// Recurring invoice returned by the get endpoint.
    /// </summary>
    [JsonPropertyName("recurring_invoice")]
    public RecurringInvoice? RecurringInvoice { get; set; }
}
