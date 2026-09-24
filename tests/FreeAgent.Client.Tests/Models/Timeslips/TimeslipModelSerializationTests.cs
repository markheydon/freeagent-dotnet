using System.Globalization;
using System.Text.Json;
using FreeAgent.Client.Models.Timeslips;

namespace FreeAgent.Client.Tests.Models.Timeslips;

public class TimeslipModelSerializationTests
{
    [Fact]
    public void Deserialize_ReadsStringHoursAndDates()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/timeslips/25",
          "dated_on": "2011-08-15",
          "hours": "12.0",
          "comment": "Planning",
          "created_at": "2011-08-16T13:32:00Z",
          "updated_at": "2011-08-16T13:32:00Z"
        }
        """;

        var timeslip = JsonSerializer.Deserialize<Timeslip>(json);

        Assert.Equal(25, timeslip!.ResourceId);
        Assert.Equal(new DateOnly(2011, 8, 15), timeslip.DatedOn);
        Assert.Equal(12.0m, timeslip.Hours);
        Assert.Equal("Planning", timeslip.Comment);
        Assert.Equal(new DateTimeOffset(2011, 8, 16, 13, 32, 0, TimeSpan.Zero), timeslip.CreatedAt);
    }

    [Fact]
    public void Deserialize_ReadsTimerObject()
    {
        const string json = """
        {
          "timer": {
            "running": true,
            "start_from": "2011-08-16T01:32:00Z"
          }
        }
        """;

        var timeslip = JsonSerializer.Deserialize<Timeslip>(json);

        Assert.True(timeslip!.Timer!.Running);
        Assert.Equal(
            DateTimeOffset.Parse("2011-08-16T01:32:00Z", CultureInfo.InvariantCulture),
            timeslip.Timer.StartFrom);
    }

    [Fact]
    public void Deserialize_LinkUriFields_ExposeIds()
    {
        const string json = """
        {
          "task": "https://api.freeagent.com/v2/tasks/3",
          "project": "https://api.freeagent.com/v2/projects/4",
          "user": "https://api.freeagent.com/v2/users/2",
          "billed_on_invoice": "https://api.freeagent.com/v2/invoices/7"
        }
        """;

        var timeslip = JsonSerializer.Deserialize<Timeslip>(json);

        Assert.Equal(3, timeslip!.TaskId);
        Assert.Null(timeslip.Task);
        Assert.Equal(4, timeslip.ProjectId);
        Assert.Null(timeslip.Project);
        Assert.Equal(2, timeslip.UserId);
        Assert.Null(timeslip.User);
        Assert.Equal(7, timeslip.BilledOnInvoiceId);
    }

    [Fact]
    public void Deserialize_NestedLinkFields_ExposeExpandedResources()
    {
        const string json = """
        {
          "task": {
            "url": "https://api.freeagent.com/v2/tasks/3",
            "name": "Nested Task"
          },
          "project": {
            "url": "https://api.freeagent.com/v2/projects/4",
            "name": "Nested Project"
          },
          "user": {
            "url": "https://api.freeagent.com/v2/users/2",
            "first_name": "Ada",
            "last_name": "Lovelace"
          }
        }
        """;

        var timeslip = JsonSerializer.Deserialize<Timeslip>(json);

        Assert.Equal("Nested Task", timeslip!.Task!.Name);
        Assert.Equal("Nested Project", timeslip.Project!.Name);
        Assert.Equal("Ada", timeslip.User!.FirstName);
    }

    [Fact]
    public void WritePayload_FallsBackToLinkUrisWhenLinkedPropertiesMissing()
    {
        var timeslip = JsonSerializer.Deserialize<Timeslip>("""
            {
              "url": "https://api.freeagent.com/v2/timeslips/25",
              "task": "https://api.freeagent.com/v2/tasks/1",
              "project": "https://api.freeagent.com/v2/projects/2",
              "user": "https://api.freeagent.com/v2/users/3",
              "dated_on": "2011-08-15",
              "hours": "1.5"
            }
            """)!;

        var payload = TimeslipWritePayload.FromTimeslip(timeslip);

        Assert.Equal("https://api.freeagent.com/v2/tasks/1", payload.Task!.Value.Uri);
        Assert.Equal("https://api.freeagent.com/v2/projects/2", payload.Project!.Value.Uri);
        Assert.Equal("https://api.freeagent.com/v2/users/3", payload.User!.Value.Uri);
        Assert.Equal(1.5m, payload.Hours);
    }

    [Fact]
    public void WritePayload_SerializesWithoutReadOnlyInvoiceLink()
    {
        var timeslip = JsonSerializer.Deserialize<Timeslip>("""
            {
              "url": "https://api.freeagent.com/v2/timeslips/25",
              "task": "https://api.freeagent.com/v2/tasks/1",
              "project": "https://api.freeagent.com/v2/projects/2",
              "user": "https://api.freeagent.com/v2/users/3",
              "billed_on_invoice": "https://api.freeagent.com/v2/invoices/7",
              "hours": "2.0"
            }
            """)!;

        var json = JsonSerializer.Serialize(TimeslipWritePayload.FromTimeslip(timeslip));

        Assert.DoesNotContain("billed_on_invoice", json, StringComparison.Ordinal);
    }
}
