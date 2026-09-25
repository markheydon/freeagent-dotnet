#if NET10_0
using FreeAgent.Client.Samples.Shared.Turpinverse;

namespace FreeAgent.Client.Tests.Samples.Turpinverse;

public class TurpinverseSeedModelsTests
{
    [Fact]
    public void StageOrder_IncludesEveryEnumValue()
    {
        var orderedStages = TurpinverseSeedStageOrder.All.ToHashSet();

        foreach (var stage in Enum.GetValues<TurpinverseSeedStage>())
        {
            Assert.Contains(stage, orderedStages);
        }
    }

    [Fact]
    public void StageOrder_ContactsFirst_TimeslipsLast()
    {
        var stages = TurpinverseSeedStageOrder.All;

        Assert.Equal(8, stages.Count);
        Assert.Equal(TurpinverseSeedStage.Contacts, stages[0]);
        Assert.Equal(TurpinverseSeedStage.Projects, stages[1]);
        Assert.Equal(TurpinverseSeedStage.Tasks, stages[2]);
        Assert.Equal(TurpinverseSeedStage.Timeslips, stages[^1]);
    }

    [Fact]
    public void RunResult_Succeeded_WhenAllStagesHaveNoFailures()
    {
        var stages = TurpinverseSeedStageOrder.All
            .Select(stage => new TurpinverseSeedStageResult(stage, Created: 1, Updated: 0, Failures: []))
            .ToList();

        var result = new TurpinverseSeedRunResult(stages, TimeSpan.FromSeconds(1));

        Assert.True(result.Succeeded);
        Assert.Equal(8, result.TotalCreated);
        Assert.Equal(0, result.TotalUpdated);
        Assert.Equal(0, result.TotalFailures);
    }

    [Fact]
    public void RunResult_NotSucceeded_WhenAnyStageHasFailures()
    {
        var stages = new List<TurpinverseSeedStageResult>
        {
            new(TurpinverseSeedStage.Contacts, 1, 0, []),
            new(TurpinverseSeedStage.Projects, 0, 0, ["missing contact"]),
        };

        var result = new TurpinverseSeedRunResult(stages, TimeSpan.FromSeconds(1));

        Assert.False(result.Succeeded);
        Assert.Equal(1, result.TotalFailures);
    }
}

#endif
