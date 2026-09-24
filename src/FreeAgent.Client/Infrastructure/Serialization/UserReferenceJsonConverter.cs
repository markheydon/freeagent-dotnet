using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="UserReference"/> as a URI string.
/// </summary>
internal sealed class UserReferenceJsonConverter : JsonConverter<UserReference>
{
    public override UserReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("User reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing user reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("User reference URI cannot be null or empty.");
        }

        return UserReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, UserReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
