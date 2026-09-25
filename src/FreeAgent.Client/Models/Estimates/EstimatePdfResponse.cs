using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// PDF payload returned by the estimate PDF endpoint.
/// </summary>
public sealed class EstimatePdfContent
{
    /// <summary>
    /// Base64-encoded PDF data.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// Response envelope for an estimate PDF download.
/// </summary>
public sealed class EstimatePdfResponse
{
    /// <summary>
    /// PDF payload.
    /// </summary>
    [JsonPropertyName("pdf")]
    public EstimatePdfContent? Pdf { get; set; }
}
