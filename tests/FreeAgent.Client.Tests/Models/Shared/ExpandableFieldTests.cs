using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Projects;

namespace FreeAgent.Client.Tests.Models.Shared;

public class ExpandableFieldTests
{
    [Fact]
    public void Deserialize_StringUri_SetsUriOnly()
    {
        var field = JsonSerializer.Deserialize<ExpandableField<Contact>>("\"https://api.freeagent.com/v2/contacts/5\"");

        Assert.Equal("https://api.freeagent.com/v2/contacts/5", field!.Uri);
        Assert.Null(field.Value);
        Assert.False(field.IsExpanded);
        Assert.Equal(5, field.Id);
    }

    [Fact]
    public void Deserialize_NestedObject_PreservesExpandedValue()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/contacts/8",
          "organisation_name": "Expanded Org"
        }
        """;

        var field = JsonSerializer.Deserialize<ExpandableField<Contact>>(json);

        Assert.Equal("https://api.freeagent.com/v2/contacts/8", field!.Uri);
        Assert.Equal("Expanded Org", field.Value!.OrganisationName);
        Assert.True(field.IsExpanded);
        Assert.Equal(8, field.Id);
    }

    [Fact]
    public void Project_ExposesNestedContactOnPublicSurface()
    {
        const string json = """
        {
          "contact": {
            "url": "https://api.freeagent.com/v2/contacts/2",
            "organisation_name": "Public Org"
          }
        }
        """;

        var project = JsonSerializer.Deserialize<Project>(json);

        Assert.Equal("Public Org", project!.Contact!.OrganisationName);
    }
}
