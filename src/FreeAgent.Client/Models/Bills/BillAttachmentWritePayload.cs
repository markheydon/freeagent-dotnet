using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Allowed MIME content types when attaching a file to a bill.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BillAttachmentContentType>))]
public enum BillAttachmentContentType
{
    /// <summary>PNG image.</summary>
    [JsonStringEnumMemberName("image/png")]
    Png,

    /// <summary>Legacy PNG image.</summary>
    [JsonStringEnumMemberName("image/x-png")]
    XPng,

    /// <summary>JPEG image.</summary>
    [JsonStringEnumMemberName("image/jpeg")]
    Jpeg,

    /// <summary>JPEG image (alternate MIME type).</summary>
    [JsonStringEnumMemberName("image/jpg")]
    Jpg,

    /// <summary>GIF image.</summary>
    [JsonStringEnumMemberName("image/gif")]
    Gif,

    /// <summary>PDF document.</summary>
    [JsonStringEnumMemberName("application/x-pdf")]
    Pdf,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}

/// <summary>
/// Writable attachment attributes for bill create and update requests.
/// </summary>
public sealed class BillAttachmentWritePayload
{
    /// <summary>
    /// Base64-encoded attachment data.
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
