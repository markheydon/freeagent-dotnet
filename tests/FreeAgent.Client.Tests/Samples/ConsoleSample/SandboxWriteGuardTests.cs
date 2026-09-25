#if NET10_0
using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample;
using FreeAgent.Client.ConsoleSample.Samples;

namespace FreeAgent.Client.Tests.Samples.ConsoleSample;

public class SandboxWriteGuardTests
{
    [Fact]
    public void EnsureRunAllAllowed_Sandbox_DoesNotThrow()
    {
        var exception = Record.Exception(() => SandboxWriteGuard.EnsureRunAllAllowed(FreeAgentEnvironment.Sandbox));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureRunAllAllowed_Production_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => SandboxWriteGuard.EnsureRunAllAllowed(FreeAgentEnvironment.Production));

        Assert.Contains("sandbox", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Production", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void EnsureMutatingExampleAllowed_Sandbox_DoesNotThrow()
    {
        var exception = Record.Exception(
            () => SandboxWriteGuard.EnsureMutatingExampleAllowed(FreeAgentEnvironment.Sandbox, allowProductionWrites: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureMutatingExampleAllowed_ProductionWithoutHatch_ThrowsSampleSkippedException()
    {
        var exception = Assert.Throws<SampleSkippedException>(
            () => SandboxWriteGuard.EnsureMutatingExampleAllowed(FreeAgentEnvironment.Production, allowProductionWrites: false));

        Assert.Contains("sandbox", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EnsureMutatingExampleAllowed_ProductionWithHatch_DoesNotThrow()
    {
        var exception = Record.Exception(
            () => SandboxWriteGuard.EnsureMutatingExampleAllowed(FreeAgentEnvironment.Production, allowProductionWrites: true));

        Assert.Null(exception);
    }

    [Fact]
    public void ResolveAllowProductionWrites_FlagTrue_ReturnsTrue()
    {
        Assert.True(SandboxWriteGuard.ResolveAllowProductionWrites(true));
    }

    [Fact]
    public void ResolveAllowProductionWrites_EnvironmentTrue_ReturnsTrue()
    {
        var previous = Environment.GetEnvironmentVariable(SandboxWriteGuard.AllowProductionWritesVariableName);
        try
        {
            Environment.SetEnvironmentVariable(SandboxWriteGuard.AllowProductionWritesVariableName, "true");

            Assert.True(SandboxWriteGuard.ResolveAllowProductionWrites(false));
        }
        finally
        {
            Environment.SetEnvironmentVariable(SandboxWriteGuard.AllowProductionWritesVariableName, previous);
        }
    }
}

#endif
