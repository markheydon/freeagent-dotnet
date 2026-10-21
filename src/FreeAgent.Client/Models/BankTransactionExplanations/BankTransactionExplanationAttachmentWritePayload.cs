using System.Text.Json.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Writable attachment attributes for bank transaction explanation create and update requests.
/// </summary>
public sealed class BankTransactionExplanationAttachmentWritePayload
{
    /// <summary>
    /// Base64-encoded attachment data for a new upload (max 5MB).
    /// </summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }

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
