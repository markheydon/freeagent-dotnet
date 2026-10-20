using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Expenses;

/// <summary>
/// Attachment metadata returned on expense read responses.
/// </summary>
public sealed class ExpenseAttachment
{
    /// <summary>
    /// Attachment resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Download URL for the attachment content.
    /// </summary>
    [JsonPropertyName("content_src")]
    public string? ContentSrc { get; set; }

    /// <summary>
    /// MIME content type of the attachment.
    /// </summary>
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    /// <summary>
    /// Attachment file name.
    /// </summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>
    /// Attachment file size in bytes.
    /// </summary>
    [JsonPropertyName("file_size")]
    public int? FileSize { get; set; }

    /// <summary>
    /// Attachment description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
