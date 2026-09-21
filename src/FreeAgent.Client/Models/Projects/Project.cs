using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Represents a FreeAgent project.
/// </summary>
public class Project : IFreeAgentResource
{
    /// <summary>
    /// Project resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Wire representation of the billing contact link.
    /// </summary>
    [JsonPropertyName("contact")]
    [JsonInclude]
    internal ExpandableField<Contact>? ContactLink { get; set; }

    /// <summary>
    /// Billing contact when returned nested on the wire or hydrated via <see cref="ProjectGetOptions.IncludeBillingContact"/>.
    /// </summary>
    [JsonIgnore]
    public Contact? Contact => ContactLink?.Value;

    /// <summary>
    /// Billing contact identifier parsed from the project response.
    /// </summary>
    [JsonIgnore]
    public long? ContactId => ContactLink?.Id;

    /// <summary>
    /// Billing contact to assign on create or update requests.
    /// </summary>
    [JsonIgnore]
    public ContactReference? BillingContact { get; set; }

    /// <summary>
    /// Contact display name when the full contact is not included in the response.
    /// </summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>
    /// Attaches a hydrated billing contact to this project.
    /// </summary>
    /// <param name="contact">Billing contact details.</param>
    internal void AttachContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        ContactLink = new ExpandableField<Contact>(contact.Url, contact);
    }

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
