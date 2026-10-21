namespace FreeAgent.Client.Models.CapitalAssetTypes;

/// <summary>
/// Attributes for creating a capital asset type.
/// </summary>
public sealed class CreateCapitalAssetTypeRequest
{
    /// <summary>
    /// Display name for the new capital asset type.
    /// </summary>
    public required string Name { get; set; }
}
