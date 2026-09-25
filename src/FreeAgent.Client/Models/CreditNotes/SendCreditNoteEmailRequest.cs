using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CreditNotes;

/// <summary>
/// Email attachment payload for sending a credit note by email.
/// </summary>
public sealed class CreditNoteEmailAttachment
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
/// Email attributes for sending a credit note.
/// </summary>
public sealed class CreditNoteEmailDetails
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
    public List<CreditNoteEmailAttachment>? Attachments { get; set; }
}

/// <summary>
/// Request payload for sending a credit note by email.
/// </summary>
public sealed class SendCreditNoteEmailRequest
{
    /// <summary>
    /// Email attributes for the credit note.
    /// </summary>
    [JsonPropertyName("email")]
    public CreditNoteEmailDetails? Email { get; set; }
}

/// <summary>
/// Request envelope for sending a credit note by email.
/// </summary>
internal sealed class SendCreditNoteEmailRequestEnvelope
{
    [JsonPropertyName("credit_note")]
    public SendCreditNoteEmailRequest? CreditNote { get; set; }
}
