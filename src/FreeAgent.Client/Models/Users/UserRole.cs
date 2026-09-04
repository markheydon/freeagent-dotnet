using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Users;

/// <summary>
/// User role values returned and accepted by FreeAgent.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UserRole>))]
public enum UserRole
{
    /// <summary>Company owner (wire value: "Owner").</summary>
    [JsonStringEnumMemberName("Owner")]
    Owner,

    /// <summary>Company director (wire value: "Director").</summary>
    [JsonStringEnumMemberName("Director")]
    Director,

    /// <summary>Company partner (wire value: "Partner").</summary>
    [JsonStringEnumMemberName("Partner")]
    Partner,

    /// <summary>Company secretary (wire value: "Company Secretary").</summary>
    [JsonStringEnumMemberName("Company Secretary")]
    CompanySecretary,

    /// <summary>Employee (wire value: "Employee").</summary>
    [JsonStringEnumMemberName("Employee")]
    Employee,

    /// <summary>Shareholder (wire value: "Shareholder").</summary>
    [JsonStringEnumMemberName("Shareholder")]
    Shareholder,

    /// <summary>Accountant advisor (wire value: "Accountant").</summary>
    [JsonStringEnumMemberName("Accountant")]
    Accountant
}
