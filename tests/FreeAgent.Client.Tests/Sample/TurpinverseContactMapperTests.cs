#if NET10_0
using FreeAgent.Client.Samples.Shared.Turpinverse;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.Tests.Sample;

public class TurpinverseContactMapperTests
{
    [Fact]
    public void ToFreeAgentContact_MapsOrganisationTradingNameAndRegisteredOffice()
    {
        var organisation = new TurpinverseOrganisation
        {
            Id = "turpin-enterprises",
            TradingName = "Turpin Enterprises",
            PrimaryContactId = "dick-turpin",
            RegisteredOffice = new TurpinverseAddress
            {
                Address1 = "Suite 12",
                Town = "Hempstead",
                Postcode = "CM23 4TA",
                Country = "United Kingdom"
            }
        };

        var personas = new Dictionary<string, TurpinversePersona>(StringComparer.Ordinal)
        {
            ["dick-turpin"] = new()
            {
                Id = "dick-turpin",
                DisplayName = "Richard Turpin",
                Email = "richard.turpin@turpinverse.uk",
                Phone = "01707-555-0101"
            }
        };

        var contact = TurpinverseContactMapper.ToFreeAgentContact(organisation, personas);

        Assert.Equal("Richard", contact.FirstName);
        Assert.Equal("Turpin", contact.LastName);
        Assert.Equal("Turpin Enterprises", contact.OrganisationName);
        Assert.Equal("richard.turpin@turpinverse.uk", contact.Email);
        Assert.Equal("01707-555-0101", contact.PhoneNumber);
        Assert.Equal("Suite 12", contact.Address1);
        Assert.Equal("Hempstead", contact.Town);
        Assert.Equal(ContactStatus.Active, contact.Status);
    }

    [Fact]
    public void ResolvePrimaryContact_UsesMemberPersonaIdsWhenPrimaryContactIdMissing()
    {
        var organisation = new TurpinverseOrganisation
        {
            Id = "example-org",
            MemberPersonaIds = ["backup-persona"]
        };

        var personas = new Dictionary<string, TurpinversePersona>(StringComparer.Ordinal)
        {
            ["backup-persona"] = new()
            {
                Id = "backup-persona",
                DisplayName = "Backup Persona",
                Email = "backup@turpinverse.uk"
            }
        };

        var primaryContact = TurpinverseContactMapper.ResolvePrimaryContact(organisation, personas);

        Assert.NotNull(primaryContact);
        Assert.Equal("backup@turpinverse.uk", primaryContact.Email);
    }

    [Fact]
    public void ResolveUpsertEmail_ThrowsWhenPrimaryContactHasNoEmail()
    {
        var organisation = new TurpinverseOrganisation
        {
            Id = "missing-email-org",
            PrimaryContactId = "no-email"
        };

        var personas = new Dictionary<string, TurpinversePersona>(StringComparer.Ordinal)
        {
            ["no-email"] = new()
            {
                Id = "no-email",
                DisplayName = "No Email Persona",
                Email = string.Empty
            }
        };

        var exception = Assert.Throws<InvalidOperationException>(() =>
            TurpinverseContactMapper.ResolveUpsertEmail(organisation, personas));

        Assert.Contains("missing-email-org", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ResolveDisplayName_UsesFallbackPrimaryContactWhenPrimaryContactIdMissing()
    {
        var organisation = new TurpinverseOrganisation
        {
            Id = "example-org",
            TradingName = "Example Org",
            MemberPersonaIds = ["backup-persona"]
        };

        var personas = new Dictionary<string, TurpinversePersona>(StringComparer.Ordinal)
        {
            ["backup-persona"] = new()
            {
                Id = "backup-persona",
                DisplayName = "Backup Persona",
                Email = "backup@turpinverse.uk"
            }
        };

        var displayName = TurpinverseContactMapper.ResolveDisplayName(organisation, personas);

        Assert.Equal("Example Org (Backup Persona)", displayName);
    }
}

#endif
