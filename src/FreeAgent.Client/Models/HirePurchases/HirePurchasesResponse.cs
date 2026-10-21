using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for hire purchases list API responses.
/// </summary>
public sealed class HirePurchasesResponse
{
    /// <summary>
    /// Hire purchases list payload.
    /// </summary>
    [JsonPropertyName("hire_purchases")]
    public List<HirePurchase>? HirePurchases { get; set; }
}
