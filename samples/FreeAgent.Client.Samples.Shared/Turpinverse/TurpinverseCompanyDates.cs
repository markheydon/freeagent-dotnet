using FreeAgent.Client;

namespace FreeAgent.Client.Samples.Shared.Turpinverse;

/// <summary>
/// Resolves and caches the minimum document date allowed by the connected FreeAgent company.
/// </summary>
public sealed class TurpinverseCompanyDates
{
    private DateOnly? _minimumDocumentDate;

    /// <summary>
    /// Gets the earliest date that invoices, estimates, and credit notes may use.
    /// </summary>
    /// <param name="client">Authenticated FreeAgent client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Company start date, or 1 January 2026 when the API omits it.</returns>
    public async Task<DateOnly> GetMinimumDocumentDateAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (_minimumDocumentDate is not null)
        {
            return _minimumDocumentDate.Value;
        }

        var company = await client.Company.GetCompanyAsync(cancellationToken).ConfigureAwait(false);
        _minimumDocumentDate = company.CompanyStartDate
            ?? company.TradingStartDate
            ?? new DateOnly(2026, 1, 1);
        return _minimumDocumentDate.Value;
    }

    /// <summary>
    /// Clears the cached minimum document date.
    /// </summary>
    public void ClearCache() => _minimumDocumentDate = null;
}
