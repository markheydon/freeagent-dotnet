using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Whether a bank account is visible in FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<BankAccountStatus>))]
public enum BankAccountStatus
{
    /// <summary>
    /// Active bank account.
    /// </summary>
    [JsonStringEnumMemberName("active")]
    Active,

    /// <summary>
    /// Hidden bank account.
    /// </summary>
    [JsonStringEnumMemberName("hidden")]
    Hidden,
    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
