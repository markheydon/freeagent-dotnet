using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Models.EcMoss;

public class EcMossModelSerializationTests
{
    [Fact]
    public void DeserializeInvoice_PlaceOfSupply_MapsEcMossEnum()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/invoices/1",
              "place_of_supply": "Czech Republic"
            }
            """;

        var invoice = JsonSerializer.Deserialize<Invoice>(json, FreeAgentJsonSerializer.Options);

        Assert.Equal(EcMossPlaceOfSupply.CzechRepublic, invoice!.PlaceOfSupply);
    }

    [Fact]
    public void SerializeEcMossPlaceOfSupply_UsesDocumentedWireName()
    {
        var json = JsonSerializer.Serialize(EcMossPlaceOfSupply.CzechRepublic, FreeAgentJsonSerializer.Options);

        Assert.Equal("\"Czech Republic\"", json);
    }
}
