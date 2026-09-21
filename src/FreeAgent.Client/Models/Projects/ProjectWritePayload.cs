using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Projects;

/// <summary>
/// Writable project attributes for create and update requests.
/// </summary>
internal sealed class ProjectWritePayload
{
    [JsonPropertyName("contact")]
    public ContactReference? Contact { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("status")]
    public ProjectStatus? Status { get; set; }

    [JsonPropertyName("contract_po_reference")]
    public string? ContractPoReference { get; set; }

    [JsonPropertyName("uses_project_invoice_sequence")]
    public bool? UsesProjectInvoiceSequence { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    [JsonPropertyName("budget")]
    public decimal? Budget { get; set; }

    [JsonPropertyName("budget_units")]
    public ProjectBudgetUnits? BudgetUnits { get; set; }

    [JsonPropertyName("hours_per_day")]
    public decimal? HoursPerDay { get; set; }

    [JsonPropertyName("normal_billing_rate")]
    public decimal? NormalBillingRate { get; set; }

    [JsonPropertyName("billing_period")]
    public ProjectBillingPeriod? BillingPeriod { get; set; }

    [JsonPropertyName("is_ir35")]
    public bool? IsIr35 { get; set; }

    [JsonPropertyName("starts_on")]
    public DateOnly? StartsOn { get; set; }

    [JsonPropertyName("ends_on")]
    public DateOnly? EndsOn { get; set; }

    [JsonPropertyName("include_unbilled_time_in_profitability")]
    public bool? IncludeUnbilledTimeInProfitability { get; set; }

    public static ProjectWritePayload FromProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        return new ProjectWritePayload
        {
            Contact = project.BillingContact
                ?? (project.ContactLink?.Uri is string uri ? ContactReference.Parse(uri) : null),
            Name = project.Name,
            Status = project.Status,
            ContractPoReference = project.ContractPoReference,
            UsesProjectInvoiceSequence = project.UsesProjectInvoiceSequence,
            Currency = project.Currency,
            Budget = project.Budget,
            BudgetUnits = project.BudgetUnits,
            HoursPerDay = project.HoursPerDay,
            NormalBillingRate = project.NormalBillingRate,
            BillingPeriod = project.BillingPeriod,
            IsIr35 = project.IsIr35,
            StartsOn = project.StartsOn,
            EndsOn = project.EndsOn,
            IncludeUnbilledTimeInProfitability = project.IncludeUnbilledTimeInProfitability
        };
    }
}
