namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Estimates.EstimateService.GetEstimateAsync(long, EstimateGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class EstimateGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the contact when the estimate response contains only a contact URI.
    /// </summary>
    public bool IncludeContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the project when the estimate response contains only a project URI.
    /// </summary>
    public bool IncludeProject { get; init; }
}
