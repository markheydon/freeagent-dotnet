using System.Text.Json;
using System.Text.Json.Serialization;
namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="BillReference"/> as a URI string.
/// </summary>
internal sealed class BillReferenceJsonConverter : JsonConverter<BillReference>
{
    public override BillReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Bill reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing bill reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Bill reference URI cannot be null or empty.");
        }

        return BillReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, BillReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
