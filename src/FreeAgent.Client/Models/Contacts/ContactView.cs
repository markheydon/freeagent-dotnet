using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client;

/// <summary>
/// Documented <c>view</c> query values for <c>GET /v2/contacts</c>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumMemberNameCompatibleConverter<ContactView>))]
public enum ContactView
{
    /// <summary>All contacts.</summary>
    [JsonStringEnumMemberName("all")]
    All,

    /// <summary>Active contacts (FreeAgent default).</summary>
    [JsonStringEnumMemberName("active")]
    Active,

    /// <summary>All clients.</summary>
    [JsonStringEnumMemberName("clients")]
    Clients,

    /// <summary>Active suppliers.</summary>
    [JsonStringEnumMemberName("suppliers")]
    Suppliers,

    /// <summary>Clients with active projects.</summary>
    [JsonStringEnumMemberName("active_projects")]
    ActiveProjects,

    /// <summary>Clients with completed invoices.</summary>
    [JsonStringEnumMemberName("completed_projects")]
    CompletedProjects,

    /// <summary>Clients with open invoices.</summary>
    [JsonStringEnumMemberName("open_clients")]
    OpenClients,

    /// <summary>Suppliers with open bills.</summary>
    [JsonStringEnumMemberName("open_suppliers")]
    OpenSuppliers,

    /// <summary>Hidden contacts.</summary>
    [JsonStringEnumMemberName("hidden")]
    Hidden,

    /// <summary>Undocumented or future FreeAgent wire value.</summary>
    Unknown
}
