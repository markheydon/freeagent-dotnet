using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CisSettings;

/// <summary>
/// CIS deductions suffered before the company started using FreeAgent.
/// </summary>
public sealed class CisPriorDeductions
{
    /// <summary>
    /// Start of the tax year the opening balance relates to.
    /// </summary>
    [JsonPropertyName("start_date")]
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Opening balance to two decimal places.
    /// </summary>
    [JsonPropertyName("initial_balance")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? InitialBalance { get; set; }
}
