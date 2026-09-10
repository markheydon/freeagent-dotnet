using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.BlazorSample.Services.Turpinverse;

internal static class TurpinverseContactMapper
{
    /// <summary>
    /// Maps a Turpinverse organisation to a FreeAgent B2B contact.
    /// The organisation trading name is the company; the primary contact persona supplies
    /// first name, last name, email, and phone. Address fields come from the registered office
    /// (FreeAgent uses these as the contact billing address on invoices).
    /// </summary>
    public static Contact ToFreeAgentContact(
        TurpinverseOrganisation organisation,
        IReadOnlyDictionary<string, TurpinversePersona> personasById)
    {
        var primaryContact = ResolvePrimaryContact(organisation, personasById);
        var (firstName, lastName) = primaryContact is null
            ? (string.Empty, string.Empty)
            : SplitName(primaryContact.DisplayName);

        return new Contact
        {
            FirstName = firstName,
            LastName = lastName,
            OrganisationName = organisation.TradingName,
            Email = primaryContact?.Email,
            PhoneNumber = primaryContact?.Phone,
            Status = ContactStatus.Active,
            Address1 = organisation.RegisteredOffice?.Address1,
            Address2 = organisation.RegisteredOffice?.Address2,
            Address3 = organisation.RegisteredOffice?.Address3,
            Town = organisation.RegisteredOffice?.Town,
            Region = organisation.RegisteredOffice?.Region,
            Postcode = organisation.RegisteredOffice?.Postcode,
            Country = organisation.RegisteredOffice?.Country
        };
    }

    public static string ResolveUpsertEmail(
        TurpinverseOrganisation organisation,
        IReadOnlyDictionary<string, TurpinversePersona> personasById)
    {
        var primaryContact = ResolvePrimaryContact(organisation, personasById);
        if (primaryContact is null || string.IsNullOrWhiteSpace(primaryContact.Email))
        {
            throw new InvalidOperationException(
                $"Organisation '{organisation.Id}' has no primary contact with an email address.");
        }

        return primaryContact.Email;
    }

    private static TurpinversePersona? ResolvePrimaryContact(
        TurpinverseOrganisation organisation,
        IReadOnlyDictionary<string, TurpinversePersona> personasById)
    {
        if (!string.IsNullOrWhiteSpace(organisation.PrimaryContactId)
            && personasById.TryGetValue(organisation.PrimaryContactId, out var primaryContact))
        {
            return primaryContact;
        }

        foreach (var personaId in organisation.MemberPersonaIds)
        {
            if (personasById.TryGetValue(personaId, out var member))
            {
                return member;
            }
        }

        return null;
    }

    private static (string FirstName, string LastName) SplitName(string displayName)
    {
        var trimmed = displayName.Trim();
        var lastSpace = trimmed.LastIndexOf(' ');
        if (lastSpace <= 0)
        {
            return trimmed.Length == 0
                ? (string.Empty, string.Empty)
                : (trimmed, string.Empty);
        }

        return (trimmed[..lastSpace], trimmed[(lastSpace + 1)..]);
    }
}
