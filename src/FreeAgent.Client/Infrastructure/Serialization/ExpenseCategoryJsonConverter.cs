using System.Text.Json;
using System.Text.Json.Serialization;
namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Serialises and deserialises expense category links, including the <c>Mileage</c> literal.
/// </summary>
internal sealed class ExpenseCategoryJsonConverter : JsonConverter<ExpenseCategoryField>
{
    public const string MileageWireValue = "Mileage";

    public override ExpenseCategoryField? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.Equals(value, MileageWireValue, StringComparison.Ordinal))
            {
                return new ExpenseCategoryField(uri: null, isMileage: true);
            }

            return new ExpenseCategoryField(value);
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var category = JsonSerializer.Deserialize<Category>(ref reader, options);
            return category is null ? null : new ExpenseCategoryField(category.Url, category);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing expense category.");
    }

    public override void Write(Utf8JsonWriter writer, ExpenseCategoryField value, JsonSerializerOptions options)
    {
        if (value.IsMileage)
        {
            writer.WriteStringValue(MileageWireValue);
            return;
        }

        if (value.Uri is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Uri);
    }
}

/// <summary>
/// Serialises writable expense category values for create and update payloads.
/// </summary>
internal sealed class ExpenseCategoryWriteJsonConverter : JsonConverter<ExpenseCategoryWriteValue?>
{
    public override ExpenseCategoryWriteValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.Equals(value, ExpenseCategoryJsonConverter.MileageWireValue, StringComparison.Ordinal))
            {
                return ExpenseCategoryWriteValue.Mileage();
            }

            return value is null ? null : ExpenseCategoryWriteValue.ForUri(value);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing expense category write value.");
    }

    public override void Write(Utf8JsonWriter writer, ExpenseCategoryWriteValue? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        if (value.Value.IsMileage)
        {
            writer.WriteStringValue(ExpenseCategoryJsonConverter.MileageWireValue);
            return;
        }

        if (value.Value.Category is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.Category.Value.Uri);
    }
}

/// <summary>
/// Writable expense category: nominal code URI or mileage.
/// </summary>
internal readonly record struct ExpenseCategoryWriteValue
{
    private ExpenseCategoryWriteValue(bool isMileage, CategoryReference? category)
    {
        IsMileage = isMileage;
        Category = category;
    }

    public bool IsMileage { get; }

    public CategoryReference? Category { get; }

    public static ExpenseCategoryWriteValue Mileage() => new(true, null);

    public static ExpenseCategoryWriteValue ForCategory(CategoryReference category) => new(false, category);

    public static ExpenseCategoryWriteValue ForUri(string uri) => new(false, CategoryReference.Parse(uri));
}
