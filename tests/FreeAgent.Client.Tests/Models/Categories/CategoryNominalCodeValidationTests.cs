using FreeAgent.Client.Models.Categories;

namespace FreeAgent.Client.Tests.Models.Categories;

public class CategoryNominalCodeValidationTests
{
    [Theory]
    [InlineData("001")]
    [InlineData("047")]
    [InlineData("049")]
    public void CreateIncomeCategoryRequest_AcceptsCodesInDocumentedRange(string nominalCode)
    {
        var request = CreateIncomeCategoryRequest.Create("Income", nominalCode);

        Assert.Equal(nominalCode, request.NominalCode);
    }

    [Theory]
    [InlineData("050")]
    [InlineData("200")]
    [InlineData("000")]
    public void CreateIncomeCategoryRequest_RejectsCodesOutsideDocumentedRange(string nominalCode)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateIncomeCategoryRequest.Create("Income", nominalCode));

        Assert.Equal("nominalCode", exception.ParamName);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("49")]
    [InlineData("0212")]
    [InlineData("602-1")]
    [InlineData("abc")]
    [InlineData("21.2")]
    public void CreateIncomeCategoryRequest_RejectsNonThreeDigitCodes(string nominalCode)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            CreateIncomeCategoryRequest.Create("Income", nominalCode));

        Assert.Equal("nominalCode", exception.ParamName);
    }

    [Fact]
    public void CreateAdminExpensesCategoryRequest_RejectsCodeFromAnotherVariantRange()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                "Admin",
                "048",
                UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                allowableForTax: true));

        Assert.Equal("nominalCode", exception.ParamName);
    }

    [Theory]
    [InlineData("212")]
    [InlineData("399")]
    public void CreateAdminExpensesCategoryRequest_AcceptsCodesInDocumentedRange(string nominalCode)
    {
        var request = CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
            "Admin",
            nominalCode,
            UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
            allowableForTax: true);

        Assert.Equal(nominalCode, request.NominalCode);
    }

    [Theory]
    [InlineData("095")]
    [InlineData("999")]
    public void CreateCostOfSalesCategoryRequest_RejectsCodesOutsideDocumentedRange(string nominalCode)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                "Cost of sales",
                nominalCode,
                UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                allowableForTax: true));

        Assert.Equal("nominalCode", exception.ParamName);
        Assert.Contains("096", exception.Message, StringComparison.Ordinal);
        Assert.Contains("199", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateCostOfSalesCategoryRequest_RejectsCodeFromAnotherVariantRange()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                "Cost of sales",
                "200",
                UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                allowableForTax: true));

        Assert.Equal("nominalCode", exception.ParamName);
    }

    [Theory]
    [InlineData("672")]
    [InlineData("720")]
    public void CreateCurrentAssetCategoryRequest_AcceptsCodesInDocumentedRange(string nominalCode)
    {
        var request = CreateCurrentAssetCategoryRequest.Create(
            "Asset",
            nominalCode,
            CurrentAssetTaxReportingName.Debtors);

        Assert.Equal(nominalCode, request.NominalCode);
    }

    [Theory]
    [InlineData("670")]
    [InlineData("721")]
    public void CreateCurrentAssetCategoryRequest_RejectsCodesOutsideDocumentedRange(string nominalCode)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateCurrentAssetCategoryRequest.Create(
                "Asset",
                nominalCode,
                CurrentAssetTaxReportingName.Debtors));
    }

    [Theory]
    [InlineData("732")]
    [InlineData("780")]
    public void CreateLiabilitiesCategoryRequest_AcceptsCodesInDocumentedRange(string nominalCode)
    {
        var request = CreateLiabilitiesCategoryRequest.ForUkLimitedCompany(
            "Liability",
            nominalCode,
            UkLimitedCompanyLiabilitiesTaxReportingName.Creditors);

        Assert.Equal(nominalCode, request.NominalCode);
    }

    [Theory]
    [InlineData("922")]
    [InlineData("960")]
    public void CreateEquityCategoryRequest_AcceptsCodesInDocumentedRange(string nominalCode)
    {
        var request = CreateEquityCategoryRequest.Create("Equity", nominalCode);

        Assert.Equal(nominalCode, request.NominalCode);
    }

    [Fact]
    public void UpdateAdminExpensesCategoryRequest_RejectsCodeOutsideDocumentedRange()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            UpdateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                "Admin",
                "048",
                UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));

        Assert.Equal("nominalCode", exception.ParamName);
        Assert.Contains("200", exception.Message, StringComparison.Ordinal);
        Assert.Contains("399", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UpdateCostOfSalesCategoryRequest_RejectsCodeOutsideDocumentedRange()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            UpdateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                "Cost of sales",
                "999",
                UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));

        Assert.Equal("nominalCode", exception.ParamName);
        Assert.Contains("096", exception.Message, StringComparison.Ordinal);
        Assert.Contains("199", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("../001")]
    [InlineData("001/extra")]
    public void ValidatePathSegment_RejectsInvalidPathCharacters(string nominalCode)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            CategoryNominalCodeValidator.ValidatePathSegment(nominalCode));

        Assert.Equal("nominalCode", exception.ParamName);
    }

    [Fact]
    public void ValidatePathSegment_AllowsSubAccountCodes()
    {
        CategoryNominalCodeValidator.ValidatePathSegment("602-1");
    }
}
