using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Represents a FreeAgent project.
/// </summary>
public class Project
{
    /// <summary>
    /// Project resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Contact to bill for the project.
    /// </summary>
    [JsonPropertyName("contact")]
    [JsonConverter(typeof(ContactUriJsonConverter))]
    public string? Contact { get; set; }

    /// <summary>
    /// Contact display name when the list response is not nested.
    /// </summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>
    /// Free-text project name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Project status.
    /// </summary>
    [JsonPropertyName("status")]
    public ProjectStatus? Status { get; set; }

    /// <summary>
    /// Contract or purchase order reference.
    /// </summary>
    [JsonPropertyName("contract_po_reference")]
    public string? ContractPoReference { get; set; }

    /// <summary>
    /// Whether invoice numbering uses a project-level sequence.
    /// </summary>
    [JsonPropertyName("uses_project_invoice_sequence")]
    public bool? UsesProjectInvoiceSequence { get; set; }

    /// <summary>
    /// Project currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Project budget amount.
    /// </summary>
    [JsonPropertyName("budget")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Budget { get; set; }

    /// <summary>
    /// Budget unit for the project.
    /// </summary>
    [JsonPropertyName("budget_units")]
    public ProjectBudgetUnits? BudgetUnits { get; set; }

    /// <summary>
    /// Hours per day for day-based budgets.
    /// </summary>
    [JsonPropertyName("hours_per_day")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? HoursPerDay { get; set; }

    /// <summary>
    /// Normal billing rate for the project.
    /// </summary>
    [JsonPropertyName("normal_billing_rate")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? NormalBillingRate { get; set; }

    /// <summary>
    /// Unit for <see cref="NormalBillingRate"/>.
    /// </summary>
    [JsonPropertyName("billing_period")]
    public ProjectBillingPeriod? BillingPeriod { get; set; }

    /// <summary>
    /// Whether the project comes under IR35 as de facto employment.
    /// </summary>
    [JsonPropertyName("is_ir35")]
    public bool? IsIr35 { get; set; }

    /// <summary>
    /// Project start date.
    /// </summary>
    [JsonPropertyName("starts_on")]
    public DateOnly? StartsOn { get; set; }

    /// <summary>
    /// Project end date.
    /// </summary>
    [JsonPropertyName("ends_on")]
    public DateOnly? EndsOn { get; set; }

    /// <summary>
    /// Whether unbilled time is included in profitability reporting.
    /// </summary>
    [JsonPropertyName("include_unbilled_time_in_profitability")]
    public bool? IncludeUnbilledTimeInProfitability { get; set; }

    /// <summary>
    /// Whether the project can be deleted. Returned on single-project GET only.
    /// </summary>
    [JsonPropertyName("is_deletable")]
    public bool? IsDeletable { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
