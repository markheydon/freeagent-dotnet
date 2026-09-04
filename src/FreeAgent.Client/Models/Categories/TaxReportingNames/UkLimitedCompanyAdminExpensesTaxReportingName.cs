using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>Tax reporting name wire values for admin expenses categories on UK limited companies.</summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<UkLimitedCompanyAdminExpensesTaxReportingName>))]
public enum UkLimitedCompanyAdminExpensesTaxReportingName
{
    /// <summary>Accountancy fees.</summary>
    [JsonStringEnumMemberName("accountancy_fees")]
    AccountancyFees,

    /// <summary>Advertising and promotional costs.</summary>
    [JsonStringEnumMemberName("advertising_and_promotional_costs")]
    AdvertisingAndPromotionalCosts,

    /// <summary>Amortisation of intangible assets.</summary>
    [JsonStringEnumMemberName("amortisation_of_intangible_assets")]
    AmortisationOfIntangibleAssets,

    /// <summary>Bad debts written off.</summary>
    [JsonStringEnumMemberName("bad_debts_written_off")]
    BadDebtsWrittenOff,

    /// <summary>Bank charges.</summary>
    [JsonStringEnumMemberName("bank_charges")]
    BankCharges,

    /// <summary>Business entertaining.</summary>
    [JsonStringEnumMemberName("business_entertaining")]
    BusinessEntertaining,

    /// <summary>Canteen.</summary>
    [JsonStringEnumMemberName("canteen")]
    Canteen,

    /// <summary>Charitable donations.</summary>
    [JsonStringEnumMemberName("charitable_donations")]
    CharitableDonations,

    /// <summary>Computer software costs.</summary>
    [JsonStringEnumMemberName("computer_software_costs")]
    ComputerSoftwareCosts,

    /// <summary>Consumable items.</summary>
    [JsonStringEnumMemberName("consumable_items")]
    ConsumableItems,

    /// <summary>Credit card charges.</summary>
    [JsonStringEnumMemberName("credit_card_charges")]
    CreditCardCharges,

    /// <summary>Depreciation of tangible fixed assets.</summary>
    [JsonStringEnumMemberName("depreciation_of_tangible_fixed_assets")]
    DepreciationOfTangibleFixedAssets,

    /// <summary>Directors pensions.</summary>
    [JsonStringEnumMemberName("directors_pensions")]
    DirectorsPensions,

    /// <summary>Directors remuneration.</summary>
    [JsonStringEnumMemberName("directors_remuneration")]
    DirectorsRemuneration,

    /// <summary>Employers ni directors.</summary>
    [JsonStringEnumMemberName("employers_ni_directors")]
    EmployersNiDirectors,

    /// <summary>Employers ni staff.</summary>
    [JsonStringEnumMemberName("employers_ni_staff")]
    EmployersNiStaff,

    /// <summary>Finance charges.</summary>
    [JsonStringEnumMemberName("finance_charges")]
    FinanceCharges,

    /// <summary>Foreign exchange transaction charges.</summary>
    [JsonStringEnumMemberName("foreign_exchange_transaction_charges")]
    ForeignExchangeTransactionCharges,

    /// <summary>Gain from disposal of tangible fixed assets.</summary>
    [JsonStringEnumMemberName("gain_from_disposal_of_tangible_fixed_assets")]
    GainFromDisposalOfTangibleFixedAssets,

    /// <summary>Gain on foreign currency transactions.</summary>
    [JsonStringEnumMemberName("gain_on_foreign_currency_transactions")]
    GainOnForeignCurrencyTransactions,

    /// <summary>General consultancy fees.</summary>
    [JsonStringEnumMemberName("general_consultancy_fees")]
    GeneralConsultancyFees,

    /// <summary>General maintenance.</summary>
    [JsonStringEnumMemberName("general_maintenance")]
    GeneralMaintenance,

    /// <summary>Hire and leasing of computer equipment.</summary>
    [JsonStringEnumMemberName("hire_and_leasing_of_computer_equipment")]
    HireAndLeasingOfComputerEquipment,

    /// <summary>Hire and leasing of motor vehicles.</summary>
    [JsonStringEnumMemberName("hire_and_leasing_of_motor_vehicles")]
    HireAndLeasingOfMotorVehicles,

    /// <summary>Hire and leasing of other assets.</summary>
    [JsonStringEnumMemberName("hire_and_leasing_of_other_assets")]
    HireAndLeasingOfOtherAssets,

    /// <summary>It and computer consumables.</summary>
    [JsonStringEnumMemberName("it_and_computer_consumables")]
    ItAndComputerConsumables,

    /// <summary>Insurance.</summary>
    [JsonStringEnumMemberName("insurance")]
    Insurance,

