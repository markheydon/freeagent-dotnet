namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Timeslips.TimeslipService.GetTimeslipAsync(long, TimeslipGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class TimeslipGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the linked task when the response contains only a task URI.
    /// </summary>
    public bool IncludeProjectTask { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the linked project when the response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the linked user when the response contains only a user URI.
    /// </summary>
    public bool IncludeUser { get; init; }
}
