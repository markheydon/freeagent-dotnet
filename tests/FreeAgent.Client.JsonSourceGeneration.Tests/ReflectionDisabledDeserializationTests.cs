using System.Text.Json;
using FreeAgent.Client;

namespace FreeAgent.Client.JsonSourceGeneration.Tests;

public sealed class ReflectionDisabledDeserializationTests
{
    [Fact]
    public void Deserialize_OAuthTokenResponse_Succeeds()
    {
        var token = Deserialize<OAuthTokenResponse>("oauth-token-response.json");

        Assert.Equal("test-access-token", token.AccessToken);
        Assert.Equal("test-refresh-token", token.RefreshToken);
    }

    [Fact]
    public void Deserialize_Invoice_Succeeds()
    {
        var invoice = Deserialize<Invoice>("invoice-wire.json");

        Assert.Equal(3, invoice.ResourceId);
        Assert.Equal(InvoiceStatus.ScheduledToEmail, invoice.Status);
        Assert.Single(invoice.InvoiceItems!);
    }

    [Fact]
    public void Deserialize_InvoiceResponse_Envelope_Succeeds()
    {
        var json = $$"""
            { "invoice": {{File.ReadAllText(FixturePath("invoice-wire.json"))}} }
            """;

        var response = JsonSerializer.Deserialize<InvoiceResponse>(json, FreeAgentJsonSerialization.ApiSerializerOptions)
            ?? throw new InvalidOperationException("Invoice response was null.");

        Assert.NotNull(response.Invoice);
        Assert.Equal(3, response.Invoice!.ResourceId);
    }

    private static T Deserialize<T>(string fixtureName)
    {
        var json = File.ReadAllText(FixturePath(fixtureName));
        return JsonSerializer.Deserialize<T>(json, FreeAgentJsonSerialization.ApiSerializerOptions)
            ?? throw new InvalidOperationException($"Fixture {fixtureName} deserialised to null.");
    }

    private static string FixturePath(string fixtureName) =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", fixtureName);
}
