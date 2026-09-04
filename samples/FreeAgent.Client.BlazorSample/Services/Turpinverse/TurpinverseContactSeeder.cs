using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

/// <summary>
/// Seeds FreeAgent contacts from bundled Turpinverse persona canon.
/// </summary>
public sealed class TurpinverseContactSeeder
{
    private readonly TurpinverseContactCatalog _catalog;

    public TurpinverseContactSeeder(TurpinverseContactCatalog catalog)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
    }

    public async Task<TurpinverseSeedResult> CreateRichardTurpinAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var richard = _catalog.RichardTurpin;
        var desired = TurpinverseContactMapper.ToFreeAgentContact(richard, _catalog.OrganisationsById);
        var (contact, action) = await ContactSeederSupport.UpsertByEmailAsync(
            client,
            richard.Email,
            desired,
            cancellationToken);

        return new TurpinverseSeedResult(contact, richard.DisplayName, action);
    }

    public async Task<TurpinverseBulkSeedResult> CreateAllContactsAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var existingContacts = await ContactSeederSupport.LoadExistingContactsByEmailAsync(client, cancellationToken);
        var created = new List<Contact>();
        var updated = new List<Contact>();

        foreach (var persona in _catalog.Personas)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var desired = TurpinverseContactMapper.ToFreeAgentContact(persona, _catalog.OrganisationsById);

            if (existingContacts.TryGetValue(persona.Email, out var existingMatch))
            {
                var contactId = ContactUrlParser.ParseId(existingMatch.Url)
                    ?? throw new InvalidOperationException($"Could not parse contact ID from URL '{existingMatch.Url}'.");

                var current = await client.Contacts.GetContactAsync(contactId, cancellationToken);
                ContactSeederSupport.MergeWritableFields(current, desired);
                var updatedContact = await client.Contacts.UpdateContactAsync(contactId, current, cancellationToken);
                updated.Add(updatedContact);
                existingContacts[persona.Email] = updatedContact;
                continue;
            }

            var createdContact = await client.Contacts.CreateContactAsync(desired, cancellationToken);
            created.Add(createdContact);

            if (!string.IsNullOrWhiteSpace(createdContact.Email))
            {
                existingContacts[createdContact.Email] = createdContact;
            }
        }

        return new TurpinverseBulkSeedResult(created, updated);
    }
}

public sealed record TurpinverseSeedResult(Contact Contact, string DisplayName, ContactSeedAction Action);

public sealed record TurpinverseBulkSeedResult(
    IReadOnlyList<Contact> Created,
    IReadOnlyList<Contact> Updated);
