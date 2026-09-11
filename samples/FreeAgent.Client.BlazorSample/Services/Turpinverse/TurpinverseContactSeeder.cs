using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent contacts from upstream Turpinverse organisation canon.
/// Each organisation becomes one B2B contact with the primary contact persona on the record.
/// </summary>
public sealed class TurpinverseContactSeeder
{
    private readonly TurpinverseContactCatalog _catalog;

    public TurpinverseContactSeeder(TurpinverseContactCatalog catalog)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
    }

    public async Task<TurpinverseSeedResult> CreateTurpinEnterprisesAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await _catalog.EnsureLoadedAsync(cancellationToken).ConfigureAwait(false);
        var organisation = _catalog.TurpinEnterprises;
        return await UpsertOrganisationContactAsync(client, organisation, cancellationToken);
    }

    public async Task<TurpinverseBulkSeedResult> CreateAllOrganisationContactsAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        await _catalog.EnsureLoadedAsync(cancellationToken).ConfigureAwait(false);
        var existingContacts = await ContactSeederSupport.LoadExistingContactsByEmailAsync(client, cancellationToken);
        var created = new List<Contact>();
        var updated = new List<Contact>();

        foreach (var organisation in _catalog.Organisations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await UpsertOrganisationContactAsync(
                client,
                organisation,
                cancellationToken,
                existingContacts);

            if (result.Action == ContactSeedAction.Created)
            {
                created.Add(result.Contact);
            }
            else
            {
                updated.Add(result.Contact);
            }
        }

        return new TurpinverseBulkSeedResult(created, updated);
    }

    private async Task<TurpinverseSeedResult> UpsertOrganisationContactAsync(
        FreeAgentClient client,
        TurpinverseOrganisation organisation,
        CancellationToken cancellationToken,
        Dictionary<string, Contact>? existingContacts = null)
    {
        var desired = TurpinverseContactMapper.ToFreeAgentContact(organisation, _catalog.PersonasById);
        var upsertEmail = TurpinverseContactMapper.ResolveUpsertEmail(organisation, _catalog.PersonasById);
        var (contact, action) = await ContactSeederSupport.UpsertByEmailAsync(
            client,
            upsertEmail,
            desired,
            cancellationToken,
            existingContacts);

        var primaryContact = _catalog.PersonasById.GetValueOrDefault(organisation.PrimaryContactId ?? string.Empty);
        var displayName = primaryContact is null
            ? organisation.TradingName
            : $"{organisation.TradingName} ({primaryContact.DisplayName})";

        return new TurpinverseSeedResult(contact, displayName, action);
    }
}

public sealed record TurpinverseSeedResult(Contact Contact, string DisplayName, ContactSeedAction Action);

public sealed record TurpinverseBulkSeedResult(
    IReadOnlyList<Contact> Created,
    IReadOnlyList<Contact> Updated);
