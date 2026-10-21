#if NET10_0
using FreeAgent.Client.ConsoleSample;
using FreeAgent.Client.ConsoleSample.Samples;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class SampleRunReporterTests
{
    [Fact]
    public async Task WriteSummaryAndGetExitCode_AllSkipped_ReturnsFailure()
    {
        var exitCode = await CaptureExitCodeAsync(async reporter =>
        {
            await reporter.RunExampleAsync(
                "Contacts",
                "Example",
                _ => throw new SampleSkippedException("no data"),
                cancellationToken: CancellationToken.None);
        });

        Assert.Equal(1, exitCode);
    }

    [Fact]
    public async Task WriteSummaryAndGetExitCode_OnePassed_ReturnsSuccess()
    {
        var exitCode = await CaptureExitCodeAsync(async reporter =>
        {
            await reporter.RunExampleAsync(
                "Contacts",
                "Example",
                _ => Task.CompletedTask,
                cancellationToken: CancellationToken.None);
        });

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task WriteSummaryAndGetExitCode_OneFailed_ReturnsFailure()
    {
        var exitCode = await CaptureExitCodeAsync(async reporter =>
        {
            await reporter.RunExampleAsync(
                "Contacts",
                "Example",
                _ => throw new InvalidOperationException("boom"),
                cancellationToken: CancellationToken.None);
        });

        Assert.Equal(1, exitCode);
    }

    /// <summary>
    /// Redirects console output so Microsoft.Testing.Platform does not treat reporter summary lines as test failures.
    /// </summary>
    private static async Task<int> CaptureExitCodeAsync(Func<SampleRunReporter, Task> run)
    {
        var writer = new StringWriter();
        var previous = Console.Out;
        try
        {
            Console.SetOut(writer);
            var reporter = new SampleRunReporter();
            await run(reporter);
            return reporter.WriteSummaryAndGetExitCode();
        }
        finally
        {
            Console.SetOut(previous);
        }
    }
}

#endif
