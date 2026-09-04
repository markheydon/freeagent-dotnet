using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Services.Categories;

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

    [Fact]
    public void UkLimitedCompanyCostOfSalesTaxReportingName_UsesWireKey()
    {
        Assert.Equal(
            "purchases",
            EnumWireValue.Get(UkLimitedCompanyCostOfSalesTaxReportingName.Purchases));
    }

    [Fact]
    public void CreateIncomeCategoryRequest_MapsToIncomeCategoryGroupWireValue()
    {
        var payload = CategoryWritePayloadMapper.FromCreate(
            CreateIncomeCategoryRequest.Create("Custom Income Category", "047"));

        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);
        using var document = JsonDocument.Parse(json);

        Assert.Equal("income", document.RootElement.GetProperty("category_group").GetString());
        Assert.Equal("047", document.RootElement.GetProperty("nominal_code").GetString());
        Assert.False(document.RootElement.TryGetProperty("tax_reporting_name", out _));
    }

    [Fact]
    public void CreateCostOfSalesCategoryRequest_ForUkLimitedCompany_MapsSpendingFields()
    {
        var payload = CategoryWritePayloadMapper.FromCreate(
            CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                "Custom Cost of Sales Category",
                "101",
                UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));

        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);
        using var document = JsonDocument.Parse(json);

        Assert.Equal("cost_of_sales", document.RootElement.GetProperty("category_group").GetString());
        Assert.Equal("purchases", document.RootElement.GetProperty("tax_reporting_name").GetString());
        Assert.True(document.RootElement.GetProperty("allowable_for_tax").GetBoolean());
    }

    [Fact]
    public void CreateAdminExpensesCategoryRequest_ForUkLimitedCompany_MapsSpendingFields()
    {
        var payload = CategoryWritePayloadMapper.FromCreate(
            CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                "Custom Admin Expenses Category",
                "212",
                UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));

        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"tax_reporting_name\":\"computer_software_costs\"", json, StringComparison.Ordinal);
    }
}
