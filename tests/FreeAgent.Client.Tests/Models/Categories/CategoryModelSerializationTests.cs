using System.Text.Json;
using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Tests.Models.Categories;

public class CategoryModelSerializationTests
{
    [Theory]
    [InlineData(CategoryAutoSalesTaxRate.OutsideOfTheScopeOfVat, "Outside of the scope of VAT")]
    [InlineData(CategoryAutoSalesTaxRate.ZeroRate, "Zero rate")]
    [InlineData(CategoryAutoSalesTaxRate.ReducedRate, "Reduced rate")]
    [InlineData(CategoryAutoSalesTaxRate.StandardRate, "Standard rate")]
    [InlineData(CategoryAutoSalesTaxRate.Exempt, "Exempt")]
    public void CategoryAutoSalesTaxRate_RoundTripsWireValue(CategoryAutoSalesTaxRate value, string wireValue)
    {
        var json = JsonSerializer.Serialize(new Category { AutoSalesTaxRate = value });
        using var document = JsonDocument.Parse(json);

        Assert.Equal(wireValue, document.RootElement.GetProperty("auto_sales_tax_rate").GetString());

        var deserialized = JsonSerializer.Deserialize<Category>(json);
        Assert.Equal(value, deserialized!.AutoSalesTaxRate);
    }

    [Theory]
    [InlineData(CategoryGroup.Income, "income")]
    [InlineData(CategoryGroup.CostOfSales, "cost_of_sales")]
    [InlineData(CategoryGroup.AdminExpenses, "admin_expenses")]
    [InlineData(CategoryGroup.CurrentAssets, "current_assets")]
    [InlineData(CategoryGroup.Liabilities, "liabilities")]
    [InlineData(CategoryGroup.Equities, "equities")]
    public void CategoryGroup_RoundTripsWireValue(CategoryGroup value, string wireValue)
    {
        var json = JsonSerializer.Serialize(new CategoryWritePayload { CategoryGroup = value });
        using var document = JsonDocument.Parse(json);

        Assert.Equal(wireValue, document.RootElement.GetProperty("category_group").GetString());
    }
}
