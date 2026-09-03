using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Sample.Services.Turpinverse;

internal static class TurpinverseContactMapper
{
    /// <summary>
    /// Maps a Turpinverse persona to a FreeAgent contact.
    /// Contact address fields are populated from the primary organisation's registered office
    /// (FreeAgent uses these as the contact billing address on invoices).
    /// </summary>
    public static Contact ToFreeAgentContact(
        TurpinversePersona persona,
        IReadOnlyDictionary<string, TurpinverseOrganisation> organisationsById)
    {
        var (firstName, lastName) = SplitName(persona.DisplayName);
        var primaryOrganisation = ResolvePrimaryOrganisation(persona, organisationsById);

        return new Contact
        {
            FirstName = firstName,
            LastName = lastName,
            OrganisationName = primaryOrganisation?.TradingName,
            Email = persona.Email,
            PhoneNumber = persona.Phone,
            Status = ContactStatus.Active,
            Address1 = primaryOrganisation?.RegisteredOffice?.Address1,
            Address2 = primaryOrganisation?.RegisteredOffice?.Address2,
            Address3 = primaryOrganisation?.RegisteredOffice?.Address3,
            Town = primaryOrganisation?.RegisteredOffice?.Town,
            Region = primaryOrganisation?.RegisteredOffice?.Region,
            Postcode = primaryOrganisation?.RegisteredOffice?.Postcode,
            Country = primaryOrganisation?.RegisteredOffice?.Country
        };
    }

    private static TurpinverseOrganisation? ResolvePrimaryOrganisation(
        TurpinversePersona persona,
        IReadOnlyDictionary<string, TurpinverseOrganisation> organisationsById)
    {
        foreach (var organisationId in persona.OrganisationIds)
        {
            if (organisationsById.TryGetValue(organisationId, out var organisation))
            {
                return organisation;
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
