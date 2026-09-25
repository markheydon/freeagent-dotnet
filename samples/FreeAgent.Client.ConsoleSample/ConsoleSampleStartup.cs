using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Startup messaging for the console sample.
/// </summary>
internal static class ConsoleSampleStartup
{
    /// <summary>
    /// Prints environment-specific guidance about mutating examples.
    /// </summary>
    /// <param name="environment">Resolved API environment.</param>
    /// <param name="allowProductionWrites">Whether interactive production writes are permitted.</param>
    public static void WriteEnvironmentNotice(FreeAgentEnvironment environment, bool allowProductionWrites)
    {
        if (environment == FreeAgentEnvironment.Sandbox)
        {
            Console.WriteLine(
                "Note: Examples marked as mutating create, update, or delete data in the connected sandbox account.");
            return;
        }

        if (allowProductionWrites)
        {
            Console.WriteLine(
                "Warning: Production writes enabled — mutating examples will modify live account data.");
            return;
        }

        Console.WriteLine(
            "Note: Mutating examples are skipped on production. Read-only examples remain available.");
    }
}
