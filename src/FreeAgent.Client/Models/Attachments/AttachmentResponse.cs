using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Wrapper for single attachment API responses.
/// </summary>
internal sealed class AttachmentResponse
{
    /// <summary>
    /// Attachment payload.
    /// </summary>
    [JsonPropertyName("attachment")]
    public Attachment? Attachment { get; set; }
}
