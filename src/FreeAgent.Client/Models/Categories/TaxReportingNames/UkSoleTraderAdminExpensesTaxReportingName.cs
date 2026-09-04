using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for admin expenses categories on UK sole traders.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UkSoleTraderAdminExpensesTaxReportingName>))]
public enum UkSoleTraderAdminExpensesTaxReportingName
{
    /// <summary>Accountancy and legal fees.</summary>
    [JsonStringEnumMemberName("accountancy_and_legal_fees")]
    AccountancyAndLegalFees,

    /// <summary>Advertising costs.</summary>
    [JsonStringEnumMemberName("advertising_costs")]
    AdvertisingCosts,

    /// <summary>Bank and loan interest.</summary>
    [JsonStringEnumMemberName("bank_and_loan_interest")]
    BankAndLoanInterest,

    /// <summary>Car van and travel expenses.</summary>
    [JsonStringEnumMemberName("car_van_and_travel_expenses")]
    CarVanAndTravelExpenses,

    /// <summary>Debts written off.</summary>
    [JsonStringEnumMemberName("debts_written_off")]
    DebtsWrittenOff,

    /// <summary>Depreciation and loss profit on sale.</summary>
    [JsonStringEnumMemberName("depreciation_and_loss_profit_on_sale")]
    DepreciationAndLossProfitOnSale,

    /// <summary>Entertainment costs.</summary>
    [JsonStringEnumMemberName("entertainment_costs")]
    EntertainmentCosts,

    /// <summary>Other business expenses.</summary>
    [JsonStringEnumMemberName("other_business_expenses")]
    OtherBusinessExpenses,

    /// <summary>Other finance charges.</summary>
    [JsonStringEnumMemberName("other_finance_charges")]
    OtherFinanceCharges,

    /// <summary>Phone and other office costs.</summary>
    [JsonStringEnumMemberName("phone_and_other_office_costs")]
    PhoneAndOtherOfficeCosts,

    /// <summary>Rent and other property costs.</summary>
    [JsonStringEnumMemberName("rent_and_other_property_costs")]
    RentAndOtherPropertyCosts,

    /// <summary>Repair and renewal costs.</summary>
    [JsonStringEnumMemberName("repair_and_renewal_costs")]
    RepairAndRenewalCosts,

    /// <summary>Wages salaries and staff costs.</summary>
    [JsonStringEnumMemberName("wages_salaries_and_staff_costs")]
    WagesSalariesAndStaffCosts,

}
