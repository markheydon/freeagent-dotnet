using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Sales tax rate on purchase resources: a percentage or the literal Auto wire value.
/// </summary>
[JsonConverter(typeof(PurchaseSalesTaxRateJsonConverter))]
public readonly struct PurchaseSalesTaxRate : IEquatable<PurchaseSalesTaxRate>
{
    /// <summary>
    /// Category-default sales tax rate (wire value: Auto).
    /// </summary>
    public static PurchaseSalesTaxRate Auto { get; } = new(isAuto: true, rate: null);

    private PurchaseSalesTaxRate(bool isAuto, decimal? rate)
    {
        IsAuto = isAuto;
        Rate = rate;
    }

    /// <summary>
    /// Whether the wire value is Auto.
    /// </summary>
    public bool IsAuto { get; }

    /// <summary>
    /// Percentage rate when not Auto.
    /// </summary>
    public decimal? Rate { get; }

    /// <summary>
    /// Creates a percentage sales tax rate.
    /// </summary>
    /// <param name="rate">Percentage value.</param>
    /// <returns>A purchase sales tax rate.</returns>
    public static PurchaseSalesTaxRate FromPercentage(decimal rate) => new(isAuto: false, rate: rate);

    /// <inheritdoc />
    public bool Equals(PurchaseSalesTaxRate other) => IsAuto == other.IsAuto && Rate == other.Rate;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is PurchaseSalesTaxRate other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(IsAuto, Rate);

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(PurchaseSalesTaxRate left, PurchaseSalesTaxRate right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(PurchaseSalesTaxRate left, PurchaseSalesTaxRate right) => !left.Equals(right);
}

internal sealed class PurchaseSalesTaxRateJsonConverter : JsonConverter<PurchaseSalesTaxRate>
{
    public override PurchaseSalesTaxRate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return default;

            case JsonTokenType.Number:
                return PurchaseSalesTaxRate.FromPercentage(reader.GetDecimal());

            case JsonTokenType.String:
                {
                    var stringValue = reader.GetString();
                    if (string.IsNullOrWhiteSpace(stringValue))
                    {
                        return default;
                    }

                    if (string.Equals(stringValue, "Auto", StringComparison.OrdinalIgnoreCase))
                    {
                        return PurchaseSalesTaxRate.Auto;
                    }

                    if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedRate))
                    {
                        return PurchaseSalesTaxRate.FromPercentage(parsedRate);
                    }

                    throw new JsonException($"Unsupported purchase sales_tax_rate wire value: {stringValue}.");
                }

            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when deserializing PurchaseSalesTaxRate.");
        }
    }

    public override void Write(Utf8JsonWriter writer, PurchaseSalesTaxRate value, JsonSerializerOptions options)
    {
        if (value.IsAuto)
        {
            writer.WriteStringValue("Auto");
            return;
        }

        if (value.Rate.HasValue)
        {
            writer.WriteNumberValue(value.Rate.Value);
            return;
        }

        writer.WriteNullValue();
    }
}
