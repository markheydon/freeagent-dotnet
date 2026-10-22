using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// An account lock that prevents changes before a given date.
/// </summary>
public sealed class AccountLock
{
    /// <summary>
    /// Date through which the account is locked.
    /// </summary>
    [JsonPropertyName("locked_to_date")]
    public DateOnly? LockedToDate { get; set; }

    /// <summary>
    /// When <see langword="true"/>, this row is the user account lock (the only lock that can be created or deleted via the API).
    /// </summary>
    [JsonPropertyName("user_lock")]
    public bool? UserLock { get; set; }

    /// <summary>
    /// Description of the resource that created the lock (for example a user name or accounting year end label).
    /// </summary>
    [JsonPropertyName("locked_by_description")]
    public string? LockedByDescription { get; set; }

    /// <summary>
    /// Wire representation of the user or resource that created the lock, when available.
    /// </summary>
    [JsonPropertyName("locked_by_url")]
    [JsonInclude]
    internal ExpandableField<User>? LockedByLink { get; set; }

    /// <summary>
    /// User nested on the wire at <c>locked_by_url</c>, when returned expanded.
    /// </summary>
    [JsonIgnore]
    public User? LockedByUser => LockedByLink?.Value;

    /// <summary>
    /// User identifier parsed from <see cref="LockedByLink"/>.
    /// </summary>
    [JsonIgnore]
    public long? LockedByUserId => LockedByLink?.Id;
}
