using System.Text.Json;
using System.Text.Json.Serialization;

namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Second sales tax status on universal accounts (wire field <c>second_sales_tax_status</c>).
/// </summary>
[JsonConverter(typeof(SecondSalesTaxStatusJsonConverter))]
public readonly struct SecondSalesTaxStatus : IEquatable<SecondSalesTaxStatus>
{
    /// <summary>
    /// Creates a second sales tax status from a known kind and optional wire value.
    /// </summary>
    /// <param name="kind">Known classification.</param>
    /// <param name="wireValue">Original wire token; required when <paramref name="kind"/> is <see cref="SecondSalesTaxStatusKind.Unknown"/>.</param>
    public SecondSalesTaxStatus(SecondSalesTaxStatusKind kind, string? wireValue = null)
    {
        Kind = kind;
        WireValue = wireValue ?? (kind == SecondSalesTaxStatusKind.Unknown ? null : GetDefaultWireValue(kind));
    }

    /// <summary>
    /// Known classification when the wire token matches a documented value.
    /// </summary>
    public SecondSalesTaxStatusKind Kind { get; }

    /// <summary>
    /// Original or canonical wire token from FreeAgent (always set when deserialised from JSON).
    /// </summary>
    public string? WireValue { get; }

    /// <summary>
    /// Parses a FreeAgent wire value.
    /// </summary>
    /// <param name="wireValue">Wire token.</param>
    /// <returns>A second sales tax status.</returns>
    public static SecondSalesTaxStatus FromWire(string wireValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(wireValue);

        var kind = wireValue.ToUpperInvariant() switch
        {
            "TAXABLE" => SecondSalesTaxStatusKind.Taxable,
            "EXEMPT" => SecondSalesTaxStatusKind.Exempt,
            "OUT_OF_SCOPE" => SecondSalesTaxStatusKind.OutOfScope,
            _ => SecondSalesTaxStatusKind.Unknown
        };

        return new SecondSalesTaxStatus(kind, wireValue);
    }

    /// <inheritdoc />
    public bool Equals(SecondSalesTaxStatus other) =>
        Kind == other.Kind && string.Equals(WireValue, other.WireValue, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is SecondSalesTaxStatus other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Kind, WireValue);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(SecondSalesTaxStatus left, SecondSalesTaxStatus right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(SecondSalesTaxStatus left, SecondSalesTaxStatus right) => !left.Equals(right);

    internal static string? GetDefaultWireValue(SecondSalesTaxStatusKind kind) => kind switch
    {
        SecondSalesTaxStatusKind.Taxable => "TAXABLE",
        SecondSalesTaxStatusKind.Exempt => "EXEMPT",
        SecondSalesTaxStatusKind.OutOfScope => "OUT_OF_SCOPE",
        SecondSalesTaxStatusKind.Unknown => null,
        _ => null
    };
}

/// <summary>
/// Documented <c>second_sales_tax_status</c> values (aligned with primary sales tax status tokens).
/// </summary>
public enum SecondSalesTaxStatusKind
{
    /// <summary>Item is taxable (wire value: <c>TAXABLE</c>).</summary>
    Taxable,

    /// <summary>Item is exempt (wire value: <c>EXEMPT</c>).</summary>
    Exempt,

    /// <summary>Item is out of scope (wire value: <c>OUT_OF_SCOPE</c>).</summary>
    OutOfScope,

    /// <summary>Undocumented or account-specific wire value.</summary>
    Unknown
}

/// <summary>
/// JSON converter for <see cref="SecondSalesTaxStatus"/>.
/// </summary>
public sealed class SecondSalesTaxStatusJsonConverter : JsonConverter<SecondSalesTaxStatus>
{
    /// <inheritdoc />
    public override SecondSalesTaxStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string when parsing {nameof(SecondSalesTaxStatus)}.");
        }

        var wire = reader.GetString();
        if (string.IsNullOrWhiteSpace(wire))
        {
            return default;
        }

        return SecondSalesTaxStatus.FromWire(wire);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, SecondSalesTaxStatus value, JsonSerializerOptions options)
    {
        var wire = value.WireValue ?? SecondSalesTaxStatus.GetDefaultWireValue(value.Kind);
        if (string.IsNullOrWhiteSpace(wire))
        {
            throw new JsonException($"Cannot serialise {nameof(SecondSalesTaxStatus)} without a wire value.");
        }

        writer.WriteStringValue(wire);
    }
}

/// <summary>
/// JSON converter for nullable <see cref="SecondSalesTaxStatus"/>.
/// </summary>
public sealed class NullableSecondSalesTaxStatusJsonConverter : JsonConverter<SecondSalesTaxStatus?>
{
    private static readonly SecondSalesTaxStatusJsonConverter Inner = new();

    /// <inheritdoc />
    public override SecondSalesTaxStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return Inner.Read(ref reader, typeof(SecondSalesTaxStatus), options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, SecondSalesTaxStatus? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        Inner.Write(writer, value.Value, options);
    }
}
