using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class PurchaseSalesTaxRateSerializationTests
{
    [Fact]
    public void SerializeAuto_WritesAutoString()
    {
        var json = JsonSerializer.Serialize(PurchaseSalesTaxRate.Auto, FreeAgentJsonSerializer.Options);

        Assert.Equal("\"Auto\"", json);
    }

    [Fact]
    public void DeserializeAuto_ReadsAutoString()
    {
        var rate = JsonSerializer.Deserialize<PurchaseSalesTaxRate>("\"Auto\"", FreeAgentJsonSerializer.Options);

        Assert.True(rate.IsAuto);
        Assert.Null(rate.Rate);
    }

    [Fact]
    public void DeserializePercentage_ReadsNumericString()
    {
        var rate = JsonSerializer.Deserialize<PurchaseSalesTaxRate>("\"20\"", FreeAgentJsonSerializer.Options);

        Assert.False(rate.IsAuto);
        Assert.Equal(20m, rate.Rate);
    }
}
