using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Sample.Services;

namespace FreeAgent.Client.Tests.Sample;

public class ModelWireDiagnosticsTests
{
    [Fact]
    public void Build_MarksMatchingWireAndModelValues()
    {
        const string rawPayload = """
        {
          "contact": {
            "url": "https://api.freeagent.com/v2/contacts/1",
            "organisation_name": "Acme Ltd",
            "status": "Active"
          }
        }
        """;

        var model = new Contact
        {
            Url = "https://api.freeagent.com/v2/contacts/1",
            OrganisationName = "Acme Ltd",
            Status = ContactStatus.Active
        };

        var snapshot = ModelWireDiagnostics.Build(model, rawPayload, "contact");

        Assert.True(snapshot.MatchCount >= 2);
        Assert.Equal(0, snapshot.MismatchCount);
    }

    [Fact]
    public void Build_FlagsModelOnlyProperty()
    {
        const string rawPayload = """
        {
          "contact": {
            "organisation_name": "Acme Ltd"
          }
        }
        """;

        var model = new Contact
        {
            OrganisationName = "Acme Ltd",
            Status = ContactStatus.Active
        };

        var snapshot = ModelWireDiagnostics.Build(model, rawPayload, "contact");

        Assert.Contains(
            snapshot.MappingRows,
            row => row.PropertyName == nameof(Contact.Status) && row.Status == MappingCheckStatus.ModelOnly);
    }

    [Fact]
    public void TryGetArrayItem_ReturnsRequestedIndex()
    {
        const string rawPayload = """
        {
          "contacts": [
            { "organisation_name": "First" },
            { "organisation_name": "Second" }
          ]
        }
        """;

        var found = ModelWireDiagnostics.TryGetArrayItem(rawPayload, "contacts", 1, out var item);

        Assert.True(found);
        Assert.Equal("Second", item.GetProperty("organisation_name").GetString());
    }
}
