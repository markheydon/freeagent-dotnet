using System.Text.Json.Serialization;

namespace FreeAgent.Client;

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
