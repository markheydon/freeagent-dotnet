using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Services.Categories;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Categories;

public class CategoryServiceTests
{
    [Fact]
    public async Task GetCategoriesAsync_ReturnsAllSets()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/categories", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            Assert.DoesNotContain("sub_accounts", request.RequestUri.Query, StringComparison.Ordinal);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "admin_expenses_categories": [
                    {
                      "url": "https://api.freeagent.com/v2/categories/285",
                      "description": "Accommodation and Meals",
                      "nominal_code": "285",
                      "allowable_for_tax": true,
                      "tax_reporting_name": "Travel and subsistence expenses",
                      "auto_sales_tax_rate": "Standard rate"
                    }
                  ],
                  "cost_of_sales_categories": [],
                  "income_categories": [
                    {
                      "url": "https://api.freeagent.com/v2/categories/001",
                      "description": "Sales",
                      "nominal_code": "001",
                      "auto_sales_tax_rate": "Standard rate"
                    }
                  ],
                  "general_categories": []
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var categories = await service.GetCategoriesAsync();

        Assert.Single(categories.AdminExpensesCategories);
        Assert.Empty(categories.CostOfSalesCategories);
        Assert.Single(categories.IncomeCategories);
        Assert.Empty(categories.GeneralCategories);
        Assert.Equal(2, categories.AllCategories.Count);
        Assert.Equal("285", categories.AdminExpensesCategories[0].NominalCode);
        Assert.Equal(CategoryAutoSalesTaxRate.StandardRate, categories.AdminExpensesCategories[0].AutoSalesTaxRate);
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenIncludeSubAccounts_AddsQueryParameter()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Contains("sub_accounts=true", request.RequestUri!.Query, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "admin_expenses_categories": [],
                  "cost_of_sales_categories": [],
                  "income_categories": [],
                  "general_categories": []
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        await service.GetCategoriesAsync(includeSubAccounts: true);
    }

    [Fact]
    public async Task GetCategoryAsync_ReturnsCategory()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/categories/001", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "income_categories": {
                    "url": "https://api.freeagent.com/v2/categories/001",
                    "description": "Sales",
                    "nominal_code": "001",
                    "auto_sales_tax_rate": "Standard rate"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var category = await service.GetCategoryAsync("001");

        Assert.Equal("Sales", category.Description);
        Assert.Equal(CategoryAutoSalesTaxRate.StandardRate, category.AutoSalesTaxRate);
    }

    [Fact]
    public async Task GetCategoryAsync_WithSubAccountNominalCode_UsesPathSegment()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.EndsWith("/categories/602-1", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "cost_of_sales_categories": {
                    "url": "https://api.freeagent.com/v2/categories/602-1",
                    "description": "Sub account",
                    "nominal_code": "602-1"
                  }
                }
                """)
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        using var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        var service = new CategoryService(client);

        var category = await service.GetCategoryAsync("602-1");

        Assert.Equal("602-1", category.NominalCode);
    }

    [Fact]
    public async Task CreateIncomeCategoryAsync_PostsIncomeCategoryEnvelope()
    {
        var handler = CreateCategoryPostHandler(
            body =>
            {
                Assert.Contains("\"category_group\":\"income\"", body, StringComparison.Ordinal);
                Assert.Contains("\"nominal_code\":\"047\"", body, StringComparison.Ordinal);
                Assert.DoesNotContain("tax_reporting_name", body, StringComparison.Ordinal);
            },
            """
            {
              "income_categories": {
                "url": "https://api.freeagent.com/v2/categories/047",
                "description": "Custom Income Category",
                "nominal_code": "047"
              }
            }
            """);

        var created = await CreateService(handler).CreateIncomeCategoryAsync(
            CreateIncomeCategoryRequest.Create("Custom Income Category", "047"));

        Assert.Equal("047", created.NominalCode);
    }

    [Fact]
    public async Task CreateCostOfSalesCategoryAsync_PostsSpendingCategoryFields()
    {
        var handler = CreateCategoryPostHandler(
            body =>
            {
                Assert.Contains("\"category_group\":\"cost_of_sales\"", body, StringComparison.Ordinal);
                Assert.Contains("\"tax_reporting_name\":\"purchases\"", body, StringComparison.Ordinal);
                Assert.Contains("\"allowable_for_tax\":true", body, StringComparison.Ordinal);
            },
            """
            {
              "cost_of_sales_categories": {
                "url": "https://api.freeagent.com/v2/categories/101",
                "description": "Custom Cost of Sales Category",
                "nominal_code": "101"
              }
            }
            """);

        var created = await CreateService(handler).CreateCostOfSalesCategoryAsync(
            CreateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                "Custom Cost of Sales Category",
                "101",
                UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));

        Assert.Equal("101", created.NominalCode);
    }

    [Fact]
    public async Task CreateAdminExpensesCategoryAsync_PostsSpendingCategoryFields()
    {
        var handler = CreateCategoryPostHandler(
            body => Assert.Contains("\"tax_reporting_name\":\"computer_software_costs\"", body, StringComparison.Ordinal),
            """
            {
              "admin_expenses_categories": {
                "url": "https://api.freeagent.com/v2/categories/212",
                "description": "Custom Admin Expenses Category",
                "nominal_code": "212"
              }
            }
            """);

        await CreateService(handler).CreateAdminExpensesCategoryAsync(
            CreateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                "Custom Admin Expenses Category",
                "212",
                UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));
    }

    [Fact]
    public async Task CreateCurrentAssetCategoryAsync_PostsCurrentAssetFields()
    {
        var handler = CreateCategoryPostHandler(
            body =>
            {
                Assert.Contains("\"category_group\":\"current_assets\"", body, StringComparison.Ordinal);
                Assert.Contains("\"tax_reporting_name\":\"debtors\"", body, StringComparison.Ordinal);
            },
            """
            {
              "general_categories": {
                "url": "https://api.freeagent.com/v2/categories/672",
                "description": "Custom Assets Category",
                "nominal_code": "672"
              }
            }
            """);

        await CreateService(handler).CreateCurrentAssetCategoryAsync(
            CreateCurrentAssetCategoryRequest.Create(
                "Custom Assets Category",
                "672",
                CurrentAssetTaxReportingName.Debtors));
    }

    [Fact]
    public async Task CreateLiabilitiesCategoryAsync_PostsLiabilitiesFields()
    {
        var handler = CreateCategoryPostHandler(
            body =>
            {
                Assert.Contains("\"category_group\":\"liabilities\"", body, StringComparison.Ordinal);
                Assert.Contains("\"tax_reporting_name\":\"creditors\"", body, StringComparison.Ordinal);
            },
            """
            {
              "general_categories": {
                "url": "https://api.freeagent.com/v2/categories/732",
                "description": "Custom Liabilities Category",
                "nominal_code": "732"
              }
            }
            """);

        await CreateService(handler).CreateLiabilitiesCategoryAsync(
            CreateLiabilitiesCategoryRequest.ForOtherCompanyTypes(
                "Custom Liabilities Category",
                "732",
                OtherCompanyLiabilitiesTaxReportingName.Creditors));
    }

    [Fact]
    public async Task CreateEquityCategoryAsync_PostsEquityCategoryEnvelope()
    {
        var handler = CreateCategoryPostHandler(
            body =>
            {
                Assert.Contains("\"category_group\":\"equities\"", body, StringComparison.Ordinal);
                Assert.Contains("\"nominal_code\":\"922\"", body, StringComparison.Ordinal);
                Assert.DoesNotContain("tax_reporting_name", body, StringComparison.Ordinal);
            },
            """
            {
              "general_categories": {
                "url": "https://api.freeagent.com/v2/categories/922",
                "description": "Custom Equity Category",
                "nominal_code": "922"
              }
            }
            """);

        var created = await CreateService(handler).CreateEquityCategoryAsync(
            CreateEquityCategoryRequest.Create("Custom Equity Category", "922"));

        Assert.Equal("922", created.NominalCode);
    }

    [Fact]
    public async Task UpdateIncomeCategoryAsync_PutsIncomeCategoryEnvelope()
    {
        var handler = CreateCategoryPutHandler(
            "047",
            body =>
            {
                Assert.Contains("\"description\":\"Renamed\"", body, StringComparison.Ordinal);
                Assert.DoesNotContain("category_group", body, StringComparison.Ordinal);
            },
            """
            {
              "income_categories": {
                "url": "https://api.freeagent.com/v2/categories/047",
                "description": "Renamed",
                "nominal_code": "047"
              }
            }
            """);

        var updated = await CreateService(handler).UpdateIncomeCategoryAsync(
            "047",
            UpdateIncomeCategoryRequest.Create("Renamed", "047"));

        Assert.Equal("Renamed", updated.Description);
    }

    [Fact]
    public async Task UpdateCostOfSalesCategoryAsync_PutsSpendingCategoryFields()
    {
        var handler = CreateCategoryPutHandler(
            "101",
            body => Assert.Contains("\"tax_reporting_name\":\"purchases\"", body, StringComparison.Ordinal),
            """
            {
              "cost_of_sales_categories": {
                "url": "https://api.freeagent.com/v2/categories/101",
                "description": "Updated Cost of Sales Category",
                "nominal_code": "101"
              }
            }
            """);

        await CreateService(handler).UpdateCostOfSalesCategoryAsync(
            "101",
            UpdateCostOfSalesCategoryRequest.ForUkLimitedCompany(
                "Updated Cost of Sales Category",
                "101",
                UkLimitedCompanyCostOfSalesTaxReportingName.Purchases,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));
    }

    [Fact]
    public async Task UpdateAdminExpensesCategoryAsync_PutsSpendingCategoryFields()
    {
        var handler = CreateCategoryPutHandler(
            "212",
            body => Assert.Contains("\"tax_reporting_name\":\"computer_software_costs\"", body, StringComparison.Ordinal),
            """
            {
              "admin_expenses_categories": {
                "url": "https://api.freeagent.com/v2/categories/212",
                "description": "Updated Admin Expenses Category",
                "nominal_code": "212"
              }
            }
            """);

        await CreateService(handler).UpdateAdminExpensesCategoryAsync(
            "212",
            UpdateAdminExpensesCategoryRequest.ForUkLimitedCompany(
                "Updated Admin Expenses Category",
                "212",
                UkLimitedCompanyAdminExpensesTaxReportingName.ComputerSoftwareCosts,
                allowableForTax: true,
                autoSalesTaxRate: CategoryAutoSalesTaxRate.StandardRate));
    }

    [Fact]
    public async Task UpdateCurrentAssetCategoryAsync_PutsCurrentAssetFields()
    {
        var handler = CreateCategoryPutHandler(
            "672",
            body => Assert.Contains("\"tax_reporting_name\":\"debtors\"", body, StringComparison.Ordinal),
            """
            {
              "general_categories": {
                "url": "https://api.freeagent.com/v2/categories/672",
                "description": "Updated Assets Category",
                "nominal_code": "672"
              }
            }
            """);

        await CreateService(handler).UpdateCurrentAssetCategoryAsync(
            "672",
            UpdateCurrentAssetCategoryRequest.Create(
                "Updated Assets Category",
                "672",
                CurrentAssetTaxReportingName.Debtors));
    }

    [Fact]
    public async Task UpdateLiabilitiesCategoryAsync_PutsLiabilitiesFields()
    {
        var handler = CreateCategoryPutHandler(
            "732",
            body => Assert.Contains("\"tax_reporting_name\":\"creditors\"", body, StringComparison.Ordinal),
            """
            {
              "general_categories": {
                "url": "https://api.freeagent.com/v2/categories/732",
                "description": "Updated Liabilities Category",
                "nominal_code": "732"
              }
            }
            """);

        await CreateService(handler).UpdateLiabilitiesCategoryAsync(
            "732",
            UpdateLiabilitiesCategoryRequest.ForUkLimitedCompany(
                "Updated Liabilities Category",
                "732",
                UkLimitedCompanyLiabilitiesTaxReportingName.Creditors));
    }

    [Fact]
    public async Task UpdateEquityCategoryAsync_PutsEquityCategoryEnvelope()
    {
        var handler = CreateCategoryPutHandler(
            "922",
            body => Assert.DoesNotContain("tax_reporting_name", body, StringComparison.Ordinal),
            """
            {
              "general_categories": {
                "url": "https://api.freeagent.com/v2/categories/922",
                "description": "Updated Equity Category",
                "nominal_code": "922"
              }
            }
            """);

        await CreateService(handler).UpdateEquityCategoryAsync(
            "922",
            UpdateEquityCategoryRequest.Create("Updated Equity Category", "922"));
    }

    [Fact]
    public async Task DeleteCategoryAsync_SendsDelete()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.EndsWith("/categories/047", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        await CreateService(handler).DeleteCategoryAsync("047");
    }

    [Fact]
    public async Task GetCategoriesAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        await Assert.ThrowsAsync<FreeAgentApiException>(() => CreateService(handler).GetCategoriesAsync());
    }

    [Fact]
    public async Task GetCategoryAsync_WhenPayloadMissing_ThrowsFreeAgentApiException()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "income_categories": null }""")
        });

        await Assert.ThrowsAsync<FreeAgentApiException>(() => CreateService(handler).GetCategoryAsync("001"));
    }

    [Fact]
    public async Task GetCategoriesAsync_DeserializesAllowableForTaxFromString()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""
            {
              "admin_expenses_categories": [
                {
                  "url": "https://api.freeagent.com/v2/categories/213",
                  "description": "Custom Admin Expenses Category",
                  "nominal_code": "213",
                  "allowable_for_tax": "true",
                  "auto_sales_tax_rate": "Standard rate"
                }
              ],
              "cost_of_sales_categories": [],
              "income_categories": [],
              "general_categories": []
            }
            """)
        });

        var categories = await CreateService(handler).GetCategoriesAsync();

        Assert.True(categories.AdminExpensesCategories[0].AllowableForTax);
    }

    private static CategoryService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new CategoryService(client);
    }

    private static QueueHttpMessageHandler CreateCategoryPostHandler(Action<string> assertBody, string responseJson) =>
        new(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.EndsWith("/categories", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Contains("\"category\"", body, StringComparison.Ordinal);
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(responseJson)
            };
        });

    private static QueueHttpMessageHandler CreateCategoryPutHandler(string nominalCode, Action<string> assertBody, string responseJson) =>
        new(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith($"/categories/{nominalCode}", request.RequestUri!.AbsolutePath, StringComparison.Ordinal);
            var body = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            assertBody(body);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson)
            };
        });
}
