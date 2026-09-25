using FreeAgent.Client;

namespace FreeAgent.Client.ConsoleSample;

/// <summary>
/// Resolves the target FreeAgent API environment for the console sample.
/// </summary>
internal static class SampleEnvironment
{
    /// <summary>
    /// Environment variable override for local experiments (<c>Sandbox</c> or <c>Production</c>).
    /// </summary>
    public const string EnvironmentVariableName = "FREEAGENT_ENVIRONMENT";

    /// <summary>
    /// Resolves the API environment from <see cref="EnvironmentVariableName"/> or the sample default.
    /// </summary>
    /// <returns>Target API environment.</returns>
    public static FreeAgentEnvironment Resolve()
    {
        var value = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        if (string.IsNullOrWhiteSpace(value))
        {
            return AuthBootstrap.DefaultEnvironment;
        }

        var trimmed = value.Trim();
        if (!Enum.TryParse<FreeAgentEnvironment>(trimmed, ignoreCase: true, out var environment)
            || !string.Equals(Enum.GetName(environment), trimmed, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"{EnvironmentVariableName} must be 'Sandbox' or 'Production' (got '{value}').");
        }

        return environment;
    }
}
