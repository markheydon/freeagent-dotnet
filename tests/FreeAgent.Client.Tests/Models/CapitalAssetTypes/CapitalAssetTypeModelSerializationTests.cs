using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.CapitalAssetTypes;

public class CapitalAssetTypeModelSerializationTests
{
    [Fact]
    public void DeserializeCapitalAssetType_MapsTimestamps()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/capital_asset_types/397",
              "name": "Computer Equipment",
              "system_default": true,
              "created_at": "2020-04-22T11:32:24.000Z",
              "updated_at": "2020-04-22T11:32:24.000Z"
            }
            """;

        var item = JsonSerializer.Deserialize<CapitalAssetType>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(item);
        Assert.Equal("Computer Equipment", item!.Name);
        Assert.True(item.SystemDefault);
        Assert.Equal(397, item.ResourceId);
        Assert.Equal(new DateTimeOffset(2020, 4, 22, 11, 32, 24, TimeSpan.Zero), item.CreatedAt);
    }
}
