namespace FreeAgent.Client.Models.Timeslips;

/// <summary>
/// Documented view filters for timeslip list endpoints.
/// </summary>
public static class TimeslipViews
{
    /// <summary>Return all timeslips.</summary>
    public const string All = "all";

    /// <summary>Return only timeslips which have not yet been rebilled to a project.</summary>
    public const string Unbilled = "unbilled";

    /// <summary>Return only timeslips which have running timers.</summary>
    public const string Running = "running";
}
