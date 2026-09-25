using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="StockItemReference"/> as a URI string.
/// </summary>
internal sealed class StockItemReferenceJsonConverter : JsonConverter<StockItemReference>
{
    public override StockItemReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Stock item reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing stock item reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Stock item reference URI cannot be null or empty.");
        }

        return StockItemReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, StockItemReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
