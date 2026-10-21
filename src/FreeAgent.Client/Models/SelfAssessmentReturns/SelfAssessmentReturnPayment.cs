using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// A payment line on an self assessment return.
/// </summary>
public sealed class SelfAssessmentReturnPayment
{
    /// <summary>
    /// Human-readable payment label.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Payment due date.
    /// </summary>
    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    /// <summary>
    /// Amount due on <see cref="DueOn"/> (may be negative for balancing payments).
    /// </summary>
    [JsonPropertyName("amount_due")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? AmountDue { get; set; }

    /// <summary>
    /// Payment status when the amount due is positive.
    /// </summary>
    [JsonPropertyName("status")]
    public SelfAssessmentReturnPaymentStatus? Status { get; set; }
}
