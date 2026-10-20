using FreeAgent.Client.Models.Categories;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Infrastructure.Serialization;

/// <summary>
/// Wire adapter for expense <c>category</c>, which may be a category URI, nested category object, or the literal <c>Mileage</c>.
/// </summary>
internal sealed class ExpenseCategoryField
{
    public ExpenseCategoryField(string? uri, Category? value = null, bool isMileage = false)
    {
        if (isMileage)
        {
            IsMileage = true;
            return;
        }

        Uri = uri;
        Value = value;
    }

    public bool IsMileage { get; }

    public string? Uri { get; }

    public Category? Value { get; }

    public bool IsExpanded => Value is not null;

    public string? NominalCode =>
        Uri is string uri ? CategoryReference.ExtractNominalCode(uri) : null;
}
