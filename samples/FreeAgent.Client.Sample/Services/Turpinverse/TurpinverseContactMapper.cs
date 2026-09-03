using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Sample.Services.Turpinverse;

internal static class TurpinverseContactMapper
{
    public static Contact ToFreeAgentContact(
        TurpinversePersona persona,
        IReadOnlyDictionary<string, string> organisationTradingNames)
    {
        var (firstName, lastName) = SplitName(persona.DisplayName);
        var organisationName = ResolveOrganisationName(persona, organisationTradingNames);

        return new Contact
        {
            FirstName = firstName,
            LastName = lastName,
            OrganisationName = organisationName,
            Email = persona.Email,
            PhoneNumber = persona.Phone,
            Status = ContactStatus.Active
        };
    }

    private static string? ResolveOrganisationName(
        TurpinversePersona persona,
        IReadOnlyDictionary<string, string> organisationTradingNames)
    {
        foreach (var organisationId in persona.OrganisationIds)
        {
            if (organisationTradingNames.TryGetValue(organisationId, out var tradingName))
            {
                return tradingName;
            }
        }

        return null;
    }

    private static (string FirstName, string LastName) SplitName(string displayName)
    {
        var parts = displayName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (parts[0], string.Empty),
            _ => (parts[0], parts[1])
        };
    }
}
