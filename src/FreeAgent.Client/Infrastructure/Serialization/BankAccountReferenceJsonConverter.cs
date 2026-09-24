using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises <see cref="BankAccountReference"/> as a URI string.
/// </summary>
internal sealed class BankAccountReferenceJsonConverter : JsonConverter<BankAccountReference>
{
    public override BankAccountReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Bank account reference cannot be null.");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing bank account reference.");
        }

        var uri = reader.GetString();
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new JsonException("Bank account reference URI cannot be null or empty.");
        }

        return BankAccountReference.Parse(uri);
    }

    public override void Write(Utf8JsonWriter writer, BankAccountReference value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Uri);
    }
}
