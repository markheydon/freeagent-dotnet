using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent projects from upstream Turpinverse project canon.
/// </summary>
public sealed class TurpinverseProjectSeeder
{
    private readonly TurpinverseProjectCatalog _projectCatalog;
    private readonly TurpinverseContactCatalog _contactCatalog;

    public TurpinverseProjectSeeder(
        TurpinverseProjectCatalog projectCatalog,
        TurpinverseContactCatalog contactCatalog)
    {
        _projectCatalog = projectCatalog ?? throw new ArgumentNullException(nameof(projectCatalog));
        _contactCatalog = contactCatalog ?? throw new ArgumentNullException(nameof(contactCatalog));
    }

    public async Task<TurpinverseProjectSeedResult> CreateBlackBessRouteOptimiserAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var project = _projectCatalog.BlackBessRouteOptimiser;
        return await UpsertProjectAsync(client, project, cancellationToken);
    }

    public async Task<TurpinverseProjectBulkSeedResult> CreateAllProjectsAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var existingProjects = await LoadExistingProjectsByContractReferenceAsync(client, cancellationToken);
        var created = new List<Project>();
        var updated = new List<Project>();
        var failures = new List<TurpinverseProjectSeedFailure>();

        foreach (var project in _projectCatalog.Projects)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await UpsertProjectAsync(
                    client,
                    project,
                    cancellationToken,
                    existingProjects);

                if (result.Action == ProjectSeedAction.Created)
                {
                    created.Add(result.Project);
                }
                else
                {
                    updated.Add(result.Project);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or FreeAgentApiException)
            {
                failures.Add(new TurpinverseProjectSeedFailure(project.Id, project.Title, ex.Message));
            }
        }

        return new TurpinverseProjectBulkSeedResult(created, updated, failures);
    }

    private async Task EnsureCanonLoadedAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        await _projectCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _contactCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TurpinverseProjectSeedResult> UpsertProjectAsync(
        FreeAgentClient client,
        TurpinverseProject project,
        CancellationToken cancellationToken,
        Dictionary<string, Project>? existingProjects = null)
    {
        var contactId = await ResolveOrganisationContactIdAsync(client, project.OrganisationId, cancellationToken);
        var desired = TurpinverseProjectMapper.ToFreeAgentProject(project, contactId);
        var contractReference = TurpinverseProjectMapper.BuildContractReference(project.Id);

        existingProjects ??= await LoadExistingProjectsByContractReferenceAsync(client, cancellationToken);
        if (!existingProjects.TryGetValue(contractReference, out var existingMatch))
        {
            var created = await client.Projects.CreateProjectAsync(desired, cancellationToken);
            existingProjects[contractReference] = created;
            return new TurpinverseProjectSeedResult(created, ProjectSeedAction.Created);
        }

        var projectId = existingMatch.GetResourceId();

        var current = await client.Projects.GetProjectAsync(projectId, cancellationToken);
        MergeWritableFields(current, desired);
        var updated = await client.Projects.UpdateProjectAsync(
            projectId,
            current,
            cancellationToken: cancellationToken);
        existingProjects[contractReference] = updated;
        return new TurpinverseProjectSeedResult(updated, ProjectSeedAction.Updated);
    }

    private async Task<long> ResolveOrganisationContactIdAsync(
        FreeAgentClient client,
        string organisationId,
        CancellationToken cancellationToken)
    {
        var organisation = _contactCatalog.Organisations
            .FirstOrDefault(org => org.Id == organisationId)
            ?? throw new InvalidOperationException($"Turpinverse organisation '{organisationId}' was not found in canon.");

        var upsertEmail = TurpinverseContactMapper.ResolveUpsertEmail(organisation, _contactCatalog.PersonasById);
        var contactsByEmail = await ContactSeederSupport.LoadExistingContactsByEmailAsync(client, cancellationToken);
        if (!contactsByEmail.TryGetValue(upsertEmail, out var contact))
        {
            throw new InvalidOperationException(
                $"No FreeAgent contact exists for organisation '{organisation.TradingName}'. Seed contacts first from Contact CRUD.");
        }

        return contact.ResourceId;
    }

    private static async Task<Dictionary<string, Project>> LoadExistingProjectsByContractReferenceAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var projectsByReference = new Dictionary<string, Project>(StringComparer.Ordinal);

        await foreach (var project in client.Projects.ListAutoPagingAsync(cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(project.ContractPoReference)
                || !project.ContractPoReference.StartsWith(TurpinverseProjectMapper.ContractReferencePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            projectsByReference[project.ContractPoReference] = project;
        }

        return projectsByReference;
    }

    private static void MergeWritableFields(Project current, Project desired)
    {
        current.Name = desired.Name;
        current.ContactId = desired.ContactId;
        current.Status = desired.Status;
        current.ContractPoReference = desired.ContractPoReference;
        current.Currency = desired.Currency;
        current.Budget = desired.Budget;
        current.BudgetUnits = desired.BudgetUnits;
        current.HoursPerDay = desired.HoursPerDay;
        current.NormalBillingRate = desired.NormalBillingRate;
        current.BillingPeriod = desired.BillingPeriod;
        current.UsesProjectInvoiceSequence = desired.UsesProjectInvoiceSequence;
        current.IncludeUnbilledTimeInProfitability = desired.IncludeUnbilledTimeInProfitability;
        current.IsIr35 = desired.IsIr35;
    }
}

public enum ProjectSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseProjectSeedResult(Project Project, ProjectSeedAction Action);

public sealed record TurpinverseProjectSeedFailure(string ProjectId, string Title, string Message);

public sealed record TurpinverseProjectBulkSeedResult(
    IReadOnlyList<Project> Created,
    IReadOnlyList<Project> Updated,
    IReadOnlyList<TurpinverseProjectSeedFailure> Failures);
