using System.Text.Json;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Tasks;
using FreeAgentProjectTaskStatus = FreeAgent.Client.Models.Tasks.ProjectTaskStatus;

namespace FreeAgent.Client.Tests.Models.Tasks;

public class ProjectTaskModelSerializationTests
{
    [Theory]
    [InlineData("\"Active\"", FreeAgentProjectTaskStatus.Active)]
    [InlineData("\"Completed\"", FreeAgentProjectTaskStatus.Completed)]
    [InlineData("\"Hidden\"", FreeAgentProjectTaskStatus.Hidden)]
    public void ProjectTaskStatus_DeserializesWireValues(string wireValue, FreeAgentProjectTaskStatus expected)
    {
        var task = JsonSerializer.Deserialize<ProjectTask>($$"""{ "status": {{wireValue}} }""");

        Assert.Equal(expected, task!.Status);
    }

    [Theory]
    [InlineData("\"hour\"", ProjectTaskBillingPeriod.Hour)]
    [InlineData("\"day\"", ProjectTaskBillingPeriod.Day)]
    public void ProjectTaskBillingPeriod_DeserializesWireValues(string wireValue, ProjectTaskBillingPeriod expected)
    {
        var task = JsonSerializer.Deserialize<ProjectTask>($$"""{ "billing_period": {{wireValue}} }""");

        Assert.Equal(expected, task!.BillingPeriod);
    }

    [Fact]
    public void Deserialize_ReadsStringDecimalsAndTimestamps()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/tasks/1",
          "project": "https://api.freeagent.com/v2/projects/1",
          "name": "Sample Task",
          "currency": "GBP",
          "is_billable": true,
          "billing_rate": "0.0",
          "billing_period": "hour",
          "status": "Active",
          "created_at": "2011-08-16T11:06:57Z",
          "updated_at": "2011-08-16T11:06:57Z",
          "is_deletable": false
        }
        """;

        var task = JsonSerializer.Deserialize<ProjectTask>(json);

        Assert.Equal("https://api.freeagent.com/v2/tasks/1", task!.Url);
        Assert.Null(task.Project);
        Assert.Equal(1, task.ProjectId);
        Assert.Equal("Sample Task", task.Name);
        Assert.Equal(CurrencyCode.GBP, task.Currency);
        Assert.True(task.IsBillable);
        Assert.Equal(0m, task.BillingRate);
        Assert.Equal(ProjectTaskBillingPeriod.Hour, task.BillingPeriod);
        Assert.Equal(FreeAgentProjectTaskStatus.Active, task.Status);
        Assert.Equal(new DateTimeOffset(2011, 8, 16, 11, 6, 57, TimeSpan.Zero), task.CreatedAt);
        Assert.False(task.IsDeletable);
    }

    [Fact]
    public void Deserialize_ProjectFromNestedObject()
    {
        const string json = """
        {
          "project": {
            "url": "https://api.freeagent.com/v2/projects/9",
            "name": "Nested Project"
          }
        }
        """;

        var task = JsonSerializer.Deserialize<ProjectTask>(json);

        Assert.Equal(9, task!.ProjectId);
        Assert.Equal("Nested Project", task.Project!.Name);
    }

    [Fact]
    public void Serialize_WritePayload_ExcludesReadOnlyFields()
    {
        var task = new ProjectTask
        {
            Url = "https://api.freeagent.com/v2/tasks/1",
            Name = "Writable Task",
            IsBillable = true,
            BillingRate = 50m,
            BillingPeriod = ProjectTaskBillingPeriod.Hour,
            Status = FreeAgentProjectTaskStatus.Active,
            Currency = CurrencyCode.GBP,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var payload = TaskWritePayload.FromProjectTask(task);
        var json = JsonSerializer.Serialize(payload);

        Assert.Contains("\"name\":\"Writable Task\"", json, StringComparison.Ordinal);
        Assert.Contains("\"is_billable\":true", json, StringComparison.Ordinal);
        Assert.Contains("\"billing_rate\":50", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"url\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"currency\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"created_at\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"project\"", json, StringComparison.Ordinal);
    }
}
