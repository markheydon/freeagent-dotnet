namespace FreeAgent.Client;

/// <summary>
/// Attributes for updating a capital asset type.
/// </summary>
public sealed class UpdateCapitalAssetTypeRequest
{
    /// <summary>
    /// Updated display name.
    /// </summary>
    public required string Name { get; set; }
}
