using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Payroll period metadata and optional payslips for the period.
/// </summary>
public class PayrollPeriod
{
    /// <summary>
    /// Period resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Pay period index within the tax year (0–11).
    /// </summary>
    [JsonPropertyName("period")]
    public int? PeriodNumber { get; set; }

    /// <summary>
    /// Pay frequency for the period.
    /// </summary>
    [JsonPropertyName("frequency")]
    public string? Frequency { get; set; }

    /// <summary>
    /// Pay date for the period.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Filing status for the period.
    /// </summary>
    [JsonPropertyName("status")]
    public PayrollPeriodFilingStatus? Status { get; set; }

    /// <summary>
    /// Whether employment allowance was claimed in this period.
    /// </summary>
    [JsonPropertyName("employment_allowance_claimed")]
    public bool? EmploymentAllowanceClaimed { get; set; }

    /// <summary>
    /// Employment allowance amount claimed in this period.
    /// </summary>
    [JsonPropertyName("employment_allowance_amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EmploymentAllowanceAmount { get; set; }

    /// <summary>
    /// CIS liability withheld this period.
    /// </summary>
    [JsonPropertyName("construction_industry_scheme_deduction")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ConstructionIndustrySchemeDeduction { get; set; }

    /// <summary>
    /// Payslips when returned on a period detail response.
    /// </summary>
    [JsonPropertyName("payslips")]
    public IReadOnlyList<PayrollPayslip>? Payslips { get; set; }

    /// <summary>
    /// Created timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Updated timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
