namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Projects.ProjectService.GetProjectAsync(long, ProjectGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class ProjectGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the billing contact when the project response contains only a contact URI.
    /// </summary>
    public bool IncludeContact { get; init; }
}
