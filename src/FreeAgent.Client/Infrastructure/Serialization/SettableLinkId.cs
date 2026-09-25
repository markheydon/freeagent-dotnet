namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Backing store for a link identifier that round-trips from deserialised wire links
/// but can be explicitly assigned (including null to clear) on write.
/// </summary>
internal struct SettableLinkId
{
    private long? _value;
    private bool _isSet;

    /// <summary>
    /// Whether the link identifier was explicitly assigned (including null to clear).
    /// </summary>
    public bool IsExplicitlySet => _isSet;

    /// <summary>
    /// Returns the explicitly assigned value when set; otherwise the identifier from the wire link.
    /// </summary>
    /// <param name="fromLink">Identifier parsed from the deserialised link field.</param>
    public long? Get(long? fromLink) => _isSet ? _value : fromLink;

    /// <summary>
    /// Assigns the link identifier for the next write payload.
    /// </summary>
    /// <param name="value">Contact, project, or other linked resource identifier; null clears the link.</param>
    public void Set(long? value)
    {
        _value = value;
        _isSet = true;
    }
}
