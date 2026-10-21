using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for a paginated invoice list.
/// </summary>
public sealed class InvoicesResponse
{
    /// <summary>
    /// Invoice list payload.
    /// </summary>
    [JsonPropertyName("invoices")]
    public List<Invoice>? Invoices { get; set; }
}
