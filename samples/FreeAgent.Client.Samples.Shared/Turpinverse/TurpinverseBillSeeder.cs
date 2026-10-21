using FreeAgent.Client;
namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Seeds FreeAgent bills from upstream Turpinverse bill canon.
/// </summary>
public sealed class TurpinverseBillSeeder
{
    private readonly TurpinverseBillCatalog _billCatalog;
    private readonly TurpinverseContactCatalog _contactCatalog;
    private readonly TurpinverseProjectCatalog _projectCatalog;
    private readonly TurpinverseCompanyDates _companyDates;

    public TurpinverseBillSeeder(
        TurpinverseBillCatalog billCatalog,
        TurpinverseContactCatalog contactCatalog,
        TurpinverseProjectCatalog projectCatalog,
        TurpinverseCompanyDates companyDates)
    {
        _billCatalog = billCatalog ?? throw new ArgumentNullException(nameof(billCatalog));
        _contactCatalog = contactCatalog ?? throw new ArgumentNullException(nameof(contactCatalog));
        _projectCatalog = projectCatalog ?? throw new ArgumentNullException(nameof(projectCatalog));
        _companyDates = companyDates ?? throw new ArgumentNullException(nameof(companyDates));
    }

    public async Task<TurpinverseBillSeedResult> CreateBrazierLegalBillAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var bill = _billCatalog.BrazierLegalBill;
        return await UpsertBillAsync(client, bill, cancellationToken);
    }

    public async Task<TurpinverseBillBulkSeedResult> CreateAllBillsAsync(
        FreeAgentClient client,
        bool refreshCanon = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(refreshCanon, cancellationToken).ConfigureAwait(false);
        var existingBills = await LoadExistingBillsByReferenceAsync(client, cancellationToken);
        var created = new List<Bill>();
        var updated = new List<Bill>();
        var failures = new List<TurpinverseBillSeedFailure>();

        foreach (var bill in _billCatalog.Bills)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await UpsertBillAsync(
                    client,
                    bill,
                    cancellationToken,
                    existingBills);

                if (result.Action == BillSeedAction.Created)
                {
                    created.Add(result.Bill);
                }
                else
                {
                    updated.Add(result.Bill);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or FreeAgentApiException)
            {
                failures.Add(new TurpinverseBillSeedFailure(
                    bill.BillId,
                    bill.BillNumber,
                    ex.Message));
            }
        }

        return new TurpinverseBillBulkSeedResult(created, updated, failures);
    }

    private async Task EnsureCanonLoadedAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        await _billCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _contactCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _projectCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TurpinverseBillSeedResult> UpsertBillAsync(
        FreeAgentClient client,
        TurpinverseBill bill,
        CancellationToken cancellationToken,
        Dictionary<string, Bill>? existingBills = null)
    {
        var contactId = await ResolveOrganisationContactIdAsync(client, bill.SupplierAccountId, cancellationToken);
        var categoryNominalCode = await ResolveDefaultExpenseCategoryNominalCodeAsync(client, cancellationToken);
        var projectsByReference = await LoadExistingProjectsByContractReferenceAsync(client, cancellationToken);
        var projectId = ResolveBillProjectId(bill, projectsByReference);
        var lineProjectIds = ResolveLineProjectIds(bill, projectsByReference);
        var minimumDocumentDate = await _companyDates.GetMinimumDocumentDateAsync(client, cancellationToken);
        var desired = TurpinverseBillMapper.ToFreeAgentBill(
            bill,
            contactId,
            projectId,
            categoryNominalCode,
            minimumDocumentDate,
            lineProjectIds);
        var reference = TurpinverseBillMapper.BuildReference(bill.BillId);

        existingBills ??= await LoadExistingBillsByReferenceAsync(client, cancellationToken);
        if (!existingBills.TryGetValue(reference, out var existingMatch))
        {
            var created = await client.Bills.CreateBillAsync(desired, cancellationToken);
            existingBills[reference] = created;
            return new TurpinverseBillSeedResult(created, BillSeedAction.Created);
        }

        var billId = existingMatch.GetResourceId();
        var current = await client.Bills.GetBillAsync(billId, cancellationToken);
        MergeWritableFields(current, desired);
        var updated = await client.Bills.UpdateBillAsync(billId, current, cancellationToken: cancellationToken);
        existingBills[reference] = updated;
        return new TurpinverseBillSeedResult(updated, BillSeedAction.Updated);
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

    private static async Task<string> ResolveDefaultExpenseCategoryNominalCodeAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var sets = await client.Categories.ListAsync(cancellationToken: cancellationToken);
        var category = sets.AdminExpensesCategories
            .FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.NominalCode));

        if (category?.NominalCode is not { Length: > 0 } nominalCode)
        {
            throw new InvalidOperationException("No admin expenses category with a nominal code is available in FreeAgent.");
        }

        return nominalCode;
    }

    private static long? ResolveBillProjectId(
        TurpinverseBill bill,
        Dictionary<string, Project> projectsByReference)
    {
        var turpinverseProjectId = bill.Lines
            .Select(static line => line.ProjectId)
            .FirstOrDefault(static id => !string.IsNullOrWhiteSpace(id));

        if (string.IsNullOrWhiteSpace(turpinverseProjectId))
        {
            return null;
        }

        var contractReference = TurpinverseProjectMapper.BuildContractReference(turpinverseProjectId);
        if (!projectsByReference.TryGetValue(contractReference, out var project))
        {
            throw new InvalidOperationException(
                $"No FreeAgent project exists for Turpinverse project '{turpinverseProjectId}'. Seed projects first from Project CRUD.");
        }

        return project.ResourceId;
    }

    private static Dictionary<string, long?> ResolveLineProjectIds(
        TurpinverseBill bill,
        Dictionary<string, Project> projectsByReference)
    {
        var lineProjectIds = new Dictionary<string, long?>(StringComparer.Ordinal);

        foreach (var turpinverseProjectId in bill.Lines
                     .Select(static line => line.ProjectId)
                     .Where(static id => !string.IsNullOrWhiteSpace(id))
                     .Distinct(StringComparer.Ordinal))
        {
            var contractReference = TurpinverseProjectMapper.BuildContractReference(turpinverseProjectId!);
            if (!projectsByReference.TryGetValue(contractReference, out var project))
            {
                throw new InvalidOperationException(
                    $"No FreeAgent project exists for Turpinverse project '{turpinverseProjectId}'. Seed projects first from Project CRUD.");
            }

            lineProjectIds[turpinverseProjectId!] = project.ResourceId;
        }

        return lineProjectIds;
    }

    private static async Task<Dictionary<string, Bill>> LoadExistingBillsByReferenceAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var billsByReference = new Dictionary<string, Bill>(StringComparer.Ordinal);

        await foreach (var bill in client.Bills.ListAutoPagingAsync(
                           view: BillViews.All,
                           cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(bill.Reference)
                || !bill.Reference.StartsWith(TurpinverseBillMapper.ReferencePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            billsByReference[bill.Reference] = bill;
        }

        return billsByReference;
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

    private static void MergeWritableFields(Bill current, Bill desired)
    {
        current.ContactId = desired.ContactId;
        current.ProjectId = desired.ProjectId;
        current.Reference = desired.Reference;
        current.DatedOn = desired.DatedOn;
        current.DueOn = desired.DueOn;
        current.Currency = desired.Currency;
        current.Comments = desired.Comments;
        if (desired.BillItems is not null)
        {
            MergeBillItems(current.BillItems, desired.BillItems);
            current.BillItems = desired.BillItems;
        }
    }

    private static void MergeBillItems(List<BillItem>? currentItems, List<BillItem> desiredItems)
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
                desiredItems[index].Url = currentItems[index].Url;
            }
        }

        if (currentItems.Count <= desiredItems.Count)
        {
            return;
        }

        for (var index = desiredItems.Count; index < currentItems.Count; index++)
        {
            if (currentItems[index].ItemId is not null)
            {
                desiredItems.Add(new BillItem { Url = currentItems[index].Url, Destroy = 1 });
            }
        }
    }
}

public enum BillSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseBillSeedResult(Bill Bill, BillSeedAction Action);

public sealed record TurpinverseBillSeedFailure(string BillId, string BillNumber, string Message);

public sealed record TurpinverseBillBulkSeedResult(
    IReadOnlyList<Bill> Created,
    IReadOnlyList<Bill> Updated,
    IReadOnlyList<TurpinverseBillSeedFailure> Failures);
