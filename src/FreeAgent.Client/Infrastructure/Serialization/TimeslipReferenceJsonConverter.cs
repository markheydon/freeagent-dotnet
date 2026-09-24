using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="TimeslipReference"/> as a URI string.
/// </summary>
internal sealed class TimeslipReferenceJsonConverter : JsonConverter<TimeslipReference>
{
    public override TimeslipReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Timeslip reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing timeslip reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Timeslip reference URI cannot be null or empty.");
        }

        return TimeslipReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, TimeslipReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
