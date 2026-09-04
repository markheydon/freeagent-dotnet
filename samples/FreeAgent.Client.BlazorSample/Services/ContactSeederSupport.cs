using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Outcome of a sample contact seed operation.
/// </summary>
public enum ContactSeedAction
{
    Created,
    Updated
}

/// <summary>
/// Shared create-or-update helpers for sample contact seeders.
/// </summary>
internal static class ContactSeederSupport
{
    public static async Task<Dictionary<string, Contact>> LoadExistingContactsByEmailAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken)
    {
        var contactsByEmail = new Dictionary<string, Contact>(StringComparer.OrdinalIgnoreCase);

        await foreach (var contact in client.Contacts.GetAllContactsAsync(
                           view: ContactViews.All,
                           cancellationToken: cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                continue;
            }

            if (contactsByEmail.TryGetValue(contact.Email, out var existing))
            {
                if (CompareContactIds(contact, existing) > 0)
                {
                    contactsByEmail[contact.Email] = contact;
                }

                continue;
            }

            contactsByEmail[contact.Email] = contact;
        }

        return contactsByEmail;
    }

    public static async Task<(Contact Contact, ContactSeedAction Action)> UpsertByEmailAsync(
        FreeAgentClient client,
        string email,
        Contact desired,
        CancellationToken cancellationToken,
        Dictionary<string, Contact>? existingContacts = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        existingContacts ??= await LoadExistingContactsByEmailAsync(client, cancellationToken);
        if (!existingContacts.TryGetValue(email, out var existingMatch))
        {
            var created = await client.Contacts.CreateContactAsync(desired, cancellationToken);
            return (created, ContactSeedAction.Created);
        }

        var contactId = ContactUrlParser.ParseId(existingMatch.Url)
            ?? throw new InvalidOperationException($"Could not parse contact ID from URL '{existingMatch.Url}'.");

        var current = await client.Contacts.GetContactAsync(contactId, cancellationToken);
        MergeWritableFields(current, desired);
        var updated = await client.Contacts.UpdateContactAsync(contactId, current, cancellationToken);
        return (updated, ContactSeedAction.Updated);
    }

    internal static void MergeWritableFields(Contact target, Contact source)
    {
        target.FirstName = source.FirstName;
        target.LastName = source.LastName;
        target.OrganisationName = source.OrganisationName;
        target.Email = source.Email;
        target.BillingEmail = source.BillingEmail;
        target.PhoneNumber = source.PhoneNumber;
        target.Mobile = source.Mobile;
        MergeAddressFieldsIfPresent(target, source);
        target.UsesContactInvoiceSequence = source.UsesContactInvoiceSequence;
        target.ContactNameOnInvoices = source.ContactNameOnInvoices;
        target.ChargeSalesTax = source.ChargeSalesTax;
        target.SalesTaxRegistrationNumber = source.SalesTaxRegistrationNumber;
        target.Status = source.Status;
        target.DefaultPaymentTermsInDays = source.DefaultPaymentTermsInDays;
        target.Locale = source.Locale;
        target.IsCisSubcontractor = source.IsCisSubcontractor;
        target.CisDeductionRate = source.CisDeductionRate;
        target.UniqueTaxReference = source.UniqueTaxReference;
        target.SubcontractorVerificationNumber = source.SubcontractorVerificationNumber;
    }

    private static void MergeAddressFieldsIfPresent(Contact target, Contact source)
    {
        if (!HasAddress(source))
        {
            return;
        }

        target.Address1 = source.Address1;
        target.Address2 = source.Address2;
        target.Address3 = source.Address3;
        target.Town = source.Town;
        target.Region = source.Region;
        target.Postcode = source.Postcode;
        target.Country = source.Country;
    }

    private static bool HasAddress(Contact contact) =>
        !string.IsNullOrWhiteSpace(contact.Address1)
        || !string.IsNullOrWhiteSpace(contact.Address2)
        || !string.IsNullOrWhiteSpace(contact.Address3)
        || !string.IsNullOrWhiteSpace(contact.Town)
        || !string.IsNullOrWhiteSpace(contact.Region)
        || !string.IsNullOrWhiteSpace(contact.Postcode)
        || !string.IsNullOrWhiteSpace(contact.Country);

    private static int CompareContactIds(Contact left, Contact right)
    {
        var leftId = ContactUrlParser.ParseId(left.Url) ?? 0;
        var rightId = ContactUrlParser.ParseId(right.Url) ?? 0;
        return leftId.CompareTo(rightId);
    }
}
