using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Email attachment payload for sending an estimate by email.
/// </summary>
public sealed class EstimateEmailAttachment
{
    /// <summary>
    /// MIME content type of the attachment.
    /// </summary>
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

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
}

/// <summary>
/// Email attributes for sending an estimate.
/// </summary>
public sealed class EstimateEmailDetails
{
    /// <summary>
    /// Recipient email address.
    /// </summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>
    /// Sender email address.
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>
    /// Email subject line.
    /// </summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    /// <summary>
    /// Email body text.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>
    /// Whether to send a copy to the sender.
    /// </summary>
    [JsonPropertyName("email_to_sender")]
    public bool? EmailToSender { get; set; }

    /// <summary>
    /// When <see langword="true"/>, use an existing email template instead of explicit email fields.
    /// </summary>
    [JsonPropertyName("use_template")]
    public bool? UseTemplate { get; set; }

    /// <summary>
    /// Additional email attachments.
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<EstimateEmailAttachment>? Attachments { get; set; }
}

/// <summary>
/// Request payload for sending an estimate by email.
/// </summary>
public sealed class SendEstimateEmailRequest
{
    /// <summary>
    /// Email attributes for the estimate.
    /// </summary>
    [JsonPropertyName("email")]
    public EstimateEmailDetails? Email { get; set; }
}

/// <summary>
/// Request envelope for sending an estimate by email.
/// </summary>
internal sealed class SendEstimateEmailRequestEnvelope
{
    [JsonPropertyName("estimate")]
    public SendEstimateEmailRequest? Estimate { get; set; }
}
