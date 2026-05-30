using System.Globalization;

namespace FreeAgent.Client.Infrastructure.Http;

internal static class FreeAgentPaginationHelper
{
    private const string TotalCountHeaderName = "X-Total-Count";

    public static int GetTotalCountOrEstimate<T>(
        FreeAgentHttpResponse<T> response,
        int page,
        int perPage,
        int itemsOnPage)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(itemsOnPage, 0);

        var headerTotal = TryGetHeaderTotal(response);
        if (headerTotal is not null)
        {
            return headerTotal.Value;
        }

        return EstimateTotalWithoutHeader(page, perPage, itemsOnPage);
    }

    private static int? TryGetHeaderTotal<T>(FreeAgentHttpResponse<T> response)
    {
        var headerValues = response.GetHeaderValues(TotalCountHeaderName);
        if (headerValues is null || headerValues.Count == 0)
        {
            return null;
        }

        if (!int.TryParse(headerValues[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var total))
        {
            return null;
        }

        return total >= 0 ? total : null;
    }

    private static int EstimateTotalWithoutHeader(int page, int perPage, int itemsOnPage)
    {
        // A full page with no total header implies there may be another page.
        if (itemsOnPage == perPage)
        {
            return (page * perPage) + 1;
        }

        return ((page - 1) * perPage) + itemsOnPage;
    }
}
