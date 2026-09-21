using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Tests.Models.Categories;

public class CategoryResourceIdTests
{
    [Fact]
    public void ResourceId_PrefersNominalCodeWhenNumeric()
    {
        var category = new Category
        {
            NominalCode = "001",
            Url = "https://api.freeagent.com/v2/categories/999"
        };

        Assert.Equal(1, category.ResourceId);
    }

    [Fact]
    public void ResourceId_FallsBackToUrlWhenNominalCodeMissing()
    {
        var category = new Category
        {
            Url = "https://api.freeagent.com/v2/categories/101"
        };

        Assert.Equal(101, category.ResourceId);
    }
}
