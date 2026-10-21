using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for listing capital assets.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<CapitalAssetView>))]
public enum CapitalAssetView
{
    /// <summary>
    /// All capital assets. This is the API default when <c>view</c> is omitted.
    /// </summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>
    /// Capital assets that have been disposed of.
    /// </summary>
    [JsonStringEnumMemberName("disposed")]
    Disposed,

    /// <summary>
    /// Capital assets that can be disposed of.
    /// </summary>
    [JsonStringEnumMemberName("disposable")]
    Disposable,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
