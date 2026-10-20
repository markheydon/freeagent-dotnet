using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Response envelope for a paginated bill list.
/// </summary>
public sealed class BillsResponse
{
    /// <summary>
    /// Bills on this page.
    /// </summary>
    [JsonPropertyName("bills")]
    public List<Bill>? Bills { get; set; }
}
