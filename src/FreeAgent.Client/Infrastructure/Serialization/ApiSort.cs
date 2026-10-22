namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Sort order for list endpoints that accept a <c>sort</c> query parameter with an optional descending <c>-</c> prefix.
/// </summary>
/// <typeparam name="TField">Documented sort field enum for the resource.</typeparam>
public readonly struct ApiSort<TField> : IEquatable<ApiSort<TField>>
    where TField : struct, Enum
{
    /// <summary>
    /// Creates a sort specification.
    /// </summary>
    /// <param name="field">Documented sort field.</param>
    /// <param name="descending">When <see langword="true"/>, prefixes the wire value with <c>-</c>.</param>
    public ApiSort(TField field, bool descending = false)
    {
        Field = field;
        Descending = descending;
    }

    /// <summary>Sort field.</summary>
    public TField Field { get; }

    /// <summary>Whether the API should sort in descending order.</summary>
    public bool Descending { get; }

    /// <summary>Returns the wire <c>sort</c> query value.</summary>
    public string ToWireValue()
    {
        var name = EnumWireValue.Get(Field);
        return Descending ? "-" + name : name;
    }

    /// <inheritdoc />
    public bool Equals(ApiSort<TField> other) => Field.Equals(other.Field) && Descending == other.Descending;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ApiSort<TField> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Field, Descending);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(ApiSort<TField> left, ApiSort<TField> right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(ApiSort<TField> left, ApiSort<TField> right) => !left.Equals(right);
}
