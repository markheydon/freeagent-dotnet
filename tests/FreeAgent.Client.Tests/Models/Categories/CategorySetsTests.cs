using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Tests.Models.Categories;

public class CategorySetsTests
{
    [Fact]
    public void AllCategories_ReturnsSameCachedInstanceOnRepeatedAccess()
    {
        var collection = new CategorySets
        {
            IncomeCategories =
            [
                new Category { NominalCode = "001" }
            ],
            AdminExpensesCategories =
            [
                new Category { NominalCode = "285" }
            ]
        };

        var first = collection.AllCategories;
        var second = collection.AllCategories;

        Assert.Same(first, second);
        Assert.Equal(2, first.Count);
    }
}
