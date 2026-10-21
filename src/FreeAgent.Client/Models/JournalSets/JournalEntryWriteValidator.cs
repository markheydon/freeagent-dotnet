using System.Globalization;

namespace FreeAgent.Client;

/// <summary>
/// Fail-fast checks for journal entry writes using constraints stated in the FreeAgent journal sets docs.
/// </summary>
/// <remarks>
/// Does not validate category-specific shapes that require account state (user categories, stock categories, landlord property).
/// </remarks>
internal static class JournalEntryWriteValidator
{
    internal const int CapitalAssetCategoryMin = 601;
    internal const int CapitalAssetCategoryMax = 607;

    internal static void ValidateForCreate(IReadOnlyList<JournalEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        if (entries.Count == 0)
        {
            throw new ArgumentException("At least one journal entry is required.", nameof(entries));
        }

        foreach (var entry in entries)
        {
            ValidateCreateEntry(entry);
        }
    }

    internal static void ValidateForUpdate(IReadOnlyList<JournalEntry>? entries)
    {
        if (entries is null)
        {
            return;
        }

        foreach (var entry in entries)
        {
            ValidateUpdateEntry(entry);
        }
    }

    private static void ValidateCreateEntry(JournalEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var categoryCode = RequireCategoryNominalCode(entry, nameof(entry));
        RequireDebitValue(entry, nameof(entry));
        ValidateDocumentedConditionalFields(entry, categoryCode);
    }

    private static void ValidateUpdateEntry(JournalEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.Destroy == true)
        {
            if (string.IsNullOrWhiteSpace(entry.Url) && entry.EntryId is null)
            {
                throw new ArgumentException(
                    "Journal entry URL or entry id is required when destroying a journal entry.",
                    nameof(entry));
            }

            return;
        }

        var isNewEntry = string.IsNullOrWhiteSpace(entry.Url) && entry.EntryId is null;
        if (isNewEntry)
        {
            var categoryCode = RequireCategoryNominalCode(entry, nameof(entry));
            RequireDebitValue(entry, nameof(entry));
            ValidateDocumentedConditionalFields(entry, categoryCode);
            return;
        }

        var effectiveCategory = GetEffectiveCategoryNominalCode(entry);
        if (effectiveCategory is not null)
        {
            ValidateDocumentedConditionalFields(entry, effectiveCategory);
        }
    }

    private static string RequireCategoryNominalCode(JournalEntry entry, string paramName)
    {
        var code = GetEffectiveCategoryNominalCode(entry);
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Category nominal code is required for journal entries.", paramName);
        }

        return code;
    }

    private static void RequireDebitValue(JournalEntry entry, string paramName)
    {
        if (entry.DebitValue is null)
        {
            throw new ArgumentException("Debit value is required for journal entries.", paramName);
        }
    }

    private static void ValidateDocumentedConditionalFields(JournalEntry entry, string categoryNominalCode)
    {
        if (!RequiresCapitalAssetType(categoryNominalCode))
        {
            return;
        }

        if (!HasCapitalAssetType(entry))
        {
            throw new ArgumentException(
                $"Capital asset type is required when the journal entry category is {categoryNominalCode} (categories {CapitalAssetCategoryMin:D3}–{CapitalAssetCategoryMax:D3}).",
                nameof(entry));
        }
    }

    internal static bool RequiresCapitalAssetType(string categoryNominalCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryNominalCode);

        if (!TryParseBaseCategoryCode(categoryNominalCode, out var baseCode))
        {
            return false;
        }

        return baseCode >= CapitalAssetCategoryMin && baseCode <= CapitalAssetCategoryMax;
    }

    private static string? GetEffectiveCategoryNominalCode(JournalEntry entry)
    {
        if (entry.CategoryNominalCodeBacking.IsExplicitlySet)
        {
            return entry.CategoryNominalCodeBacking.Get(fromLink: null);
        }

        return entry.CategoryLinkNominalCode;
    }

    private static bool HasCapitalAssetType(JournalEntry entry) =>
        entry.CapitalAssetTypeIdBacking.IsExplicitlySet
            ? entry.CapitalAssetTypeId is not null
            : entry.CapitalAssetTypeLinkId is not null;

    private static bool TryParseBaseCategoryCode(string categoryNominalCode, out int baseCode)
    {
        baseCode = 0;
        var segment = categoryNominalCode.Split('-', 2, StringSplitOptions.TrimEntries)[0];
        return int.TryParse(segment, NumberStyles.None, CultureInfo.InvariantCulture, out baseCode);
    }
}
