using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.HirePurchases;

namespace FreeAgent.Client.Tests.Models.HirePurchases;

public class HirePurchaseModelSerializationTests
{
    [Fact]
    public void Deserialise_ListEnvelope_MapsLinkedResourceIds()
    {
        const string json = """
        {
          "hire_purchases": [
            {
              "url": "https://api.freeagent.com/v2/hire_purchases/1",
              "description": "My hire purchase bill",
              "bill": "https://api.freeagent.com/v2/bills/1",
              "liabilities_over_one_year_category": "https://api.freeagent.com/v2/categories/793-1",
              "liabilities_under_one_year_category": "https://api.freeagent.com/v2/categories/792-1"
            }
          ]
        }
        """;

        var response = JsonSerializer.Deserialize<HirePurchasesResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.HirePurchases);
        var hirePurchase = Assert.Single(response.HirePurchases);
        Assert.Equal("My hire purchase bill", hirePurchase.Description);
        Assert.Equal(1, hirePurchase.BillId);
        Assert.Equal("793-1", hirePurchase.LiabilitiesOverOneYearCategoryNominalCode);
        Assert.Equal("792-1", hirePurchase.LiabilitiesUnderOneYearCategoryNominalCode);
    }

    [Fact]
    public void Deserialise_SingleEnvelope_MapsFields()
    {
        const string json = """
        {
          "hire_purchase": {
            "url": "https://api.freeagent.com/v2/hire_purchases/5",
            "description": "Vehicle finance",
            "bill": "https://api.freeagent.com/v2/bills/9",
            "liabilities_over_one_year_category": "https://api.freeagent.com/v2/categories/793-1",
            "liabilities_under_one_year_category": "https://api.freeagent.com/v2/categories/792-1"
          }
        }
        """;

        var response = JsonSerializer.Deserialize<HirePurchaseResponse>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(response?.HirePurchase);
        Assert.Equal(5, response.HirePurchase.ResourceId);
        Assert.Equal(9, response.HirePurchase.BillId);
    }
}
