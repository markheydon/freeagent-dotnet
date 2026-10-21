using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Payroll;

/// <summary>
/// Payroll periods and liability payments for a tax year.
/// </summary>
public class PayrollTaxYear
{
    /// <summary>
    /// Payroll periods in the tax year.
    /// </summary>
    [JsonPropertyName("periods")]
    public IReadOnlyList<PayrollPeriod>? Periods { get; set; }

    /// <summary>
    /// Liability payments due for the tax year.
    /// </summary>
    [JsonPropertyName("payments")]
    public IReadOnlyList<PayrollPayment>? Payments { get; set; }
}
