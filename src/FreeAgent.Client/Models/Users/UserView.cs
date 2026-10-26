using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/users</c>.
/// </summary>
/// <remarks>
/// Member names follow the SDK; wire values use snake_case (for example <see cref="ActiveStaff"/> → <c>active_staff</c>).
/// There is no <c>Active</c> member — use <see cref="ActiveStaff"/> for non-hidden staff users.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UserView>))]
public enum UserView
{
    /// <summary>All users (FreeAgent default).</summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>Users with a staff role.</summary>
    [JsonStringEnumMemberName("staff")]
    Staff,

    /// <summary>Non-hidden users with a staff role.</summary>
    [JsonStringEnumMemberName("active_staff")]
    ActiveStaff,

    /// <summary>Users with an Accountant role.</summary>
    [JsonStringEnumMemberName("advisors")]
    Advisors,

    /// <summary>Non-hidden users with an Accountant role.</summary>
    [JsonStringEnumMemberName("active_advisors")]
    ActiveAdvisors,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
