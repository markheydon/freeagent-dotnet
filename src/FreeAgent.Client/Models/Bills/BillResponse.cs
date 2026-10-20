using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Response envelope for a single bill.
/// </summary>
public sealed class BillResponse
{
    /// <summary>
    /// Bill details.
    /// </summary>
    [JsonPropertyName("bill")]
    public Bill? Bill { get; set; }
}
