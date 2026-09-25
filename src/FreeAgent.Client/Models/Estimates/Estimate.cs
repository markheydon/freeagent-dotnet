using System.Text.Json.Serialization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Invoices;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.Estimates;

/// <summary>
/// Represents a FreeAgent estimate.
/// </summary>
public sealed class Estimate : IFreeAgentResource
{
    private SettableLinkId _contactId;
    private SettableLinkId _projectId;

    /// <summary>
    /// Estimate resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public long ResourceId => FreeAgentResourceId.TryParse(Url, out var id) ? id : 0;

    /// <summary>
    /// Estimate status.
    /// </summary>
    [JsonPropertyName("status")]
    public EstimateStatus? Status { get; set; }

    /// <summary>
    /// Estimate document type.
    /// </summary>
    [JsonPropertyName("estimate_type")]
    public EstimateType? EstimateType { get; set; }

    /// <summary>
    /// Wire representation of the contact link.
    /// </summary>
    [JsonPropertyName("contact")]
    [JsonInclude]
    internal ExpandableField<Contact>? ContactLink { get; set; }

    /// <summary>
    /// Contact when returned nested on the wire or hydrated via <see cref="EstimateGetOptions.IncludeContact"/>.
    /// </summary>
    [JsonIgnore]
    public Contact? Contact => ContactLink?.Value;

    /// <summary>
    /// Contact identifier for create and update requests. Populated after GET when the API returns a contact link.
    /// </summary>
    [JsonIgnore]
    public long? ContactId
    {
        get => _contactId.Get(ContactLink?.Id);
        set => _contactId.Set(value);
    }

    /// <summary>
    /// Wire representation of the project link.
    /// </summary>
    [JsonPropertyName("project")]
    [JsonInclude]
    internal ExpandableField<Project>? ProjectLink { get; set; }

    /// <summary>
    /// Project when returned nested on the wire or hydrated via <see cref="EstimateGetOptions.IncludeProject"/>.
    /// </summary>
    [JsonIgnore]
    public Project? Project => ProjectLink?.Value;

    /// <summary>
    /// Project identifier for create and update requests. Populated after GET when the API returns a project link.
    /// </summary>
    [JsonIgnore]
    public long? ProjectId
    {
        get => _projectId.Get(ProjectLink?.Id);
        set => _projectId.Set(value);
    }

    /// <summary>
    /// Wire representation of the linked invoice after conversion.
    /// </summary>
    [JsonPropertyName("invoice")]
    [JsonInclude]
    internal ExpandableField<Invoice>? InvoiceLink { get; set; }

    /// <summary>
    /// Invoice identifier parsed from the estimate response.
    /// </summary>
    [JsonIgnore]
    public long? InvoiceId => InvoiceLink?.Id;

    /// <summary>
    /// Free-text reference.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Date of the estimate.
    /// </summary>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; set; }

    /// <summary>
    /// Estimate currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public CurrencyCode? Currency { get; set; }

    /// <summary>
    /// Additional text.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Discount applied across the whole estimate.
    /// </summary>
    [JsonPropertyName("discount_percent")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DiscountPercent { get; set; }

    /// <summary>
    /// Client contact name override for this estimate.
    /// </summary>
    [JsonPropertyName("client_contact_name")]
    public string? ClientContactName { get; set; }

    /// <summary>
    /// Estimate VAT status for reporting purposes.
    /// </summary>
    [JsonPropertyName("ec_status")]
    public EstimateEcStatus? EcStatus { get; set; }

    /// <summary>
    /// Place of supply when <see cref="EcStatus"/> is EC VAT MOSS.
    /// </summary>
    [JsonPropertyName("place_of_supply")]
    public string? PlaceOfSupply { get; set; }

    /// <summary>
    /// Total value calculated from estimate items.
    /// </summary>
    [JsonPropertyName("net_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? NetValue { get; set; }

    /// <summary>
    /// Total value of sales tax.
    /// </summary>
    [JsonPropertyName("sales_tax_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? SalesTaxValue { get; set; }

    /// <summary>
    /// Sales tax status for the estimate.
    /// </summary>
    [JsonPropertyName("sales_tax_status")]
    public InvoiceSalesTaxStatus? SalesTaxStatus { get; set; }

    /// <summary>
    /// Whether sales tax is included in totals shown on the estimate.
    /// </summary>
    [JsonPropertyName("include_sales_tax_on_total_value")]
    public bool? IncludeSalesTaxOnTotalValue { get; set; }

    /// <summary>
    /// Estimate line items.
    /// </summary>
    [JsonPropertyName("estimate_items")]
    public List<EstimateItem>? EstimateItems { get; set; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Attaches a hydrated contact to this estimate.
    /// </summary>
    /// <param name="contact">Contact details.</param>
    internal void AttachContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        ContactLink = new ExpandableField<Contact>(contact.Url, contact);
    }

    /// <summary>
    /// Attaches a hydrated project to this estimate.
    /// </summary>
    /// <param name="project">Project details.</param>
    internal void AttachProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        ProjectLink = new ExpandableField<Project>(project.Url, project);
    }
}
