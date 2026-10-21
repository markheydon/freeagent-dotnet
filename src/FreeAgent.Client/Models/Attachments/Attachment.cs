using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Represents a FreeAgent attachment fetched by identifier.
/// </summary>
public sealed class Attachment : IFreeAgentResource
{
    /// <summary>
    /// Attachment resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// URL to the original full-size file.
    /// </summary>
    [JsonPropertyName("content_src")]
    public string? ContentSrc { get; set; }

    /// <summary>
    /// URL to a medium-size thumbnail when one exists.
    /// </summary>
    [JsonPropertyName("content_src_medium")]
    public string? ContentSrcMedium { get; set; }

    /// <summary>
    /// URL to a small-size thumbnail when one exists.
    /// </summary>
    [JsonPropertyName("content_src_small")]
    public string? ContentSrcSmall { get; set; }

    /// <summary>
    /// When the <see cref="ContentSrc"/> URLs expire. Fetch the attachment again after this time.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// MIME content type of the attachment.
    /// </summary>
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    /// <summary>
    /// Original file name.
    /// </summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>
    /// Size of the original file in bytes.
    /// </summary>
    [JsonPropertyName("file_size")]
    public int? FileSize { get; set; }

    /// <summary>
    /// Free-text description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
