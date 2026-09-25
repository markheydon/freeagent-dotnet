namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Tri-state write value for a linked resource: omit from JSON, clear with null, or set to a reference.
/// </summary>
internal sealed class WriteLink<T>
    where T : struct
{
    /// <summary>
    /// When <see langword="true"/>, the wire field is serialised as JSON null.
    /// </summary>
    public bool IsCleared { get; init; }

    /// <summary>
    /// Typed reference to serialise when not cleared.
    /// </summary>
    public T Value { get; init; }
}
