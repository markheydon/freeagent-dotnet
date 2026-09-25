namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.Estimates.EstimateService.UpdateEstimateAsync(long, Models.Estimates.Estimate, EstimateUpdateOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class EstimateUpdateOptions
{
    /// <summary>
    /// When <see langword="true"/>, line items are excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitLineItems { get; init; }

    /// <summary>
    /// When <see langword="true"/>, the contact link is excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitContact { get; init; }

    /// <summary>
    /// When <see langword="true"/>, the project link is excluded from the update payload even when present on the model.
    /// </summary>
    public bool OmitProject { get; init; }
}
