using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Users;

/// <summary>
/// Read-only payroll profile subsection returned on user GET when payroll is configured for the current tax year.
/// </summary>
public class CurrentPayrollProfile
{
    /// <summary>
    /// Total pay during previous employment.
    /// </summary>
    [JsonPropertyName("total_pay_in_previous_employment")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalPayInPreviousEmployment { get; set; }

    /// <summary>
    /// Total tax paid during previous employment.
    /// </summary>
    [JsonPropertyName("total_tax_in_previous_employment")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? TotalTaxInPreviousEmployment { get; set; }
}
