using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for admin expenses categories on UK partnerships.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UkPartnershipAdminExpensesTaxReportingName>))]
public enum UkPartnershipAdminExpensesTaxReportingName
{
    /// <summary>Advertising etc.</summary>
    [JsonStringEnumMemberName("advertising_etc")]
    AdvertisingEtc,

    /// <summary>Bad debts.</summary>
    [JsonStringEnumMemberName("bad_debts")]
    BadDebts,

    /// <summary>Business entertaining.</summary>
    [JsonStringEnumMemberName("business_entertaining")]
    BusinessEntertaining,

    /// <summary>Depreciation and loss.</summary>
    [JsonStringEnumMemberName("depreciation_and_loss")]
    DepreciationAndLoss,

    /// <summary>Employee costs.</summary>
    [JsonStringEnumMemberName("employee_costs")]
    EmployeeCosts,

    /// <summary>General administrative expenses.</summary>
    [JsonStringEnumMemberName("general_administrative_expenses")]
    GeneralAdministrativeExpenses,

    /// <summary>Interest.</summary>
    [JsonStringEnumMemberName("interest")]
    Interest,

    /// <summary>Legal and professional costs.</summary>
    [JsonStringEnumMemberName("legal_and_professional_costs")]
    LegalAndProfessionalCosts,

    /// <summary>Motor expenses.</summary>
    [JsonStringEnumMemberName("motor_expenses")]
    MotorExpenses,

    /// <summary>Other direct costs.</summary>
    [JsonStringEnumMemberName("other_direct_costs")]
    OtherDirectCosts,

    /// <summary>Other expenses.</summary>
    [JsonStringEnumMemberName("other_expenses")]
    OtherExpenses,

    /// <summary>Other finance charges.</summary>
    [JsonStringEnumMemberName("other_finance_charges")]
    OtherFinanceCharges,

    /// <summary>Premises costs.</summary>
    [JsonStringEnumMemberName("premises_costs")]
    PremisesCosts,

    /// <summary>Repairs.</summary>
    [JsonStringEnumMemberName("repairs")]
    Repairs,

    /// <summary>Travel and subsistence.</summary>
    [JsonStringEnumMemberName("travel_and_subsistence")]
    TravelAndSubsistence,

}
