using System.Globalization;

namespace FreeAgent.Client.Models.Categories;

/// <summary>
/// Validates documented nominal-code ranges for typed category create and update requests.
/// Uniqueness within an account is enforced by FreeAgent at request time.
/// </summary>
internal static class CategoryNominalCodeValidator
{
    internal static class Ranges
    {
        internal const int IncomeMin = 1;
        internal const int IncomeMax = 49;

        internal const int CostOfSalesMin = 96;
        internal const int CostOfSalesMax = 199;

        internal const int AdminExpensesMin = 200;
        internal const int AdminExpensesMax = 399;

        internal const int CurrentAssetMin = 671;
        internal const int CurrentAssetMax = 720;

        internal const int LiabilitiesMin = 731;
        internal const int LiabilitiesMax = 780;

        internal const int EquityMin = 921;
        internal const int EquityMax = 960;
    }

    internal static void ValidateIncome(string nominalCode) =>
        ValidateRange(nominalCode, Ranges.IncomeMin, Ranges.IncomeMax, "income");

    internal static void ValidateCostOfSales(string nominalCode) =>
        ValidateRange(nominalCode, Ranges.CostOfSalesMin, Ranges.CostOfSalesMax, "cost of sales");

    internal static void ValidateAdminExpenses(string nominalCode) =>
        ValidateRange(nominalCode, Ranges.AdminExpensesMin, Ranges.AdminExpensesMax, "admin expenses");

    internal static void ValidateCurrentAsset(string nominalCode) =>
        ValidateRange(nominalCode, Ranges.CurrentAssetMin, Ranges.CurrentAssetMax, "current asset");

    internal static void ValidateLiabilities(string nominalCode) =>
        ValidateRange(nominalCode, Ranges.LiabilitiesMin, Ranges.LiabilitiesMax, "liabilities");

    internal static void ValidateEquity(string nominalCode) =>
        ValidateRange(nominalCode, Ranges.EquityMin, Ranges.EquityMax, "equity");

    private static void ValidateRange(string nominalCode, int minInclusive, int maxInclusive, string variantName)
    {
        if (!int.TryParse(nominalCode, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            throw new ArgumentException(
                $"Nominal code '{nominalCode}' is not a valid integer for {variantName} categories.",
                nameof(nominalCode));
        }

        if (value < minInclusive || value > maxInclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(nominalCode),
                nominalCode,
                $"Nominal code must be between {minInclusive} and {maxInclusive} for {variantName} categories.");
        }
    }
}
