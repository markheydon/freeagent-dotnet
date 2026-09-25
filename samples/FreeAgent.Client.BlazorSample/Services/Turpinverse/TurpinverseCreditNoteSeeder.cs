using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.CreditNotes;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent credit notes from upstream Turpinverse credit note canon.
/// </summary>
public sealed class TurpinverseCreditNoteSeeder
{
    private readonly TurpinverseCreditNoteCatalog _creditNoteCatalog;
    private readonly TurpinverseContactCatalog _contactCatalog;

    public TurpinverseCreditNoteSeeder(
        TurpinverseCreditNoteCatalog creditNoteCatalog,
        TurpinverseContactCatalog contactCatalog)
    {
        _creditNoteCatalog = creditNoteCatalog ?? throw new ArgumentNullException(nameof(creditNoteCatalog));
        _contactCatalog = contactCatalog ?? throw new ArgumentNullException(nameof(contactCatalog));
    }

    public async Task<TurpinverseCreditNoteSeedResult> CreateHighwayCommissionDraftAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var creditNote = _creditNoteCatalog.HighwayCommissionCreditNote;
        return await UpsertCreditNoteAsync(client, creditNote, cancellationToken);
    }

    public async Task<TurpinverseCreditNoteBulkSeedResult> CreateAllCreditNotesAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await EnsureCanonLoadedAsync(forceRefresh: true, cancellationToken).ConfigureAwait(false);
        var existingCreditNotes = await LoadExistingCreditNotesByReferenceAsync(client, cancellationToken);
        var created = new List<CreditNote>();
        var updated = new List<CreditNote>();
        var failures = new List<TurpinverseCreditNoteSeedFailure>();

        foreach (var creditNote in _creditNoteCatalog.CreditNotes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var result = await UpsertCreditNoteAsync(
                    client,
                    creditNote,
                    cancellationToken,
                    existingCreditNotes);

                if (result.Action == CreditNoteSeedAction.Created)
                {
                    created.Add(result.CreditNote);
                }
                else
                {
                    updated.Add(result.CreditNote);
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or FreeAgentApiException)
            {
                failures.Add(new TurpinverseCreditNoteSeedFailure(
                    creditNote.CreditNoteId,
                    creditNote.CreditNoteNumber,
                    ex.Message));
            }
        }

        return new TurpinverseCreditNoteBulkSeedResult(created, updated, failures);
    }

    private async Task EnsureCanonLoadedAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        await _creditNoteCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
        await _contactCatalog.EnsureLoadedAsync(forceRefresh, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TurpinverseCreditNoteSeedResult> UpsertCreditNoteAsync(
        FreeAgentClient client,
        TurpinverseCreditNote creditNote,
        CancellationToken cancellationToken,
        Dictionary<string, CreditNote>? existingCreditNotes = null)
    {
        var contactId = await ResolveOrganisationContactIdAsync(client, creditNote.AccountId, cancellationToken);
        var desired = TurpinverseCreditNoteMapper.ToFreeAgentCreditNote(creditNote, contactId);
        var reference = TurpinverseCreditNoteMapper.BuildReference(creditNote.CreditNoteId);

        existingCreditNotes ??= await LoadExistingCreditNotesByReferenceAsync(client, cancellationToken);
        if (!existingCreditNotes.TryGetValue(reference, out var existingMatch))
        {
            var created = await client.CreditNotes.CreateCreditNoteAsync(desired, cancellationToken);
            existingCreditNotes[reference] = created;
            return new TurpinverseCreditNoteSeedResult(created, CreditNoteSeedAction.Created);
        }

        var creditNoteId = existingMatch.GetResourceId();
        var current = await client.CreditNotes.GetCreditNoteAsync(creditNoteId, cancellationToken);
        MergeWritableFields(current, desired);
        var updated = await client.CreditNotes.UpdateCreditNoteAsync(creditNoteId, current, cancellationToken: cancellationToken);
        existingCreditNotes[reference] = updated;
        return new TurpinverseCreditNoteSeedResult(updated, CreditNoteSeedAction.Updated);
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

    private static async Task<Dictionary<string, CreditNote>> LoadExistingCreditNotesByReferenceAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var creditNotesByReference = new Dictionary<string, CreditNote>(StringComparer.Ordinal);

        await foreach (var creditNote in client.CreditNotes.ListAutoPagingAsync(
                           view: CreditNoteViews.All,
                           cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(creditNote.Reference)
                || !creditNote.Reference.StartsWith(TurpinverseCreditNoteMapper.ReferencePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            creditNotesByReference[creditNote.Reference] = creditNote;
        }

        return creditNotesByReference;
    }

    private static void MergeWritableFields(CreditNote current, CreditNote desired)
    {
        current.ContactId = desired.ContactId;
        current.Reference = desired.Reference;
        current.PoReference = desired.PoReference;
        current.DatedOn = desired.DatedOn;
        current.DueOn = desired.DueOn;
        current.PaymentTermsInDays = desired.PaymentTermsInDays;
        current.Currency = desired.Currency;
        current.Comments = desired.Comments;
        if (desired.CreditNoteItems is not null)
        {
            MergeCreditNoteItems(current.CreditNoteItems, desired.CreditNoteItems);
            current.CreditNoteItems = desired.CreditNoteItems;
        }
    }

    private static void MergeCreditNoteItems(List<CreditNoteItem>? currentItems, List<CreditNoteItem> desiredItems)
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
                desiredItems.Add(new CreditNoteItem { ItemId = itemId, Destroy = 1 });
            }
        }
    }
}

public enum CreditNoteSeedAction
{
    Created,
    Updated
}

public sealed record TurpinverseCreditNoteSeedResult(CreditNote CreditNote, CreditNoteSeedAction Action);

public sealed record TurpinverseCreditNoteSeedFailure(string CreditNoteId, string CreditNoteNumber, string Message);

public sealed record TurpinverseCreditNoteBulkSeedResult(
    IReadOnlyList<CreditNote> Created,
    IReadOnlyList<CreditNote> Updated,
    IReadOnlyList<TurpinverseCreditNoteSeedFailure> Failures);
