using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Company;

/// <summary>
/// Represents the legal type of a FreeAgent company.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CompanyType
{
    /// <summary>UK limited company.</summary>
    [JsonStringEnumMemberName("UkLimitedCompany")]
    UkLimitedCompany,
    /// <summary>UK limited liability partnership.</summary>
    [JsonStringEnumMemberName("UkLimitedLiabilityPartnership")]
    UkLimitedLiabilityPartnership,
    /// <summary>UK partnership.</summary>
    [JsonStringEnumMemberName("UkPartnership")]
    UkPartnership,
    /// <summary>UK sole trader.</summary>
    [JsonStringEnumMemberName("UkSoleTrader")]
    UkSoleTrader,
    /// <summary>UK unincorporated landlord.</summary>
    [JsonStringEnumMemberName("UkUnincorporatedLandlord")]
    UkUnincorporatedLandlord,
    /// <summary>US limited liability company.</summary>
    [JsonStringEnumMemberName("UsLimitedLiabilityCompany")]
    UsLimitedLiabilityCompany,
    /// <summary>US partnership.</summary>
    [JsonStringEnumMemberName("UsPartnership")]
    UsPartnership,
    /// <summary>US sole proprietor.</summary>
    [JsonStringEnumMemberName("UsSoleProprietor")]
    UsSoleProprietor,
    /// <summary>US C corporation.</summary>
    [JsonStringEnumMemberName("UsCCorp")]
    UsCCorp,
    /// <summary>US S corporation.</summary>
    [JsonStringEnumMemberName("UsSCorp")]
    UsSCorp,
    /// <summary>Universal company type for non-UK/US entities.</summary>
    [JsonStringEnumMemberName("UniversalCompany")]
    UniversalCompany
}
