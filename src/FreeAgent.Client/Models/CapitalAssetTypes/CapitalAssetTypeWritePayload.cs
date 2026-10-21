using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.CapitalAssetTypes;

/// <summary>
/// Writable capital asset type attributes for create and update requests.
/// </summary>
internal sealed class CapitalAssetTypeWritePayload
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    public static CapitalAssetTypeWritePayload FromCreate(CreateCapitalAssetTypeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CapitalAssetTypeWritePayload
        {
            Name = request.Name
        };
    }

    public static CapitalAssetTypeWritePayload FromUpdate(UpdateCapitalAssetTypeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CapitalAssetTypeWritePayload
        {
            Name = request.Name
        };
    }
}
