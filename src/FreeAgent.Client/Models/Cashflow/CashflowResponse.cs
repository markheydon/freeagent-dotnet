using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Cashflow;

/// <summary>
/// API envelope for cashflow responses.
/// </summary>
public sealed class CashflowResponse
{
    /// <summary>
    /// Cashflow payload.
    /// </summary>
    [JsonPropertyName("cashflow")]
    public Cashflow? Cashflow { get; set; }
}
