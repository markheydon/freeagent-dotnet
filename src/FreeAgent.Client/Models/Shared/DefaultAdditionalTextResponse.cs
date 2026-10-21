using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Response envelope for company default additional text on invoices or estimates.
/// </summary>
public sealed class DefaultAdditionalTextResponse
{
    /// <summary>
    /// Default additional text shown on all invoices or estimates (depending on the endpoint).
    /// </summary>
    [JsonPropertyName("default_additional_text")]
    public string? DefaultAdditionalText { get; set; }
}

/// <summary>
/// Request envelope for updating company default additional text on invoices or estimates.
/// </summary>
internal sealed class DefaultAdditionalTextRequest
{
    [JsonPropertyName("default_additional_text")]
    public string? DefaultAdditionalText { get; set; }
}
