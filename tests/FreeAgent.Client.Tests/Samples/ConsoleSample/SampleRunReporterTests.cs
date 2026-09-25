#if NET10_0
using FreeAgent.Client.ConsoleSample;
using FreeAgent.Client.ConsoleSample.Samples;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class SampleRunReporterTests
{
    [Fact]
    public async Task WriteSummaryAndGetExitCode_AllSkipped_ReturnsFailure()
    {
        var reporter = new SampleRunReporter();

        await reporter.RunExampleAsync(
            "Contacts",
            "Example",
            _ => throw new SampleSkippedException("no data"),
            cancellationToken: CancellationToken.None);

        var exitCode = reporter.WriteSummaryAndGetExitCode();

        Assert.Equal(1, exitCode);
    }

    [Fact]
    public async Task WriteSummaryAndGetExitCode_OnePassed_ReturnsSuccess()
    {
        var reporter = new SampleRunReporter();

        await reporter.RunExampleAsync(
            "Contacts",
            "Example",
            _ => Task.CompletedTask,
            cancellationToken: CancellationToken.None);

        var exitCode = reporter.WriteSummaryAndGetExitCode();

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task WriteSummaryAndGetExitCode_OneFailed_ReturnsFailure()
    {
        var reporter = new SampleRunReporter();

        await reporter.RunExampleAsync(
            "Contacts",
            "Example",
            _ => throw new InvalidOperationException("boom"),
            cancellationToken: CancellationToken.None);

        var exitCode = reporter.WriteSummaryAndGetExitCode();

        Assert.Equal(1, exitCode);
    }
}

#endif
