using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Properties;

namespace FreeAgent.Client.Tests.Models.Properties;

public class LandlordPropertyModelSerializationTests
{
    [Fact]
    public void DeserialisesListItemFromDocsExample()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/properties/5",
              "address1": "Sherlock Holmes Museum",
              "address2": "221B Baker Street",
              "address3": "Marylebone",
              "town": "London",
              "region": "City of London",
              "postcode": "NW1 6XE",
              "country": "United Kingdom"
            }
            """;

        var property = JsonSerializer.Deserialize<LandlordProperty>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(property);
        Assert.Equal(5, property.ResourceId);
        Assert.Equal("Sherlock Holmes Museum", property.Address1);
        Assert.Equal("NW1 6XE", property.Postcode);
    }
}
