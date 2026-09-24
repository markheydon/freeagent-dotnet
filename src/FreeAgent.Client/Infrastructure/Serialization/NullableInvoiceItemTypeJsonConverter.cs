using System.Text.Json;
using System.Text.Json.Serialization;
using FreeAgent.Client.Models.Invoices;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Deserialises nullable <see cref="InvoiceItemType"/> values, treating blank strings as unset.
/// </summary>
internal sealed class NullableInvoiceItemTypeJsonConverter : JsonConverter<InvoiceItemType?>
{
    private static readonly JsonStringEnumMemberNameCompatibleConverter<InvoiceItemType> EnumConverter = new();

    public override InvoiceItemType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string when parsing {nameof(InvoiceItemType)}.");
        }

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var encoded = JsonSerializer.Serialize(value);
        var encodedBytes = System.Text.Encoding.UTF8.GetBytes(encoded);
        var enumReader = new Utf8JsonReader(encodedBytes);
        enumReader.Read();
        return EnumConverter.Read(ref enumReader, typeof(InvoiceItemType), options);
    }

    public override void Write(Utf8JsonWriter writer, InvoiceItemType? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        EnumConverter.Write(writer, value.Value, options);
    }
}
