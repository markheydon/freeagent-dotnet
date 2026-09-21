using System.Text.Json;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Projects;

public class ProjectModelSerializationTests
{
    [Theory]
    [InlineData("\"Active\"", ProjectStatus.Active)]
    [InlineData("\"Completed\"", ProjectStatus.Completed)]
    [InlineData("\"Cancelled\"", ProjectStatus.Cancelled)]
    [InlineData("\"Hidden\"", ProjectStatus.Hidden)]
    public void ProjectStatus_DeserializesWireValues(string wireValue, ProjectStatus expected)
    {
        var project = JsonSerializer.Deserialize<Project>($$"""{ "status": {{wireValue}} }""");

        Assert.Equal(expected, project!.Status);
    }

    [Theory]
    [InlineData("\"Hours\"", ProjectBudgetUnits.Hours)]
    [InlineData("\"Days\"", ProjectBudgetUnits.Days)]
    [InlineData("\"Monetary\"", ProjectBudgetUnits.Monetary)]
    public void ProjectBudgetUnits_DeserializesWireValues(string wireValue, ProjectBudgetUnits expected)
    {
        var project = JsonSerializer.Deserialize<Project>($$"""{ "budget_units": {{wireValue}} }""");

        Assert.Equal(expected, project!.BudgetUnits);
    }

    [Theory]
    [InlineData("\"hour\"", ProjectBillingPeriod.Hour)]
    [InlineData("\"day\"", ProjectBillingPeriod.Day)]
    public void ProjectBillingPeriod_DeserializesWireValues(string wireValue, ProjectBillingPeriod expected)
    {
        var project = JsonSerializer.Deserialize<Project>($$"""{ "billing_period": {{wireValue}} }""");

        Assert.Equal(expected, project!.BillingPeriod);
    }

    [Fact]
    public void Deserialize_ReadsStringDecimalsAndDates()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/projects/1",
          "contact": "https://api.freeagent.com/v2/contacts/1",
          "contact_name": "Acme Trading",
          "name": "Test Project",
          "status": "Active",
          "currency": "GBP",
          "budget": 0,
          "budget_units": "Hours",
          "normal_billing_rate": "0.0",
          "hours_per_day": "8.0",
          "billing_period": "hour",
          "starts_on": "2026-01-15",
          "ends_on": "2026-12-31",
          "created_at": "2011-09-14T16:05:57Z",
          "updated_at": "2011-09-14T16:05:57Z",
          "is_deletable": false
        }
        """;

        var project = JsonSerializer.Deserialize<Project>(json);

        Assert.Equal("https://api.freeagent.com/v2/projects/1", project!.Url);
        Assert.Null(project.Contact);
        Assert.Equal(1, project.ContactId);
        Assert.Equal("Acme Trading", project.ContactName);
        Assert.Equal("Test Project", project.Name);
        Assert.Equal(CurrencyCode.GBP, project.Currency);
        Assert.Equal(0m, project.Budget);
        Assert.Equal(0m, project.NormalBillingRate);
        Assert.Equal(8m, project.HoursPerDay);
        Assert.Equal(new DateOnly(2026, 1, 15), project.StartsOn);
        Assert.Equal(new DateOnly(2026, 12, 31), project.EndsOn);
        Assert.Equal(new DateTimeOffset(2011, 9, 14, 16, 5, 57, TimeSpan.Zero), project.CreatedAt);
        Assert.False(project.IsDeletable);
    }

    [Fact]
    public void Deserialize_ContactFromNestedObject()
    {
        const string json = """
        {
          "contact": {
            "url": "https://api.freeagent.com/v2/contacts/9",
            "organisation_name": "Nested Org"
          }
        }
        """;

        var project = JsonSerializer.Deserialize<Project>(json);

        Assert.Equal(9, project!.ContactId);
        Assert.Equal("Nested Org", project.Contact!.OrganisationName);
    }

    [Fact]
    public void WritePayload_UsesBillingContact()
    {
        var payload = ProjectWritePayload.FromProject(new Project
        {
            BillingContact = ContactReference.Parse("https://api.freeagent.com/v2/contacts/3"),
            Name = "Example"
        });

        Assert.Equal("https://api.freeagent.com/v2/contacts/3", payload.Contact!.Value.Uri);
    }
}
