using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Estimates;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent estimates from upstream Turpinverse quote canon.
/// </summary>
public sealed class TurpinverseQuoteSeeder
{
    private readonly TurpinverseQuoteCatalog _quoteCatalog;
    private readonly TurpinverseContactCatalog _contactCatalog;
    private readonly TurpinverseProjectCatalog _projectCatalog;

    public TurpinverseQuoteSeeder(
        TurpinverseQuoteCatalog quoteCatalog,
        TurpinverseContactCatalog contactCatalog,
        TurpinverseProjectCatalog projectCatalog)
    {
        _quoteCatalog = quoteCatalog ?? throw new ArgumentNullException(nameof(quoteCatalog));
        _contactCatalog = contactCatalog ?? throw new ArgumentNullException(nameof(contactCatalog));
        _projectCatalog = projectCatalog ?? throw new ArgumentNullException(nameof(projectCatalog));
    }

    public async Task<TurpinverseQuoteSeedResult> CreateHighwayCommissionDraftAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var quote = _quoteCatalog.HighwayCommissionDraft;
        return await UpsertQuoteAsync(client, quote, cancellationToken);
    }

    public async Task<TurpinverseQuoteBulkSeedResult> CreateAllQuotesAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var existingEstimates = await LoadExistingEstimatesByReferenceAsync(client, cancellationToken);
        var created = new List<Estimate>();
        var updated = new List<Estimate>();
        var failures = new List<TurpinverseQuoteSeedFailure>();

        foreach (var quote in _quoteCatalog.Quotes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await UpsertQuoteAsync(
                    client,
                    quote,
                    cancellationToken,
                    existingEstimates);

                if (result.Action == QuoteSeedAction.Created)
                {
                    created.Add(result.Estimate);
                }
                else
                {
                    updated.Add(result.Estimate);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or FreeAgentApiException)
            {
                failures.Add(new TurpinverseQuoteSeedFailure(
                    quote.QuoteId,
                    quote.QuoteNumber,
                    ex.Message));
            }
        }

        return new TurpinverseQuoteBulkSeedResult(created, updated, failures);
    }

    private async Task EnsureCanonLoadedAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        await _quoteCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _contactCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _projectCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TurpinverseQuoteSeedResult> UpsertQuoteAsync(
        FreeAgentClient client,
        TurpinverseQuote quote,
        CancellationToken cancellationToken,
        Dictionary<string, Estimate>? existingEstimates = null)
    {
        var contact = await ResolveOrganisationContactReferenceAsync(client, quote.AccountId, cancellationToken);
        var project = await ResolveProjectReferenceAsync(client, quote, cancellationToken);
        var desired = TurpinverseQuoteMapper.ToFreeAgentEstimate(quote, contact, project);
        var reference = TurpinverseQuoteMapper.BuildReference(quote.QuoteId);

        existingEstimates ??= await LoadExistingEstimatesByReferenceAsync(client, cancellationToken);
        if (!existingEstimates.TryGetValue(reference, out var existingMatch))
        {
            var created = await client.Estimates.CreateEstimateAsync(desired, cancellationToken);
            created = await ApplyCanonStatusAsync(client, created, quote, cancellationToken);
            existingEstimates[reference] = created;
            return new TurpinverseQuoteSeedResult(created, QuoteSeedAction.Created);
        }

        var estimateId = existingMatch.GetResourceId();
        var current = await client.Estimates.GetEstimateAsync(estimateId, cancellationToken);
        MergeWritableFields(current, desired);
        var updated = await client.Estimates.UpdateEstimateAsync(estimateId, current, cancellationToken);
        updated = await ApplyCanonStatusAsync(client, updated, quote, cancellationToken);
        existingEstimates[reference] = updated;
        return new TurpinverseQuoteSeedResult(updated, QuoteSeedAction.Updated);
    }

    private async Task<ContactReference> ResolveOrganisationContactReferenceAsync(
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

        return ContactReference.Parse(contact.Url);
    }

    private static async Task<ProjectReference?> ResolveProjectReferenceAsync(
        FreeAgentClient client,
        TurpinverseQuote quote,
        CancellationToken cancellationToken)
    {
        var projectId = quote.Lines
            .Select(static line => line.ProjectId)
            .FirstOrDefault(static id => !string.IsNullOrWhiteSpace(id));

        if (string.IsNullOrWhiteSpace(projectId))
        {
            return null;
        }

        var contractReference = TurpinverseProjectMapper.BuildContractReference(projectId);
        var projectsByReference = await LoadExistingProjectsByContractReferenceAsync(client, cancellationToken);
        if (!projectsByReference.TryGetValue(contractReference, out var project))
        {
            throw new InvalidOperationException(
                $"No FreeAgent project exists for Turpinverse project '{projectId}'. Seed projects first from Project CRUD.");
        }

        return ProjectReference.Parse(project.Url);
    }

    private static async Task<Dictionary<string, Estimate>> LoadExistingEstimatesByReferenceAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var estimatesByReference = new Dictionary<string, Estimate>(StringComparer.Ordinal);

        await foreach (var estimate in client.Estimates.ListAutoPagingAsync(
                           view: EstimateViews.All,
                           cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(estimate.Reference)
                || !estimate.Reference.StartsWith(TurpinverseQuoteMapper.ReferencePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            estimatesByReference[estimate.Reference] = estimate;
        }

        return estimatesByReference;
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

    private static async Task<Estimate> ApplyCanonStatusAsync(
        FreeAgentClient client,
        Estimate estimate,
        TurpinverseQuote canon,
        CancellationToken cancellationToken)
    {
        var estimateId = estimate.GetResourceId();
        var targetStatus = TurpinverseQuoteMapper.MapStatus(canon.Status);

        if (estimate.Status == targetStatus)
        {
            return estimate;
        }

        if (targetStatus == EstimateStatus.Draft)
        {
            return estimate.Status == EstimateStatus.Draft
                ? estimate
                : await client.Estimates.MarkEstimateAsDraftAsync(estimateId, cancellationToken);
        }

        if (estimate.Status == EstimateStatus.Draft)
        {
            estimate = await client.Estimates.MarkEstimateAsSentAsync(estimateId, cancellationToken);
        }

        return targetStatus switch
        {
            EstimateStatus.Approved => await client.Estimates.MarkEstimateAsApprovedAsync(estimateId, cancellationToken),
            EstimateStatus.Rejected => await client.Estimates.MarkEstimateAsRejectedAsync(estimateId, cancellationToken),
            _ => estimate
        };
    }

    private static void MergeWritableFields(Estimate current, Estimate desired)
    {
        current.BillingContact = desired.BillingContact;
        current.LinkedProject = desired.LinkedProject;
        current.OmitBillingContactFromWrite = false;
        current.OmitProjectFromWrite = desired.LinkedProject is null;
        current.OmitEstimateItemsFromWrite = false;
        current.EstimateType = desired.EstimateType;
        current.Reference = desired.Reference;
        current.DatedOn = desired.DatedOn;
        current.Currency = desired.Currency;
        current.Notes = desired.Notes;
        if (desired.EstimateItems is not null)
        {
            MergeEstimateItems(current.EstimateItems, desired.EstimateItems);
            current.EstimateItems = desired.EstimateItems;
        }
    }

    private static void MergeEstimateItems(List<EstimateItem>? currentItems, List<EstimateItem> desiredItems)
    {
        if (currentItems is null)
        {
            return;
        }

        var sharedCount = Math.Min(currentItems.Count, desiredItems.Count);
        for (var index = 0; index < sharedCount; index++)
        {
            if (desiredItems[index].ItemId is null && currentItems[index].ItemId is long itemId)
            {
                desiredItems[index].ItemId = itemId;
            }
        }

        if (currentItems.Count <= desiredItems.Count)
        {
            return;
        }

        for (var index = desiredItems.Count; index < currentItems.Count; index++)
        {
            if (currentItems[index].ItemId is long itemId)
            {
                desiredItems.Add(new EstimateItem { ItemId = itemId, Destroy = 1 });
            }
        }
    }
}

public enum QuoteSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseQuoteSeedResult(Estimate Estimate, QuoteSeedAction Action);

public sealed record TurpinverseQuoteSeedFailure(string QuoteId, string QuoteNumber, string Message);

public sealed record TurpinverseQuoteBulkSeedResult(
    IReadOnlyList<Estimate> Created,
    IReadOnlyList<Estimate> Updated,
    IReadOnlyList<TurpinverseQuoteSeedFailure> Failures);
