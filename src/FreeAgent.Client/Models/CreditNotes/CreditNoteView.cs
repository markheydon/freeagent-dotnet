namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/credit_notes</c>, including the <c>last_{n}_months</c> pattern.
/// </summary>
public readonly struct CreditNoteView : IEquatable<CreditNoteView>
{
    private CreditNoteView(string wireValue) => WireValue = wireValue;

    /// <summary>Wire value sent as the <c>view</c> query parameter.</summary>
    public string WireValue { get; }

    /// <summary>All credit notes (default).</summary>
    public static CreditNoteView All { get; } = new("all");

    /// <summary>Recent, open, or overdue credit notes.</summary>
    public static CreditNoteView RecentOpenOrOverdue { get; } = new("recent_open_or_overdue");

    /// <summary>Open credit notes only.</summary>
    public static CreditNoteView Open { get; } = new("open");

    /// <summary>Overdue credit notes only.</summary>
    public static CreditNoteView Overdue { get; } = new("overdue");

    /// <summary>Open or overdue credit notes.</summary>
    public static CreditNoteView OpenOrOverdue { get; } = new("open_or_overdue");

    /// <summary>Draft credit notes only.</summary>
    public static CreditNoteView Draft { get; } = new("draft");

    /// <summary>Refunded credit notes only.</summary>
    public static CreditNoteView Refunded { get; } = new("refunded");

    /// <summary>
    /// Credit notes from the last <paramref name="monthCount"/> months (for example <c>last_3_months</c>).
    /// </summary>
    /// <param name="monthCount">Number of months (must be at least 1).</param>
    public static CreditNoteView LastMonths(int monthCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(monthCount, 1);
        return new($"last_{monthCount}_months");
    }

    /// <inheritdoc />
    public bool Equals(CreditNoteView other) => string.Equals(WireValue, other.WireValue, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CreditNoteView other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => WireValue.GetHashCode(StringComparison.Ordinal);

    /// <inheritdoc />
    public override string ToString() => WireValue;

    /// <summary>Equality operator.</summary>
    public static bool operator ==(CreditNoteView left, CreditNoteView right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(CreditNoteView left, CreditNoteView right) => !left.Equals(right);
}
