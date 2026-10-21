using System.Text.Json;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.DepreciationProfiles;

public class DepreciationProfileModelSerializationTests
{
    [Fact]
    public void Deserialize_DepreciationProfile_MapsStraightLineFields()
    {
        const string json = """
            {
              "method": "straight_line",
              "asset_life_years": 12,
              "frequency": "annually"
            }
            """;

        var profile = JsonSerializer.Deserialize<DepreciationProfile>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(profile);
        Assert.Equal(DepreciationMethod.StraightLine, profile!.Method);
        Assert.Equal(12, profile.AssetLifeYears);
        Assert.Equal(DepreciationFrequency.Annually, profile.Frequency);
    }

    [Fact]
    public void SerializeExpenseWritePayload_StraightLineDepreciationProfile_MapsNestedCapitalAsset()
    {
        var expense = new Expense
        {
            UserId = 1,
            CategoryNominalCode = "602",
            DatedOn = new DateOnly(2024, 2, 1),
            GrossValue = -20_000m,
            Description = "Capital asset expense",
            CapitalAssetPurchase = CapitalAssetPurchaseWrite.StraightLine(10, DepreciationFrequency.Annually)
        };

        var payload = ExpenseWritePayload.FromExpense(expense, FreeAgentEnvironment.Sandbox);
        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"capital_asset\"", json, StringComparison.Ordinal);
        Assert.Contains("\"depreciation_profile\"", json, StringComparison.Ordinal);
        Assert.Contains("\"method\":\"straight_line\"", json, StringComparison.Ordinal);
        Assert.Contains("\"asset_life_years\":10", json, StringComparison.Ordinal);
        Assert.Contains("\"frequency\":\"annually\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("depreciation_schedule", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeExpenseWritePayload_ReducingBalanceDepreciationProfile_OmitsStraightLineFields()
    {
        var expense = new Expense
        {
            UserId = 1,
            CategoryNominalCode = "602",
            DatedOn = new DateOnly(2024, 2, 1),
            GrossValue = -5_000m,
            CapitalAssetPurchase = CapitalAssetPurchaseWrite.ReducingBalance(25)
        };

        var payload = ExpenseWritePayload.FromExpense(expense, FreeAgentEnvironment.Sandbox);
        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"method\":\"reducing_balance\"", json, StringComparison.Ordinal);
        Assert.Contains("\"annual_depreciation_percentage\":25", json, StringComparison.Ordinal);
        Assert.DoesNotContain("asset_life_years", json, StringComparison.Ordinal);
    }

    [Fact]
    public void SerializeExpenseWritePayload_NoDepreciationProfile_SerialisesMethodOnly()
    {
        var expense = new Expense
        {
            UserId = 1,
            CategoryNominalCode = "602",
            DatedOn = new DateOnly(2024, 2, 1),
            GrossValue = -1_000m,
            CapitalAssetPurchase = CapitalAssetPurchaseWrite.NoDepreciation()
        };

        var payload = ExpenseWritePayload.FromExpense(expense, FreeAgentEnvironment.Sandbox);
        var json = JsonSerializer.Serialize(payload, FreeAgentJsonSerializer.Options);

        Assert.Contains("\"method\":\"no_depreciation\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("asset_life_years", json, StringComparison.Ordinal);
        Assert.DoesNotContain("annual_depreciation_percentage", json, StringComparison.Ordinal);
    }
}
