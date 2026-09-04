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

        Category? category = null;
        var envelopeCount = 0;

        if (response.AdminExpensesCategory is not null)
        {
            category = response.AdminExpensesCategory;
            envelopeCount++;
        }

        if (response.CostOfSalesCategory is not null)
        {
            category = response.CostOfSalesCategory;
            envelopeCount++;
        }

        if (response.IncomeCategory is not null)
        {
            category = response.IncomeCategory;
            envelopeCount++;
        }

        if (response.GeneralCategory is not null)
        {
            category = response.GeneralCategory;
            envelopeCount++;
        }

        if (envelopeCount == 0)
        {
            throw new FreeAgentApiException("Category data missing from API response");
        }

        if (envelopeCount > 1)
        {
            throw new FreeAgentApiException("Multiple category envelopes present in API response");
        }

        return category!;
    }
}
