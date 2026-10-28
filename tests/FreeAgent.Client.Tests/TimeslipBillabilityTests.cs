using System.Text.Json;
using FreeAgent.Client.Tests.Infrastructure.Serialization;

namespace FreeAgent.Client.Tests;

public class TimeslipBillabilityTests
{
    [Fact]
    public void Resolve_WhenTaskNotBillable_ReturnsUnbillable()
    {
        var timeslip = new Timeslip();
        var task = new ProjectTask { IsBillable = false };

        Assert.Equal(TimeslipBillabilityCategory.Unbillable, TimeslipBillability.Resolve(timeslip, task));
    }

    [Fact]
    public void Resolve_WhenBillableAndNotInvoiced_ReturnsUnbilled()
    {
        var timeslip = new Timeslip();
        var task = new ProjectTask { IsBillable = true };

        Assert.Equal(TimeslipBillabilityCategory.Unbilled, TimeslipBillability.Resolve(timeslip, task));
    }

    [Fact]
    public void Resolve_WhenBillableAndInvoiced_ReturnsBilled()
    {
        const string json = """
        {
          "billed_on_invoice": "https://api.freeagent.com/v2/invoices/7"
        }
        """;

        var timeslip = JsonSerializer.Deserialize<Timeslip>(json, FreeAgentJsonTestSupport.Options)!;
        var task = new ProjectTask { IsBillable = true };

        Assert.Equal(TimeslipBillabilityCategory.Billed, TimeslipBillability.Resolve(timeslip, task));
    }

    [Fact]
    public void Resolve_WhenTaskBillabilityUnknown_ReturnsUnknown()
    {
        var timeslip = new Timeslip();
        var task = new ProjectTask { IsBillable = null };

        Assert.Equal(TimeslipBillabilityCategory.Unknown, TimeslipBillability.Resolve(timeslip, task));
    }

    [Fact]
    public void Resolve_UsesNestedTaskOnTimeslipWhenTaskArgumentOmitted()
    {
        const string json = """
        {
          "task": {
            "url": "https://api.freeagent.com/v2/tasks/1",
            "is_billable": true
          }
        }
        """;

        var timeslip = JsonSerializer.Deserialize<Timeslip>(json, FreeAgentJsonTestSupport.Options)!;

        Assert.Equal(TimeslipBillabilityCategory.Unbilled, TimeslipBillability.Resolve(timeslip));
    }

    [Fact]
    public void Resolve_NullTimeslip_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TimeslipBillability.Resolve(null!));
    }
}
