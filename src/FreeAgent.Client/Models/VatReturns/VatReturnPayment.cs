using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.VatReturns;

/// <summary>
/// A payment or refund line on a VAT return.
/// </summary>
public sealed class VatReturnPayment
{
    /// <summary>
    /// Human-readable payment label.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Date the payment is due.
    /// </summary>
    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    /// <summary>
    /// Amount due (may be negative for refunds).
    /// </summary>
    [JsonPropertyName("amount_due")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? AmountDue { get; set; }

    /// <summary>
    /// Payment status when provided by the API.
    /// </summary>
    [JsonPropertyName("status")]
    public VatReturnPaymentStatus? Status { get; set; }
}
