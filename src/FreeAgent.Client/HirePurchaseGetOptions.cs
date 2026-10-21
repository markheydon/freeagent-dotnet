namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.HirePurchases.HirePurchasesService.GetHirePurchaseAsync(long, HirePurchaseGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class HirePurchaseGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the originating bill when the hire purchase response contains only a bill URI.
    /// </summary>
    public bool IncludeBill { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the liabilities over one year category when the response contains only a category URI.
    /// </summary>
    public bool IncludeLiabilitiesOverOneYearCategory { get; init; }

    /// <summary>
    /// When <see langword="true"/>, fetches the liabilities under one year category when the response contains only a category URI.
    /// </summary>
    public bool IncludeLiabilitiesUnderOneYearCategory { get; init; }
}
