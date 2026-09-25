using System.Globalization;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Clamps Turpinverse canon dates to FreeAgent company constraints.
/// </summary>
internal static class TurpinverseSalesDateSupport
{
    public static (DateOnly DatedOn, DateOnly DueOn) ClampInvoiceDates(
        DateOnly datedOn,
        DateOnly dueOn,
        DateOnly minimumDocumentDate)
    {
        var clampedDatedOn = ClampToMinimum(datedOn, minimumDocumentDate);
        var clampedDueOn = ClampToMinimum(dueOn, minimumDocumentDate);
        if (clampedDueOn < clampedDatedOn)
        {
            clampedDueOn = clampedDatedOn;
        }

        return (clampedDatedOn, clampedDueOn);
    }

    public static DateOnly ClampToMinimum(DateOnly date, DateOnly minimumDocumentDate) =>
        date < minimumDocumentDate ? minimumDocumentDate : date;

    public static DateOnly ParseCanonDate(string value, string fieldName)
    {
        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"Turpinverse field '{fieldName}' is not a valid date: '{value}'.");
    }
}
