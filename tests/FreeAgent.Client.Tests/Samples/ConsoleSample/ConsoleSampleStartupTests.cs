#if NET10_0
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class ConsoleSampleStartupTests
{
    [Fact]
    public void WriteEnvironmentNotice_Sandbox_WritesInformationalNote()
    {
        var output = CaptureOutput(
            () => ConsoleSampleStartup.WriteEnvironmentNotice(FreeAgentEnvironment.Sandbox, allowProductionWrites: false));

        Assert.Contains("Note:", output, StringComparison.Ordinal);
        Assert.Contains("sandbox", output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Warning:", output, StringComparison.Ordinal);
    }

    [Fact]
    public void WriteEnvironmentNotice_ProductionWithHatch_WritesWarning()
    {
        var output = CaptureOutput(
            () => ConsoleSampleStartup.WriteEnvironmentNotice(FreeAgentEnvironment.Production, allowProductionWrites: true));

        Assert.Contains("Warning:", output, StringComparison.Ordinal);
        Assert.Contains("Production writes enabled", output, StringComparison.Ordinal);
    }

    [Fact]
    public void WriteEnvironmentNotice_ProductionWithoutHatch_WritesSkipNote()
    {
        var output = CaptureOutput(
            () => ConsoleSampleStartup.WriteEnvironmentNotice(FreeAgentEnvironment.Production, allowProductionWrites: false));

        Assert.Contains("Note:", output, StringComparison.Ordinal);
        Assert.Contains("skipped", output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Warning:", output, StringComparison.Ordinal);
    }

    private static string CaptureOutput(Action action)
    {
        var writer = new StringWriter();
        var previous = Console.Out;
        try
        {
            Console.SetOut(writer);
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(previous);
        }
    }
}

#endif
