using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for admin expenses categories on universal and US companies.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UniversalAndUsAdminExpensesTaxReportingName>))]
public enum UniversalAndUsAdminExpensesTaxReportingName
{
    /// <summary>Advertising.</summary>
    [JsonStringEnumMemberName("advertising")]
    Advertising,

    /// <summary>Bad debts written off.</summary>
    [JsonStringEnumMemberName("bad_debts_written_off")]
    BadDebtsWrittenOff,

    /// <summary>Car and truck expenses.</summary>
    [JsonStringEnumMemberName("car_and_truck_expenses")]
    CarAndTruckExpenses,

    /// <summary>Commissions and fees.</summary>
    [JsonStringEnumMemberName("commissions_and_fees")]
    CommissionsAndFees,

    /// <summary>Contract labor.</summary>
    [JsonStringEnumMemberName("contract_labor")]
    ContractLabor,

    /// <summary>Depletion.</summary>
    [JsonStringEnumMemberName("depletion")]
    Depletion,

    /// <summary>Depreciation.</summary>
    [JsonStringEnumMemberName("depreciation")]
    Depreciation,

    /// <summary>Employee benefit programs.</summary>
    [JsonStringEnumMemberName("employee_benefit_programs")]
    EmployeeBenefitPrograms,

    /// <summary>Expenses for business use of home.</summary>
    [JsonStringEnumMemberName("expenses_for_business_use_of_home")]
    ExpensesForBusinessUseOfHome,

    /// <summary>Insurance.</summary>
    [JsonStringEnumMemberName("insurance")]
    Insurance,

    /// <summary>Mortgage interest.</summary>
    [JsonStringEnumMemberName("mortgage_interest")]
    MortgageInterest,

    /// <summary>Other interest.</summary>
    [JsonStringEnumMemberName("other_interest")]
    OtherInterest,

    /// <summary>Legal and professional services.</summary>
    [JsonStringEnumMemberName("legal_and_professional_services")]
    LegalAndProfessionalServices,

    /// <summary>Meals and entertainment.</summary>
    [JsonStringEnumMemberName("meals_and_entertainment")]
    MealsAndEntertainment,

    /// <summary>Office expense.</summary>
    [JsonStringEnumMemberName("office_expense")]
    OfficeExpense,

    /// <summary>Other expenses.</summary>
    [JsonStringEnumMemberName("other_expenses")]
    OtherExpenses,

    /// <summary>Pension and profit sharing plans.</summary>
    [JsonStringEnumMemberName("pension_and_profit_sharing_plans")]
    PensionAndProfitSharingPlans,

    /// <summary>Other business property rent.</summary>
    [JsonStringEnumMemberName("other_business_property_rent")]
    OtherBusinessPropertyRent,

    /// <summary>Vehicle machinery and equipment rent.</summary>
    [JsonStringEnumMemberName("vehicle_machinery_and_equipment_rent")]
    VehicleMachineryAndEquipmentRent,

    /// <summary>Repairs and maintenance.</summary>
    [JsonStringEnumMemberName("repairs_and_maintenance")]
    RepairsAndMaintenance,

    /// <summary>Supplies.</summary>
    [JsonStringEnumMemberName("supplies")]
    Supplies,

    /// <summary>Taxes and licenses.</summary>
    [JsonStringEnumMemberName("taxes_and_licenses")]
    TaxesAndLicenses,

    /// <summary>Travel.</summary>
    [JsonStringEnumMemberName("travel")]
    Travel,

    /// <summary>Utilities.</summary>
    [JsonStringEnumMemberName("utilities")]
    Utilities,

    /// <summary>Wages.</summary>
    [JsonStringEnumMemberName("wages")]
    Wages,

}
