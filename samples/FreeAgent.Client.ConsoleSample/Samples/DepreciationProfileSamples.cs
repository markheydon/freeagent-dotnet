using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Depreciation profile nested payload examples.
/// </summary>
[ConsoleSamples(Category = "Depreciation profiles")]
internal sealed class DepreciationProfileSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "Deserialize straight line depreciation profile")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task DeserializeStraightLineProfileAsync()
    {
        _ = context.Client;
        const string json = """
            {
              "method": "straight_line",
              "asset_life_years": 10,
              "frequency": "annually"
            }
            """;

        var profile = JsonSerializer.Deserialize<DepreciationProfile>(json, FreeAgentJsonSerializer.Options);
        SampleOutput.WriteHeader("Straight line DepreciationProfile");
        SampleOutput.WriteField("Method", profile?.Method);
        SampleOutput.WriteField("Asset life years", profile?.AssetLifeYears);
        SampleOutput.WriteField("Frequency", profile?.Frequency);
        return Task.CompletedTask;
    }

    [ConsoleSample(Name = "Deserialize reducing balance depreciation profile")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task DeserializeReducingBalanceProfileAsync()
    {
        _ = context.Client;
        const string json = """
            {
              "method": "reducing_balance",
              "annual_depreciation_percentage": 25,
              "frequency": "monthly"
            }
            """;

        var profile = JsonSerializer.Deserialize<DepreciationProfile>(json, FreeAgentJsonSerializer.Options);
        SampleOutput.WriteHeader("Reducing balance DepreciationProfile");
        SampleOutput.WriteField("Method", profile?.Method);
        SampleOutput.WriteField("Annual depreciation %", profile?.AnnualDepreciationPercentage);
        SampleOutput.WriteField("Frequency", profile?.Frequency);
        return Task.CompletedTask;
    }

    [ConsoleSample(Name = "Build no depreciation capital asset purchase helper")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private Task BuildNoDepreciationPurchaseAsync()
    {
        _ = context.Client;
        _ = CapitalAssetPurchaseWrite.NoDepreciation();
        SampleOutput.WriteHeader("CapitalAssetPurchaseWrite.NoDepreciation()");
        SampleOutput.WriteField(
            "Usage",
            "Assign to Expense.CapitalAssetPurchase or pass to CreateWithDepreciationProfile on bank transaction explanations.");
        return Task.CompletedTask;
    }
}