    /// <summary>Insurance on premises.</summary>
    [JsonStringEnumMemberName("insurance_on_premises")]
    InsuranceOnPremises,

    /// <summary>Interest payable.</summary>
    [JsonStringEnumMemberName("interest_payable")]
    InterestPayable,

    /// <summary>Irrecoverable vat.</summary>
    [JsonStringEnumMemberName("irrecoverable_vat")]
    IrrecoverableVat,

    /// <summary>Late payment of tax.</summary>
    [JsonStringEnumMemberName("late_payment_of_tax")]
    LatePaymentOfTax,

    /// <summary>Leases and hire purchase contracts.</summary>
    [JsonStringEnumMemberName("leases_and_hire_purchase_contracts")]
    LeasesAndHirePurchaseContracts,

    /// <summary>Legal fees.</summary>
    [JsonStringEnumMemberName("legal_fees")]
    LegalFees,

    /// <summary>Management fees.</summary>
    [JsonStringEnumMemberName("management_fees")]
    ManagementFees,

    /// <summary>Other legal and professional fees.</summary>
    [JsonStringEnumMemberName("other_legal_and_professional_fees")]
    OtherLegalAndProfessionalFees,

    /// <summary>Political donations.</summary>
    [JsonStringEnumMemberName("political_donations")]
    PoliticalDonations,

    /// <summary>Postage costs.</summary>
    [JsonStringEnumMemberName("postage_costs")]
    PostageCosts,

    /// <summary>Premises cleaning.</summary>
    [JsonStringEnumMemberName("premises_cleaning")]
    PremisesCleaning,

    /// <summary>Premises repairs and maintenance.</summary>
    [JsonStringEnumMemberName("premises_repairs_and_maintenance")]
    PremisesRepairsAndMaintenance,

    /// <summary>Premises repairs and renewals.</summary>
    [JsonStringEnumMemberName("premises_repairs_and_renewals")]
    PremisesRepairsAndRenewals,

    /// <summary>Printing costs.</summary>
    [JsonStringEnumMemberName("printing_costs")]
    PrintingCosts,

    /// <summary>Publication and other information subscriptions.</summary>
    [JsonStringEnumMemberName("publication_and_other_information_subscriptions")]
    PublicationAndOtherInformationSubscriptions,

    /// <summary>Rates on premises.</summary>
    [JsonStringEnumMemberName("rates_on_premises")]
    RatesOnPremises,

    /// <summary>Rent of premises.</summary>
    [JsonStringEnumMemberName("rent_of_premises")]
    RentOfPremises,

    /// <summary>Research and development costs.</summary>
    [JsonStringEnumMemberName("research_and_development_costs")]
    ResearchAndDevelopmentCosts,

    /// <summary>Staff benefits in kind.</summary>
    [JsonStringEnumMemberName("staff_benefits_in_kind")]
    StaffBenefitsInKind,

    /// <summary>Staff entertaining.</summary>
    [JsonStringEnumMemberName("staff_entertaining")]
    StaffEntertaining,

    /// <summary>Staff pensions.</summary>
    [JsonStringEnumMemberName("staff_pensions")]
    StaffPensions,

    /// <summary>Staff training.</summary>
    [JsonStringEnumMemberName("staff_training")]
    StaffTraining,

    /// <summary>Staff welfare.</summary>
    [JsonStringEnumMemberName("staff_welfare")]
    StaffWelfare,

    /// <summary>Stationery.</summary>
    [JsonStringEnumMemberName("stationery")]
    Stationery,

    /// <summary>Subscriptions to professional and trade bodies.</summary>
    [JsonStringEnumMemberName("subscriptions_to_professional_and_trade_bodies")]
    SubscriptionsToProfessionalAndTradeBodies,

    /// <summary>Sundry expenses.</summary>
    [JsonStringEnumMemberName("sundry_expenses")]
    SundryExpenses,

    /// <summary>Telecommunication costs.</summary>
    [JsonStringEnumMemberName("telecommunication_costs")]
    TelecommunicationCosts,

    /// <summary>Travel and subsistence.</summary>
    [JsonStringEnumMemberName("travel_and_subsistence")]
    TravelAndSubsistence,

    /// <summary>Use of residence.</summary>
    [JsonStringEnumMemberName("use_of_residence")]
    UseOfResidence,

    /// <summary>Vehicle running costs.</summary>
    [JsonStringEnumMemberName("vehicle_running_costs")]
    VehicleRunningCosts,

    /// <summary>Wages and salaries.</summary>
    [JsonStringEnumMemberName("wages_and_salaries")]
    WagesAndSalaries,

}
