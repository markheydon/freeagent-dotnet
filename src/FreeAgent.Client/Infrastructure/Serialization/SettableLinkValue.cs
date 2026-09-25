namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Backing store for a link key (for example category nominal code) that round-trips from
/// deserialised wire links but can be explicitly assigned on write.
/// </summary>
internal struct SettableLinkValue
{
    private string? _value;
    private bool _isSet;

    /// <summary>
    /// Returns the explicitly assigned value when set; otherwise the key from the wire link.
    /// </summary>
    /// <param name="fromLink">Key parsed from the deserialised link field.</param>
    public string? Get(string? fromLink) => _isSet ? _value : fromLink;

    /// <summary>
    /// Assigns the link key for the next write payload.
    /// </summary>
    /// <param name="value">Category nominal code or other non-numeric link key.</param>
    public void Set(string? value)
    {
        _value = value;
        _isSet = true;
    }
}
