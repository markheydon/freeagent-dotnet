using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.RecurringInvoices;

/// <summary>
/// Response envelope for recurring invoice list operations.
/// </summary>
public sealed class RecurringInvoicesResponse
{
    /// <summary>
    /// Recurring invoices returned by the list endpoint.
    /// </summary>
    [JsonPropertyName("recurring_invoices")]
    public List<RecurringInvoice>? RecurringInvoices { get; set; }
}
