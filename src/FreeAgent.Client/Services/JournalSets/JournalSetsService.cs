using System.Globalization;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Services.JournalSets;

/// <summary>
/// Service for interacting with FreeAgent journal sets.
/// </summary>
public sealed class JournalSetsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the journal sets service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal JournalSetsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists journal sets, optionally filtered by date range, update time, or tag.
    /// </summary>
    /// <param name="fromDate">Optional start date (<c>from_date</c> query parameter).</param>
    /// <param name="toDate">Optional end date (<c>to_date</c> query parameter).</param>
    /// <param name="updatedSince">Optional updated-since filter (<c>updated_since</c> query parameter).</param>
    /// <param name="tag">Optional tag filter (<c>tag</c> query parameter).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All journal sets returned by FreeAgent for the filters</returns>
    public async Task<IReadOnlyList<JournalSet>> ListAsync(
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        string? tag = null,
        CancellationToken cancellationToken = default)
    {
        var queryParameters = new List<KeyValuePair<string, string>>();

        if (fromDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "from_date",
                fromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (toDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "to_date",
                toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (updatedSince is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "updated_since",
                updatedSince.Value.ToString("o", CultureInfo.InvariantCulture)));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            queryParameters.Add(new KeyValuePair<string, string>("tag", tag));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("journal_sets", queryParameters);
        var response = await _requestClient.GetAsync<JournalSetsListResponse>(endpoint, cancellationToken);

        if (response.JournalSets is null)
        {
            throw new FreeAgentApiException("Journal sets data missing from API response");
        }

        return response.JournalSets;
    }

    /// <summary>
    /// Gets a single journal set by identifier.
    /// </summary>
    /// <param name="journalSetId">Journal set identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Journal set details</returns>
    public async Task<JournalSet> GetJournalSetAsync(long journalSetId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(journalSetId);

        var response = await _requestClient.GetAsync<JournalSetResponse>(
            $"journal_sets/{journalSetId}",
            cancellationToken);

        if (response.JournalSet is null)
        {
            throw new FreeAgentApiException("Journal set data missing from API response");
        }

        return response.JournalSet;
    }

    /// <summary>
    /// Gets the opening balances journal set.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Opening balances journal set</returns>
    public async Task<JournalSet> GetOpeningBalancesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<JournalSetResponse>(
            "journal_sets/opening_balances",
            cancellationToken);

        if (response.JournalSet is null)
        {
            throw new FreeAgentApiException("Journal set data missing from API response");
        }

        return response.JournalSet;
    }

    /// <summary>
    /// Creates a journal set.
    /// </summary>
    /// <param name="request">Journal set attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created journal set</returns>
    public async Task<JournalSet> CreateJournalSetAsync(
        CreateJournalSetRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        JournalEntryWriteValidator.ValidateForCreate(request.JournalEntries);

        var content = FreeAgentJsonSerializer.CreateContent(new JournalSetRequest<JournalSetWritePayload>
        {
            JournalSet = JournalSetWritePayload.FromCreate(request, _requestClient.Environment)
        });

        var response = await _requestClient.PostAsync<JournalSetResponse>("journal_sets", content, cancellationToken);

        if (response.JournalSet is null)
        {
            throw new FreeAgentApiException("Journal set data missing from API response");
        }

        return response.JournalSet;
    }

    /// <summary>
    /// Updates a journal set.
    /// </summary>
    /// <param name="journalSetId">Journal set identifier from the resource URL</param>
    /// <param name="request">Journal set attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated journal set</returns>
    public async Task<JournalSet> UpdateJournalSetAsync(
        long journalSetId,
        UpdateJournalSetRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(journalSetId);
        ArgumentNullException.ThrowIfNull(request);

        JournalEntryWriteValidator.ValidateForUpdate(request.JournalEntries);

        var content = FreeAgentJsonSerializer.CreateContent(new JournalSetRequest<JournalSetWritePayload>
        {
            JournalSet = JournalSetWritePayload.FromUpdate(request, _requestClient.Environment, journalSetId)
        });

        var response = await _requestClient.PutAsync<JournalSetResponse>(
            $"journal_sets/{journalSetId}",
            content,
            cancellationToken);

        if (response.JournalSet is null)
        {
            throw new FreeAgentApiException("Journal set data missing from API response");
        }

        return response.JournalSet;
    }

    /// <summary>
    /// Deletes a journal set.
    /// </summary>
    /// <param name="journalSetId">Journal set identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeleteJournalSetAsync(long journalSetId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(journalSetId);

        return _requestClient.DeleteAsync($"journal_sets/{journalSetId}", cancellationToken);
    }
}
