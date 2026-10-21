using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Writable attachment attributes for expense create and update requests.
/// </summary>
public sealed class ExpenseAttachmentWritePayload
{
    /// <summary>
    /// Base64-encoded attachment data for a new upload.
    /// </summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }

    /// <summary>
    /// Existing attachment URI to link instead of uploading <see cref="Data"/>.
    /// </summary>
    [JsonPropertyName("file")]
    public string? File { get; set; }

    /// <summary>
    /// Attachment file name.
    /// </summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>
    /// Attachment description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// MIME content type of the attachment.
    /// </summary>
    [JsonPropertyName("content_type")]
    public BillAttachmentContentType? ContentType { get; set; }
}
