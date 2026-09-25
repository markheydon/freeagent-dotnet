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
}
