namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// View filter constants for estimate list requests.
/// </summary>
public static class EstimateViews
{
    /// <summary>All estimates.</summary>
    public const string All = "all";

    /// <summary>Recent estimates only.</summary>
    public const string Recent = "recent";

    /// <summary>Draft estimates only.</summary>
    public const string Draft = "draft";

    /// <summary>Non-draft estimates only.</summary>
    public const string NonDraft = "non_draft";

    /// <summary>Sent estimates only.</summary>
    public const string Sent = "sent";

    /// <summary>Approved estimates only.</summary>
    public const string Approved = "approved";

    /// <summary>Rejected estimates only.</summary>
    public const string Rejected = "rejected";

    /// <summary>Invoiced estimates only.</summary>
    public const string Invoiced = "invoiced";
}
