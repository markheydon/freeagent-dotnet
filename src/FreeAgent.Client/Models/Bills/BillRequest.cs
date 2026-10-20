using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Bills;

/// <summary>
/// Request envelope for creating or updating a bill.
/// </summary>
internal sealed class BillRequest
{
    [JsonPropertyName("bill")]
    public BillWritePayload? Bill { get; set; }
}
