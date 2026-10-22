using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/expenses</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ExpenseView>))]
public enum ExpenseView
{
    /// <summary>Recent expenses only.</summary>
    [JsonStringEnumMemberName("recent")]
    Recent,

    /// <summary>Recurring expenses only.</summary>
    [JsonStringEnumMemberName("recurring")]
    Recurring,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
