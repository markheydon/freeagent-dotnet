using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Invoices;

/// <summary>
/// Response envelope for company default invoice additional text.
/// </summary>
public sealed class DefaultAdditionalTextResponse
{
    /// <summary>
    /// Default additional text shown on all invoices.
    /// </summary>
    [JsonPropertyName("default_additional_text")]
    public string? DefaultAdditionalText { get; set; }
}

/// <summary>
/// Request envelope for updating company default invoice additional text.
/// </summary>
internal sealed class DefaultAdditionalTextRequest
{
    [JsonPropertyName("default_additional_text")]
    public string? DefaultAdditionalText { get; set; }
}
