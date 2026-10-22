using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client;

/// <summary>
/// Represents a journal entry within a journal set.
/// </summary>
public sealed class JournalEntry
{
    private SettableLinkId _userId;
    private SettableLinkId _contactId;
    private SettableLinkId _stockItemId;
    private SettableLinkId _bankAccountId;
    private SettableLinkId _capitalAssetTypeId;
    private SettableLinkId _propertyId;
    private SettableLinkValue _categoryNominalCode;

    /// <summary>
    /// Journal entry resource URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Wire representation of the category link.
    /// </summary>
    [JsonPropertyName("category")]
    [JsonInclude]
    internal ExpandableField<Category>? CategoryLink { get; set; }

    /// <summary>
    /// Category when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Category? Category => CategoryLink?.Value;

    /// <summary>
    /// Category nominal code for create and update requests.
    /// </summary>
    [JsonIgnore]
    public string? CategoryNominalCode
    {
        get => _categoryNominalCode.Get(
            CategoryLink?.Uri is string uri ? CategoryReference.ExtractNominalCode(uri) : null);
        set => _categoryNominalCode.Set(value);
    }

    /// <summary>
    /// Entry description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Debit value for the journal entry.
    /// </summary>
    [JsonPropertyName("debit_value")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? DebitValue { get; set; }

    /// <summary>
    /// Wire representation of the user link.
    /// </summary>
    [JsonPropertyName("user")]
    [JsonInclude]
    internal ExpandableField<User>? UserLink { get; set; }

    /// <summary>
    /// User when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public User? User => UserLink?.Value;

    /// <summary>
    /// User identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? UserId
    {
        get => _userId.Get(UserLink?.Id);
        set => _userId.Set(value);
    }

    /// <summary>
    /// Wire representation of the contact link.
    /// </summary>
    [JsonPropertyName("contact")]
    [JsonInclude]
    internal ExpandableField<Contact>? ContactLink { get; set; }

    /// <summary>
    /// Contact when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public Contact? Contact => ContactLink?.Value;

    /// <summary>
    /// Contact identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? ContactId
    {
        get => _contactId.Get(ContactLink?.Id);
        set => _contactId.Set(value);
    }

    /// <summary>
    /// Wire representation of the stock item link.
    /// </summary>
    [JsonPropertyName("stock_item")]
    [JsonInclude]
    internal ExpandableField<StockItem>? StockItemLink { get; set; }

    /// <summary>
    /// Stock item when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public StockItem? StockItem => StockItemLink?.Value;

    /// <summary>
    /// Stock item identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? StockItemId
    {
        get => _stockItemId.Get(StockItemLink?.Id);
        set => _stockItemId.Set(value);
    }

    /// <summary>
    /// Quantity change for the linked stock item.
    /// </summary>
    [JsonPropertyName("stock_altering_quantity")]
    public int? StockAlteringQuantity { get; set; }

    /// <summary>
    /// Wire representation of the bank account link (historical data only).
    /// </summary>
    [JsonPropertyName("bank_account")]
    [JsonInclude]
    internal ExpandableField<BankAccount>? BankAccountLink { get; set; }

    /// <summary>
    /// Bank account when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public BankAccount? BankAccount => BankAccountLink?.Value;

    /// <summary>
    /// Bank account identifier on read responses.
    /// </summary>
    [JsonIgnore]
    public long? BankAccountId
    {
        get => _bankAccountId.Get(BankAccountLink?.Id);
        set => _bankAccountId.Set(value);
    }

    /// <summary>
    /// Wire representation of the capital asset type link.
    /// </summary>
    [JsonPropertyName("capital_asset_type")]
    [JsonInclude]
    internal ExpandableField<CapitalAssetType>? CapitalAssetTypeLink { get; set; }

    /// <summary>
    /// Capital asset type when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public CapitalAssetType? CapitalAssetType => CapitalAssetTypeLink?.Value;

    /// <summary>
    /// Capital asset type identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? CapitalAssetTypeId
    {
        get => _capitalAssetTypeId.Get(CapitalAssetTypeLink?.Id);
        set => _capitalAssetTypeId.Set(value);
    }

    /// <summary>
    /// Wire representation of the property link.
    /// </summary>
    [JsonPropertyName("property")]
    [JsonInclude]
    internal ExpandableField<LandlordProperty>? PropertyLink { get; set; }

    /// <summary>
    /// Landlord property when returned nested on the wire.
    /// </summary>
    [JsonIgnore]
    public LandlordProperty? Property => PropertyLink?.Value;

    /// <summary>
    /// Property identifier for create and update requests.
    /// </summary>
    [JsonIgnore]
    public long? PropertyId
    {
        get => _propertyId.Get(PropertyLink?.Id);
        set => _propertyId.Set(value);
    }

    private long? _explicitEntryId;

    /// <summary>
    /// Journal entry identifier used when updating or destroying an existing line.
    /// </summary>
    [JsonIgnore]
    public long? EntryId
    {
        get => _explicitEntryId ?? TryParseEntryIdFromUrl();
        set => _explicitEntryId = value;
    }

    /// <summary>
    /// When <see langword="true"/>, removes the journal entry on update.
    /// </summary>
    [JsonIgnore]
    public bool? Destroy { get; set; }

    internal SettableLinkValue CategoryNominalCodeBacking => _categoryNominalCode;

    internal string? CategoryLinkNominalCode =>
        CategoryLink?.Uri is string uri ? CategoryReference.ExtractNominalCode(uri) : null;

    internal SettableLinkId UserIdBacking => _userId;

    internal long? UserLinkId => UserLink?.Id;

    internal SettableLinkId ContactIdBacking => _contactId;

    internal long? ContactLinkId => ContactLink?.Id;

    internal SettableLinkId StockItemIdBacking => _stockItemId;

    internal long? StockItemLinkId => StockItemLink?.Id;

    internal SettableLinkId CapitalAssetTypeIdBacking => _capitalAssetTypeId;

    internal long? CapitalAssetTypeLinkId => CapitalAssetTypeLink?.Id;

    internal SettableLinkId PropertyIdBacking => _propertyId;

    internal long? PropertyLinkId => PropertyLink?.Id;

    internal SettableLinkId BankAccountIdBacking => _bankAccountId;

    internal long? BankAccountLinkId => BankAccountLink?.Id;

    private long? TryParseEntryIdFromUrl() =>
        !string.IsNullOrWhiteSpace(Url) && FreeAgentResourceId.TryParse(Url, out var id) ? id : null;
}
