using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Services.Categories;

namespace FreeAgent.Client.Tests.Services.Categories;

public class CategoryResponseMapperTests
{
    [Fact]
    public void ToCollection_WhenOnlyGeneralCategoriesPresent_ReturnsCollection()
    {
        var response = new CategoryListResponse
        {
            GeneralCategories =
            [
                new Category { NominalCode = "051", Description = "Interest Received" }
            ]
        };

        var collection = CategoryResponseMapper.ToCollection(response);

        Assert.Single(collection.GeneralCategories);
        Assert.Empty(collection.IncomeCategories);
    }

    [Fact]
    public void ToCollection_WhenAllSetsMissing_ThrowsFreeAgentApiException()
    {
        var response = new CategoryListResponse();

        Assert.Throws<FreeAgentApiException>(() => CategoryResponseMapper.ToCollection(response));
    }

    [Fact]
    public void ToCategory_WhenSingleEnvelopePresent_ReturnsCategory()
    {
        var response = new CategorySingleResponse
        {
            IncomeCategory = new Category { NominalCode = "001", Description = "Sales" }
        };

        var category = CategoryResponseMapper.ToCategory(response);

        Assert.Equal("001", category.NominalCode);
    }

    [Fact]
    public void ToCategory_WhenNoEnvelopePresent_ThrowsFreeAgentApiException()
    {
        var response = new CategorySingleResponse();

        Assert.Throws<FreeAgentApiException>(() => CategoryResponseMapper.ToCategory(response));
    }

    [Fact]
    public void ToCategory_WhenMultipleEnvelopesPresent_ThrowsFreeAgentApiException()
    {
        var response = new CategorySingleResponse
        {
            IncomeCategory = new Category { NominalCode = "001" },
            GeneralCategory = new Category { NominalCode = "051" }
        };

        var exception = Assert.Throws<FreeAgentApiException>(() => CategoryResponseMapper.ToCategory(response));

        Assert.Contains("Multiple category envelopes", exception.Message, StringComparison.Ordinal);
    }
}
