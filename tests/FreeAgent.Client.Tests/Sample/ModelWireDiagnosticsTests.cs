using System.Text.Json;
using FreeAgent.Client.BlazorSample.Services;
using FreeAgent.Client.Tests.Infrastructure.Serialization;
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

    [Fact]
    public void BuildStringArrayItem_MarksMatchingWireAndModelValues()
    {
        using var document = JsonDocument.Parse("\"Software Development\"");

        var snapshot = ModelWireDiagnostics.BuildStringArrayItem("Software Development", document.RootElement);

        Assert.Equal(1, snapshot.MatchCount);
        Assert.Equal(0, snapshot.MismatchCount);
    }

    [Fact]
    public void BuildStringArrayItem_FlagsMismatch()
    {
        using var document = JsonDocument.Parse("\"Software Development\"");

        var snapshot = ModelWireDiagnostics.BuildStringArrayItem("Design", document.RootElement);

        Assert.Equal(0, snapshot.MatchCount);
        Assert.Equal(1, snapshot.MismatchCount);
    }

    [Fact]
    public void BuildStringArrayItem_WhenWireMissing_MarksNotReturned()
    {
        var snapshot = ModelWireDiagnostics.BuildStringArrayItem("Software Development", default);

        Assert.Equal(0, snapshot.MatchCount);
        Assert.Equal(0, snapshot.MismatchCount);
        Assert.Equal(1, snapshot.NotReturnedCount);
    }

    [Fact]
    public void BuildStringArrayItem_WhenWireElementIsObject_FlagsMismatch()
    {
        using var document = JsonDocument.Parse("{\"name\":\"Software Development\"}");

        var snapshot = ModelWireDiagnostics.BuildStringArrayItem("Software Development", document.RootElement);

        Assert.Equal(0, snapshot.MatchCount);
        Assert.Equal(1, snapshot.MismatchCount);
    }

    [Fact]
    public void BuildStringArrayItemFromWirePayload_UsesRequestedArrayItem()
    {
        const string rawPayload = """
        {
          "business_categories": ["Software Development", "Design"]
        }
        """;

        var snapshot = ModelWireDiagnostics.BuildStringArrayItemFromWirePayload(
            "Design",
            rawPayload,
            "business_categories",
            1);

        Assert.Equal(1, snapshot.MatchCount);
        Assert.Equal(0, snapshot.MismatchCount);
    }

    [Fact]
    public void BuildStringArrayItemFromWirePayload_DegradesWhenWirePayloadUnavailable()
    {
        var snapshot = ModelWireDiagnostics.BuildStringArrayItemFromWirePayload(
            "Design",
            "Failed to capture raw payload: timeout",
            "business_categories",
            0);

        Assert.Equal(0, snapshot.MatchCount);
        Assert.Equal(0, snapshot.MismatchCount);
        Assert.Equal(1, snapshot.NotReturnedCount);
    }

    [Fact]
    public void Build_Timeslip_IncludesTaskLinkFacadesAndMatchesTaskUri()
    {
        const string rawPayload = """
        {
          "timeslip": {
            "url": "https://api.freeagent.com/v2/timeslips/25",
            "task": "https://api.freeagent.com/v2/tasks/3",
            "project": "https://api.freeagent.com/v2/projects/4",
            "user": "https://api.freeagent.com/v2/users/2",
            "dated_on": "2011-08-15",
            "hours": "12.0"
          }
        }
        """;

        const string timeslipJson = """
        {
          "url": "https://api.freeagent.com/v2/timeslips/25",
          "task": "https://api.freeagent.com/v2/tasks/3",
          "project": "https://api.freeagent.com/v2/projects/4",
          "user": "https://api.freeagent.com/v2/users/2",
          "dated_on": "2011-08-15",
          "hours": "12.0"
        }
        """;

        var timeslip = JsonSerializer.Deserialize<Timeslip>(timeslipJson, FreeAgentJsonTestSupport.Options)!;

        var snapshot = ModelWireDiagnostics.Build(timeslip, rawPayload, "timeslip");

        Assert.Contains(
            snapshot.FieldRows,
            row => row.PropertyName == nameof(Timeslip.ProjectTaskId) && row.JsonFieldName == "task");
        Assert.Contains(
            snapshot.MappingRows,
            row => row.PropertyName == nameof(Timeslip.ProjectTaskId)
                   && row.JsonFieldName == "task"
                   && row.Status == MappingCheckStatus.Match);
        Assert.Equal(3, timeslip.ProjectTaskId);
    }
}
