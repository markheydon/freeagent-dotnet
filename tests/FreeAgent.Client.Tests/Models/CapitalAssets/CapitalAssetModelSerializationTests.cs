using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.CapitalAssets;

public class CapitalAssetModelSerializationTests
{
    [Fact]
    public void DeserializeCapitalAsset_MapsDatesTimestampsAndDepreciationProfile()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/capital_assets/3",
              "description": "Workstation",
              "asset_life_years": 2,
              "depreciation_profile": {
                "method": "straight_line",
                "asset_life_years": 2,
                "frequency": "monthly"
              },
              "asset_type": "Computer Equipment Purchase",
              "purchased_on": "2014-10-07",
              "disposed_on": "2015-12-07",
              "created_at": "2014-10-07T10:41:21.000Z",
              "updated_at": "2014-10-07T10:41:43.000Z"
            }
            """;

        var item = JsonSerializer.Deserialize<CapitalAsset>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal(3, item!.ResourceId);
        Assert.Equal("Workstation", item.Description);
        Assert.Equal(2, item.AssetLifeYears);
        Assert.Equal(DepreciationMethod.StraightLine, item.DepreciationProfile!.Method);
        Assert.Equal(2, item.DepreciationProfile.AssetLifeYears);
        Assert.Equal(DepreciationFrequency.Monthly, item.DepreciationProfile.Frequency);
        Assert.Equal("Computer Equipment Purchase", item.AssetType);
        Assert.Equal(new DateOnly(2014, 10, 7), item.PurchasedOn);
        Assert.Equal(new DateOnly(2015, 12, 7), item.DisposedOn);
        Assert.Equal(new DateTimeOffset(2014, 10, 7, 10, 41, 21, TimeSpan.Zero), item.CreatedAt);
    }

    [Fact]
    public void DeserializeCapitalAssetHistory_ParsesLinkTargetsAndDecimalStrings()
    {
        const string json = """
            {
              "type": "disposal",
              "description": "Disposal",
              "date": "2020-07-03",
              "value": "-701.5",
              "tax_value": "-741.67",
              "link": "https://api.freeagent.com/v2/bank_transaction_explanations/444"
            }
            """;

        var entry = JsonSerializer.Deserialize<CapitalAssetHistoryEntry>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(entry);
        Assert.Equal(CapitalAssetHistoryEventType.Disposal, entry!.Type);
        Assert.Equal(-701.5m, entry.Value);
        Assert.Equal(-741.67m, entry.TaxValue);
        Assert.Equal(444, entry.BankTransactionExplanationId);
        Assert.Null(entry.ExpenseId);
        Assert.Null(entry.BillId);
    }

    [Fact]
    public void DeserializeCapitalAssetHistory_ParsesExpenseLink()
    {
        const string json = """
            {
              "type": "purchase",
              "link": "https://api.freeagent.com/v2/expenses/30"
            }
            """;

        var entry = JsonSerializer.Deserialize<CapitalAssetHistoryEntry>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(entry);
        Assert.Equal(CapitalAssetHistoryEventType.Purchase, entry!.Type);
        Assert.Equal(30, entry.ExpenseId);
    }
}
