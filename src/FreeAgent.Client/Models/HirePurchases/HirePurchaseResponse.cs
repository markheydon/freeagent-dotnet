using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.HirePurchases;

/// <summary>
/// Wrapper for a single hire purchase API response.
/// </summary>
public sealed class HirePurchaseResponse
{
    /// <summary>
    /// Hire purchase payload.
    /// </summary>
    [JsonPropertyName("hire_purchase")]
    public HirePurchase? HirePurchase { get; set; }
}
