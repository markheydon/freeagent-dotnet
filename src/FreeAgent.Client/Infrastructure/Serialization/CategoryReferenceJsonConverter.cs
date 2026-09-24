using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="CategoryReference"/> as a URI string.
/// </summary>
internal sealed class CategoryReferenceJsonConverter : JsonConverter<CategoryReference>
{
    public override CategoryReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Category reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing category reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Category reference URI cannot be null or empty.");
        }

        return CategoryReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, CategoryReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
