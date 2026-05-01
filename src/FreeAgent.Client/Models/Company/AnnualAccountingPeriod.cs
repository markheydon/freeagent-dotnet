using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents a single annual accounting period for a company.
/// </summary>
public class AnnualAccountingPeriod
{
    /// <summary>
    /// Start date of the accounting period.
    /// </summary>
    [JsonPropertyName("starts_on")]
    public DateOnly StartsOn { get; set; }

    /// <summary>
    /// End date of the accounting period.
    /// </summary>
    [JsonPropertyName("ends_on")]
    public DateOnly EndsOn { get; set; }
}
