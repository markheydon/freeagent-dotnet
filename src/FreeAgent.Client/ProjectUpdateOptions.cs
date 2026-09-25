namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Projects.ProjectService.UpdateProjectAsync(long, Models.Projects.Project, ProjectUpdateOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class ProjectUpdateOptions
{
    /// <summary>
    /// When <see langword="true"/>, the billing contact is excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitContact { get; init; }
}
