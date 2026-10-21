using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// API response envelope for a single payroll period with payslips.
/// </summary>
public class PayrollPeriodResponse
{
    /// <summary>
    /// Payroll period payload.
    /// </summary>
    [JsonPropertyName("period")]
    public PayrollPeriod? Period { get; set; }
}
