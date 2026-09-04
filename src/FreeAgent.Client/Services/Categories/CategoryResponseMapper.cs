using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Services.Categories;

internal static class CategoryResponseMapper
{
    public static CategorySets ToCollection(CategoryListResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.AdminExpensesCategories is null
            && response.CostOfSalesCategories is null
            && response.IncomeCategories is null
            && response.GeneralCategories is null)
        {
            throw new FreeAgentApiException("Categories data missing from API response");
        }

        return new CategorySets
        {
            AdminExpensesCategories = response.AdminExpensesCategories ?? [],
            CostOfSalesCategories = response.CostOfSalesCategories ?? [],
            IncomeCategories = response.IncomeCategories ?? [],
            GeneralCategories = response.GeneralCategories ?? []
        };
    }

    public static Category ToCategory(CategorySingleResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var category = response.AdminExpensesCategory
            ?? response.CostOfSalesCategory
            ?? response.IncomeCategory
            ?? response.GeneralCategory;

        if (category is null)
        {
            throw new FreeAgentApiException("Category data missing from API response");
        }

        return category;
    }
}
