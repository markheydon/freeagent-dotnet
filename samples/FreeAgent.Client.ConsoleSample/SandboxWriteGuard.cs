using FreeAgent.Client;
using FreeAgent.Client.ConsoleSample.Samples;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Enforces sandbox-only execution for mutating console sample examples.
/// </summary>
internal static class SandboxWriteGuard
{
    /// <summary>
    /// Environment variable that opts in to interactive mutating examples on non-sandbox environments.
    /// </summary>
    public const string AllowProductionWritesVariableName = "FREEAGENT_ALLOW_PRODUCTION_WRITES";

    /// <summary>
    /// Ensures non-interactive sandbox smoke, seed, and linked-resource verification runs target sandbox only.
    /// Also applies to the linked-resource verification example from the interactive menu (ignores production-write hatch).
    /// </summary>
    /// <param name="environment">Resolved API environment.</param>
    /// <exception cref="InvalidOperationException">When the environment is not sandbox.</exception>
    public static void EnsureRunAllAllowed(FreeAgentEnvironment environment)
    {
        if (environment == FreeAgentEnvironment.Sandbox)
        {
            return;
        }

        throw new InvalidOperationException(
            $"--run-all, --seed-turpinverse, and --verify-linked-resources require the sandbox environment; connected environment is {environment}. " +
            $"The linked-resource verification example from the interactive menu is sandbox-only and ignores {AllowProductionWritesVariableName}. " +
            $"Set {SampleEnvironment.EnvironmentVariableName}=Sandbox or use the default console sample configuration.");
    }

    /// <summary>
    /// Ensures a mutating example may run in the current environment and mode.
    /// </summary>
    /// <param name="environment">Resolved API environment.</param>
    /// <param name="allowProductionWrites">When <see langword="true"/>, interactive production writes are permitted.</param>
    /// <exception cref="SampleSkippedException">When mutating examples are blocked.</exception>
    public static void EnsureMutatingExampleAllowed(FreeAgentEnvironment environment, bool allowProductionWrites)
    {
        if (environment == FreeAgentEnvironment.Sandbox || allowProductionWrites)
        {
            return;
        }

        throw new SampleSkippedException(
            $"Mutating examples require the sandbox environment; connected environment is {environment}. " +
            $"Use {SampleEnvironment.EnvironmentVariableName}=Sandbox, or set {AllowProductionWritesVariableName}=true " +
            "or pass --allow-production-writes for interactive production writes.");
    }

    /// <summary>
    /// Resolves whether interactive production writes are permitted.
    /// </summary>
    /// <param name="allowProductionWritesFlag">CLI opt-in flag.</param>
    /// <returns><see langword="true"/> when production writes are explicitly allowed.</returns>
    public static bool ResolveAllowProductionWrites(bool allowProductionWritesFlag)
    {
        if (allowProductionWritesFlag)
        {
            return true;
        }

        var value = Environment.GetEnvironmentVariable(AllowProductionWritesVariableName);
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase);
    }
}
