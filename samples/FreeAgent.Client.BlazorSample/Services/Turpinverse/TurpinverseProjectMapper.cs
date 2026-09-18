using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Maps Turpinverse project canon entries to FreeAgent project payloads.
/// </summary>
internal static class TurpinverseProjectMapper
{
    public const string ContractReferencePrefix = "turpinverse:";

    public static Project ToFreeAgentProject(TurpinverseProject project, string contactUrl)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactUrl);

        return new Project
        {
            Name = project.Title,
            Contact = contactUrl,
            Status = ProjectStatus.Active,
            ContractPoReference = BuildContractReference(project.Id),
            Currency = CurrencyCode.GBP,
            Budget = 0m,
            BudgetUnits = ProjectBudgetUnits.Hours,
            HoursPerDay = 8m,
            NormalBillingRate = 0m,
            BillingPeriod = ProjectBillingPeriod.Hour,
            UsesProjectInvoiceSequence = false,
            IncludeUnbilledTimeInProfitability = true,
            IsIr35 = false
        };
    }

    public static string BuildContractReference(string projectId) =>
        $"{ContractReferencePrefix}{projectId}";
}
