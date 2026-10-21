using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Payroll liability payment due for a tax year.
/// </summary>
public class PayrollPayment
{
    /// <summary>
    /// Date on which the payment is due.
    /// </summary>
    [JsonPropertyName("due_on")]
    public DateOnly? DueOn { get; set; }

    /// <summary>
    /// Amount due on <see cref="DueOn"/>.
    /// </summary>
    [JsonPropertyName("amount_due")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? AmountDue { get; set; }

    /// <summary>
    /// Payment status when payments are due.
    /// </summary>
    [JsonPropertyName("status")]
    public PayrollPaymentStatus? Status { get; set; }
}
