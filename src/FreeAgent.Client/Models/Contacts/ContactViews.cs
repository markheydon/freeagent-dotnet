namespace FreeAgent.Client.Models.Contacts;

/// <summary>
/// View filter values for <c>GET /v2/contacts</c>.
/// </summary>
public static class ContactViews
{
    /// <summary>All contacts.</summary>
    public const string All = "all";

    /// <summary>Active contacts (FreeAgent default).</summary>
    public const string Active = "active";

    /// <summary>All clients.</summary>
    public const string Clients = "clients";

    /// <summary>Active suppliers.</summary>
    public const string Suppliers = "suppliers";

    /// <summary>Clients with active projects.</summary>
    public const string ActiveProjects = "active_projects";

    /// <summary>Clients with completed invoices.</summary>
    public const string CompletedProjects = "completed_projects";

    /// <summary>Clients with open invoices.</summary>
    public const string OpenClients = "open_clients";

    /// <summary>Suppliers with open bills.</summary>
    public const string OpenSuppliers = "open_suppliers";

    /// <summary>Hidden contacts.</summary>
    public const string Hidden = "hidden";
}
