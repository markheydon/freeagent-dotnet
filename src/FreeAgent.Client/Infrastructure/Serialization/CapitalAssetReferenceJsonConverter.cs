using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="CapitalAssetReference"/> as a URI string.
/// </summary>
internal sealed class CapitalAssetReferenceJsonConverter : JsonConverter<CapitalAssetReference>
{
    public override CapitalAssetReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Capital asset reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing capital asset reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Capital asset reference URI cannot be null or empty.");
        }

        return CapitalAssetReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, CapitalAssetReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
