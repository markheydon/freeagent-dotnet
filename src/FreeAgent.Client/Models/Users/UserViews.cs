namespace FreeAgent.Client.Models.Users;

/// <summary>
/// View filter values for <c>GET /v2/users</c>.
/// </summary>
public static class UserViews
{
    /// <summary>All users (FreeAgent default).</summary>
    public const string All = "all";

    /// <summary>Users with an Owner, Director, Partner, Company Secretary, Employee, or Shareholder role.</summary>
    public const string Staff = "staff";

    /// <summary>Non-hidden users with a staff role.</summary>
    public const string ActiveStaff = "active_staff";

    /// <summary>Users with an Accountant role.</summary>
    public const string Advisors = "advisors";

    /// <summary>Non-hidden users with an Accountant role.</summary>
    public const string ActiveAdvisors = "active_advisors";
}
