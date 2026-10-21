using System.Text.Json;
using System.Text.Json.Serialization;
namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="PropertyReference"/> as a URI string.
/// </summary>
internal sealed class PropertyReferenceJsonConverter : JsonConverter<PropertyReference>
{
    public override PropertyReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Property reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing property reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Property reference URI cannot be null or empty.");
        }

        return PropertyReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, PropertyReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
