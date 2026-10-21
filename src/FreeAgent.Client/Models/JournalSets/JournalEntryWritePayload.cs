using System.Text.Json.Serialization;
using FreeAgent.Client.Infrastructure.Configuration;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Models.JournalSets;

/// <summary>
/// Writable journal entry attributes for create and update requests.
/// </summary>
internal sealed class JournalEntryWritePayload
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("category")]
    [JsonConverter(typeof(WriteLinkJsonConverter<CategoryReference>))]
    public WriteLink<CategoryReference>? Category { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("debit_value")]
    public decimal? DebitValue { get; set; }

    [JsonPropertyName("user")]
    [JsonConverter(typeof(WriteLinkJsonConverter<UserReference>))]
    public WriteLink<UserReference>? User { get; set; }

    [JsonPropertyName("contact")]
    [JsonConverter(typeof(WriteLinkJsonConverter<ContactReference>))]
    public WriteLink<ContactReference>? Contact { get; set; }

    [JsonPropertyName("stock_item")]
    [JsonConverter(typeof(WriteLinkJsonConverter<StockItemReference>))]
    public WriteLink<StockItemReference>? StockItem { get; set; }

    [JsonPropertyName("stock_altering_quantity")]
    public int? StockAlteringQuantity { get; set; }

    [JsonPropertyName("capital_asset_type")]
    [JsonConverter(typeof(WriteLinkJsonConverter<CapitalAssetTypeReference>))]
    public WriteLink<CapitalAssetTypeReference>? CapitalAssetType { get; set; }

    [JsonPropertyName("property")]
    [JsonConverter(typeof(WriteLinkJsonConverter<PropertyReference>))]
    public WriteLink<PropertyReference>? Property { get; set; }

    [JsonPropertyName("_destroy")]
    public bool? Destroy { get; set; }

    public static JournalEntryWritePayload FromJournalEntry(
        JournalEntry entry,
        FreeAgentEnvironment environment,
        long? journalSetId,
        bool isUpdate)
    {
        ArgumentNullException.ThrowIfNull(entry);

        string? url = null;
        if (isUpdate)
        {
            if (entry.Destroy == true || entry.Url is not null || entry.EntryId is not null)
            {
                url = entry.Url;
                if (string.IsNullOrEmpty(url) && entry.EntryId is long entryId && journalSetId is long setId)
                {
                    url = $"{FreeAgentEnvironmentEndpoints.GetApiBaseUrl(environment)}journal_sets/{setId}/journal_entries/{entryId}";
                }

                if (entry.Destroy != true && string.IsNullOrEmpty(url))
                {
                    url = string.Empty;
                }
            }
        }
        else if (!string.IsNullOrEmpty(entry.Url))
        {
            url = entry.Url;
        }

        return new JournalEntryWritePayload
        {
            Url = url,
            Category = LinkedResourceWriteMapper.ResolveCategoryReference(
                environment,
                entry.CategoryNominalCodeBacking,
                entry.CategoryLinkNominalCode,
                omit: entry.Destroy == true),
            Description = entry.Destroy == true ? null : entry.Description,
            DebitValue = entry.Destroy == true ? null : entry.DebitValue,
            User = LinkedResourceWriteMapper.ResolveUserReference(
                environment,
                entry.UserIdBacking,
                entry.UserLinkId,
                omit: entry.Destroy == true),
            Contact = LinkedResourceWriteMapper.ResolveContactReference(
                environment,
                entry.ContactIdBacking,
                entry.ContactLinkId,
                omit: entry.Destroy == true),
            StockItem = LinkedResourceWriteMapper.ResolveStockItemReference(
                environment,
                entry.StockItemIdBacking,
                entry.StockItemLinkId,
                omit: entry.Destroy == true),
            StockAlteringQuantity = entry.Destroy == true ? null : entry.StockAlteringQuantity,
            CapitalAssetType = LinkedResourceWriteMapper.ResolveCapitalAssetTypeReference(
                environment,
                entry.CapitalAssetTypeIdBacking,
                entry.CapitalAssetTypeLinkId,
                omit: entry.Destroy == true),
            Property = LinkedResourceWriteMapper.ResolvePropertyReference(
                environment,
                entry.PropertyIdBacking,
                entry.PropertyLinkId,
                omit: entry.Destroy == true),
            Destroy = entry.Destroy
        };
    }
}
