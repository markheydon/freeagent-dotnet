using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Date format preference used throughout a FreeAgent account.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ShortDateFormat>))]
public enum ShortDateFormat
{
    /// <summary>dd mmm yy</summary>
    [JsonStringEnumMemberName("dd mmm yy")]
    DayMonthAbbreviatedYear,

    /// <summary>dd-mm-yyyy</summary>
    [JsonStringEnumMemberName("dd-mm-yyyy")]
    DayMonthYearDashes,

    /// <summary>mm/dd/yyyy</summary>
    [JsonStringEnumMemberName("mm/dd/yyyy")]
    MonthDayYearSlashes,

    /// <summary>yyyy-mm-dd</summary>
    [JsonStringEnumMemberName("yyyy-mm-dd")]
    YearMonthDayDashes
}
