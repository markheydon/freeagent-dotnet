namespace FreeAgent.Client.Models.Users;

/// <summary>
/// FreeAgent permission levels for user resources.
/// </summary>
/// <remarks>
/// See the <a href="https://dev.freeagent.com/docs/users#permissions">Users permissions</a> section in the official API docs.
/// </remarks>
public enum UserPermissionLevel
{
    /// <summary>No access.</summary>
    NoAccess = 0,

    /// <summary>Time access.</summary>
    Time = 1,

    /// <summary>My Money access.</summary>
    MyMoney = 2,

    /// <summary>Contacts and Projects access.</summary>
    ContactsAndProjects = 3,

    /// <summary>Invoices, Estimates and Files access.</summary>
    InvoicesEstimatesAndFiles = 4,

    /// <summary>Bills access.</summary>
    Bills = 5,

    /// <summary>Banking access.</summary>
    Banking = 6,

    /// <summary>Tax, Accounting and Users access.</summary>
    TaxAccountingAndUsers = 7,

    /// <summary>Full access.</summary>
    Full = 8
}
