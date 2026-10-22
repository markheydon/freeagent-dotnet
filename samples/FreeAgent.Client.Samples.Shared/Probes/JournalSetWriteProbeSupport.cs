namespace FreeAgent.Client.Samples.Shared.Probes;

/// <summary>
/// Balanced journal set payloads for console and Blazor write probes.
/// </summary>
/// <remarks>
/// Category <c>901</c> is a user category; FreeAgent returns <c>404 Not Found</c> when <c>user</c> is omitted.
/// </remarks>
public static class JournalSetWriteProbeSupport
{
    /// <summary>
    /// Builds the standard balancing entry pair (categories <c>001</c> and <c>901</c>).
    /// </summary>
    /// <param name="userId">User id for the <c>901</c> line (director capital / user category).</param>
    public static JournalEntry[] CreateBalancingProbeEntries(long userId) =>
    [
        new JournalEntry
        {
            CategoryNominalCode = "001",
            DebitValue = -1.00m,
        },
        new JournalEntry
        {
            CategoryNominalCode = "901",
            UserId = userId,
            DebitValue = 1.00m,
        },
    ];

    /// <summary>
    /// Builds a create request for a tagged probe journal set.
    /// </summary>
    public static CreateJournalSetRequest BuildCreateRequest(
        string description,
        string tag,
        long userId,
        DateOnly datedOn) =>
        new()
        {
            Description = description,
            DatedOn = datedOn,
            Tag = tag,
            JournalEntries = CreateBalancingProbeEntries(userId),
        };
}
