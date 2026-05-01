namespace FreeAgent.Client.Infrastructure.Configuration;

/// <summary>
/// Specifies the FreeAgent API environment to target.
/// </summary>
public enum FreeAgentEnvironment
{
    /// <summary>
    /// The FreeAgent production environment (<c>https://api.freeagent.com/v2/</c>).
    /// </summary>
    Production,

    /// <summary>
    /// The FreeAgent sandbox environment (<c>https://api.sandbox.freeagent.com/v2/</c>).
    /// Use for safe, isolated testing without real data.
    /// </summary>
    Sandbox
}
