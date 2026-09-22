using System.Text.Json;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Tasks;

namespace FreeAgent.Client.Tests.Models.Tasks;

public class TaskModelSerializationTests
{
    [Theory]
    [InlineData("\"Active\"", FreeAgent.Client.Models.Tasks.TaskStatus.Active)]
    [InlineData("\"Completed\"", FreeAgent.Client.Models.Tasks.TaskStatus.Completed)]
    [InlineData("\"Hidden\"", FreeAgent.Client.Models.Tasks.TaskStatus.Hidden)]
    public void TaskStatus_DeserializesWireValues(string wireValue, FreeAgent.Client.Models.Tasks.TaskStatus expected)
    {
        var task = JsonSerializer.Deserialize<FreeAgent.Client.Models.Tasks.Task>($$"""{ "status": {{wireValue}} }""");

        Assert.Equal(expected, task!.Status);
    }

    [Theory]
    [InlineData("\"hour\"", TaskBillingPeriod.Hour)]
    [InlineData("\"day\"", TaskBillingPeriod.Day)]
    public void TaskBillingPeriod_DeserializesWireValues(string wireValue, TaskBillingPeriod expected)
    {
        var task = JsonSerializer.Deserialize<FreeAgent.Client.Models.Tasks.Task>($$"""{ "billing_period": {{wireValue}} }""");

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

        var task = JsonSerializer.Deserialize<FreeAgent.Client.Models.Tasks.Task>(json);

        Assert.Equal("https://api.freeagent.com/v2/tasks/1", task!.Url);
        Assert.Null(task.Project);
        Assert.Equal(1, task.ProjectId);
        Assert.Equal("Sample Task", task.Name);
        Assert.Equal(CurrencyCode.GBP, task.Currency);
        Assert.True(task.IsBillable);
        Assert.Equal(0m, task.BillingRate);
        Assert.Equal(TaskBillingPeriod.Hour, task.BillingPeriod);
        Assert.Equal(FreeAgent.Client.Models.Tasks.TaskStatus.Active, task.Status);
        Assert.Equal(new DateTimeOffset(2011, 8, 16, 11, 6, 57, TimeSpan.Zero), task.CreatedAt);
        Assert.False(task.IsDeletable);
    }

    [Fact]
    public void Serialize_WritePayload_ExcludesReadOnlyFields()
    {
        var task = new FreeAgent.Client.Models.Tasks.Task
        {
            Url = "https://api.freeagent.com/v2/tasks/1",
            Name = "Writable Task",
            IsBillable = true,
            BillingRate = 50m,
            BillingPeriod = TaskBillingPeriod.Hour,
            Status = FreeAgent.Client.Models.Tasks.TaskStatus.Active,
            Currency = CurrencyCode.GBP,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var payload = TaskWritePayload.FromTask(task);
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
