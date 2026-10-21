using System.Text.Json;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Tests.Infrastructure.Serialization;

public class JsonStringEnumMemberNameCompatibleConverterTests
{
    [Fact]
    public void Read_UnknownWireValue_ReturnsUnknownMember()
    {
        const string json = "\"NotARealStatus\"";

        var status = JsonSerializer.Deserialize<InvoiceStatus>(json, FreeAgentJsonSerializer.Options);

        Assert.Equal(InvoiceStatus.Unknown, status);
    }

    [Fact]
    public void Write_UnknownMember_Throws()
    {
        var exception = Assert.Throws<JsonException>(() =>
            JsonSerializer.Serialize(InvoiceStatus.Unknown, FreeAgentJsonSerializer.Options));

        Assert.Contains("Cannot serialise unknown", exception.Message, StringComparison.Ordinal);
    }
}
