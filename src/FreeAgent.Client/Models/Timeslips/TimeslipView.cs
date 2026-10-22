using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for timeslip list endpoints.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<TimeslipView>))]
public enum TimeslipView
{
    /// <summary>All timeslips (default when omitted).</summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>Timeslips not yet billed on an invoice.</summary>
    [JsonStringEnumMemberName("unbilled")]
    Unbilled,

    /// <summary>Timeslips with running timers.</summary>
    [JsonStringEnumMemberName("running")]
    Running,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
