namespace FreeAgent.Client;

/// <summary>
/// Fail-fast checks for expense writes using constraints stated in the FreeAgent expenses docs.
/// </summary>
internal static class ExpenseWriteValidator
{
    internal static void ValidateForCreate(Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense);
        ValidateCommon(expense, nameof(expense));
    }

    internal static void ValidateForUpdate(Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense);
        ValidateCommon(expense, nameof(expense));
    }

    private static void ValidateCommon(Expense expense, string paramName)
    {
        if (expense.UserId is null)
        {
            throw new ArgumentException("User is required.", paramName);
        }

        if (expense.DatedOn is null)
        {
            throw new ArgumentException("Dated on is required.", paramName);
        }

        if (string.IsNullOrWhiteSpace(expense.Description))
        {
            throw new ArgumentException("Description is required.", paramName);
        }

        if (expense.IsMileageCategory)
        {
            if (expense.Mileage is null)
            {
                throw new ArgumentException("Mileage is required when IsMileageCategory is true.", paramName);
            }

            if (expense.VehicleType is null)
            {
                throw new ArgumentException("Vehicle type is required when IsMileageCategory is true.", paramName);
            }

            return;
        }

        if (string.IsNullOrWhiteSpace(expense.CategoryNominalCode))
        {
            throw new ArgumentException("Category nominal code is required when IsMileageCategory is false.", paramName);
        }

        if (expense.GrossValue is null)
        {
            throw new ArgumentException("Gross value is required when the category is not Mileage.", paramName);
        }

        if (RequiresRebillFactor(expense.RebillType) && expense.RebillFactor is null)
        {
            throw new ArgumentException("Rebill factor is required when rebill type is markup or price.", paramName);
        }

        ValidateStockCompanions(expense, paramName);
    }

    private static void ValidateStockCompanions(Expense expense, string paramName)
    {
        var stockItemExplicit = expense.StockItemIdBacking.IsExplicitlySet;
        var quantitySet = expense.StockAlteringQuantity is not null;

        if (!stockItemExplicit && !quantitySet)
        {
            return;
        }

        if (expense.StockItemId is null || expense.StockAlteringQuantity is null)
        {
            throw new ArgumentException(
                "Stock item and stock altering quantity must both be set when either is provided.",
                paramName);
        }
    }

    private static bool RequiresRebillFactor(BillRebillType? rebillType) =>
        rebillType is BillRebillType.Markup or BillRebillType.Price;
}
