namespace FreeAgent.Client.Models.Shared;

/// <summary>
/// Optional field for partial PUT payloads: unspecified keys are omitted, specified null clears the section.
/// </summary>
/// <typeparam name="T">Payload type for the section.</typeparam>
public readonly struct PatchValue<T>
{
    internal PatchValue(bool isSpecified, T? value)
    {
        IsSpecified = isSpecified;
        Value = value;
    }

    /// <summary>
    /// Whether this field was included in the update request.
    /// </summary>
    public bool IsSpecified { get; }

    /// <summary>
    /// Section value, or <see langword="null"/> when clearing a registration.
    /// </summary>
    public T? Value { get; }
}

/// <summary>
/// Factory helpers for <see cref="PatchValue{T}"/>.
/// </summary>
public static class PatchValue
{
#pragma warning disable CA1000 // Generic factory methods are intentional for partial-update ergonomics.

    /// <summary>
    /// Creates an unspecified patch (property omitted from JSON).
    /// </summary>
    public static PatchValue<T> Unspecified<T>() => default;

    /// <summary>
    /// Includes the section in the request with the given value (use <see langword="null"/> to clear).
    /// </summary>
    /// <param name="value">Section payload or null to clear.</param>
    public static PatchValue<T> Set<T>(T? value) => new(true, value);

#pragma warning restore CA1000
}
