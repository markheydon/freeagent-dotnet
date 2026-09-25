using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="EstimateReference"/> as a URI string.
/// </summary>
internal sealed class EstimateReferenceJsonConverter : JsonConverter<EstimateReference>
{
    public override EstimateReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Estimate reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing estimate reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Estimate reference URI cannot be null or empty.");
        }

        return EstimateReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, EstimateReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
