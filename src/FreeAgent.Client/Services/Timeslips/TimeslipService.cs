using System.Globalization;
using System.Text;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Timeslips;

namespace FreeAgent.Client.Services.Timeslips;

/// <summary>
/// Service for interacting with FreeAgent timeslips.
/// </summary>
/// <remarks>
/// Timeslips require <c>task</c>, <c>project</c>, and <c>user</c> on create.
/// See also <see cref="Tasks.TaskService"/>, <see cref="Projects.ProjectService"/>, and <see cref="Users.UserService"/>.
/// </remarks>
public sealed class TimeslipService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the timeslip service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal TimeslipService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of timeslips.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="TimeslipViews.Unbilled"/>)</param>
    /// <param name="fromDate">Return timeslips dated on or after this date</param>
    /// <param name="toDate">Return timeslips dated on or before this date</param>
    /// <param name="updatedSince">Return timeslips updated on or after this timestamp</param>
    /// <param name="nested">When <see langword="true"/>, return linked resources as nested JSON objects</param>
    /// <param name="user">Filter by user resource reference</param>
    /// <param name="userId">Filter by user identifier (equivalent to <c>client.Urls.User(userId)</c>)</param>
    /// <param name="task">Filter by task resource reference</param>
    /// <param name="taskId">Filter by task identifier (equivalent to <c>client.Urls.Task(taskId)</c>)</param>
    /// <param name="project">Filter by project resource reference</param>
    /// <param name="projectId">Filter by project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated timeslips response</returns>
    public async Task<PaginatedResponse<Timeslip>> ListAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        bool? nested = null,
        UserReference? user = null,
        long? userId = null,
        TaskReference? task = null,
        long? taskId = null,
        ProjectReference? project = null,
        long? projectId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(perPage, 100);

        var queryParameters = new List<KeyValuePair<string, string>>
        {
            new("page", page.ToString(CultureInfo.InvariantCulture)),
            new("per_page", perPage.ToString(CultureInfo.InvariantCulture))
        };

        if (!string.IsNullOrWhiteSpace(view))
        {
            queryParameters.Add(new KeyValuePair<string, string>("view", view));
        }

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
                updatedSince.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture)));
        }

        if (nested is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "nested",
                nested.Value ? "true" : "false"));
        }

        var userFilter = ResolveUserFilter(user, userId);
        if (userFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("user", userFilter));
        }

        var taskFilter = ResolveTaskFilter(task, taskId);
        if (taskFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("task", taskFilter));
        }

        var projectFilter = ResolveProjectFilter(project, projectId);
        if (projectFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("project", projectFilter));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("timeslips", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<TimeslipsResponse>(endpoint, cancellationToken);

        if (response.Data.Timeslips is null)
        {
            throw new FreeAgentApiException("Timeslips data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(response, page, perPage, response.Data.Timeslips.Count);

        return new PaginatedResponse<Timeslip>(
            page,
            perPage,
            total,
            response.Data.Timeslips);
    }

    /// <summary>
    /// Lists all timeslips across all pages.
    /// </summary>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="TimeslipViews.Unbilled"/>)</param>
    /// <param name="fromDate">Return timeslips dated on or after this date</param>
    /// <param name="toDate">Return timeslips dated on or before this date</param>
    /// <param name="updatedSince">Return timeslips updated on or after this timestamp</param>
    /// <param name="nested">When <see langword="true"/>, return linked resources as nested JSON objects</param>
    /// <param name="user">Filter by user resource reference</param>
    /// <param name="userId">Filter by user identifier (equivalent to <c>client.Urls.User(userId)</c>)</param>
    /// <param name="task">Filter by task resource reference</param>
    /// <param name="taskId">Filter by task identifier (equivalent to <c>client.Urls.Task(taskId)</c>)</param>
    /// <param name="project">Filter by project resource reference</param>
    /// <param name="projectId">Filter by project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async stream of timeslips</returns>
    public async IAsyncEnumerable<Timeslip> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        bool? nested = null,
        UserReference? user = null,
        long? userId = null,
        TaskReference? task = null,
        long? taskId = null,
        ProjectReference? project = null,
        long? projectId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var timeslipsPage = await ListAsync(
                page,
                perPage,
                view,
                fromDate,
                toDate,
                updatedSince,
                nested,
                user,
                userId,
                task,
                taskId,
                project,
                projectId,
                cancellationToken);

            foreach (var timeslip in timeslipsPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return timeslip;
            }

            if (!timeslipsPage.HasNextPage)
            {
                yield break;
            }

            page = timeslipsPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single timeslip by identifier.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Timeslip details</returns>
    public Task<Timeslip> GetTimeslipAsync(long timeslipId, CancellationToken cancellationToken = default) =>
        GetTimeslipAsync(timeslipId, nested: null, options: null, cancellationToken);

    /// <summary>
    /// Gets a single timeslip by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Timeslip details</returns>
    public Task<Timeslip> GetTimeslipAsync(
        long timeslipId,
        TimeslipGetOptions? options,
        CancellationToken cancellationToken = default) =>
        GetTimeslipAsync(timeslipId, nested: null, options, cancellationToken);

    /// <summary>
    /// Gets a single timeslip by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier from the resource URL</param>
    /// <param name="nested">When <see langword="true"/>, return linked resources as nested JSON objects</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Timeslip details</returns>
    public async Task<Timeslip> GetTimeslipAsync(
        long timeslipId,
        bool? nested,
        TimeslipGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeslipId);

        var queryParameters = new List<KeyValuePair<string, string>>();
        if (nested is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "nested",
                nested.Value ? "true" : "false"));
        }

        var endpoint = queryParameters.Count == 0
            ? $"timeslips/{timeslipId}"
            : FreeAgentQueryStringBuilder.BuildEndpoint($"timeslips/{timeslipId}", queryParameters);

        var response = await _requestClient.GetAsync<TimeslipResponse>(endpoint, cancellationToken);

        if (response.Timeslip is null)
        {
            throw new FreeAgentApiException("Timeslip data missing from API response");
        }

        await LinkedResourceHydration.HydrateTaskAsync(
            response.Timeslip,
            _requestClient,
            options?.IncludeTask == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateProjectAsync(
            response.Timeslip,
            _requestClient,
            options?.IncludeProject == true,
            cancellationToken);

        await LinkedResourceHydration.HydrateUserAsync(
            response.Timeslip,
            _requestClient,
            options?.IncludeUser == true,
            cancellationToken);

        return response.Timeslip;
    }

    /// <summary>
    /// Creates a timeslip.
    /// </summary>
    /// <param name="timeslip">Timeslip attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created timeslip</returns>
    public async Task<Timeslip> CreateTimeslipAsync(Timeslip timeslip, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(timeslip);

        var content = FreeAgentJsonSerializer.CreateContent(
            new TimeslipRequest { Timeslip = TimeslipWritePayload.FromTimeslip(timeslip, _requestClient.Environment) });

        var response = await _requestClient.PostAsync<TimeslipResponse>("timeslips", content, cancellationToken);

        if (response.Timeslip is null)
        {
            throw new FreeAgentApiException("Timeslip data missing from API response");
        }

        return response.Timeslip;
    }

    /// <summary>
    /// Creates multiple timeslips in one request.
    /// </summary>
    /// <param name="timeslips">Timeslip attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created timeslips</returns>
    public async Task<IReadOnlyList<Timeslip>> CreateTimeslipsAsync(
        IReadOnlyList<Timeslip> timeslips,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(timeslips);

        if (timeslips.Count == 0)
        {
            throw new ArgumentException("At least one timeslip is required.", nameof(timeslips));
        }

        var content = FreeAgentJsonSerializer.CreateContent(new TimeslipsBatchRequest
        {
            Timeslips = timeslips.Select(t => TimeslipWritePayload.FromTimeslip(t, _requestClient.Environment)).ToList()
        });

        var response = await _requestClient.PostAsync<TimeslipsResponse>("timeslips", content, cancellationToken);

        if (response.Timeslips is null)
        {
            throw new FreeAgentApiException("Timeslips data missing from API response");
        }

        return response.Timeslips;
    }

    /// <summary>
    /// Updates a timeslip.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier from the resource URL</param>
    /// <param name="timeslip">Timeslip attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated timeslip</returns>
    public async Task<Timeslip> UpdateTimeslipAsync(
        long timeslipId,
        Timeslip timeslip,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeslipId);
        ArgumentNullException.ThrowIfNull(timeslip);

        var content = FreeAgentJsonSerializer.CreateContent(
            new TimeslipRequest { Timeslip = TimeslipWritePayload.FromTimeslip(timeslip, _requestClient.Environment) });

        var response = await _requestClient.PutAsync<TimeslipResponse>($"timeslips/{timeslipId}", content, cancellationToken);

        if (response.Timeslip is null)
        {
            throw new FreeAgentApiException("Timeslip data missing from API response");
        }

        return response.Timeslip;
    }

    /// <summary>
    /// Deletes a timeslip.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeleteTimeslipAsync(long timeslipId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeslipId);

        return _requestClient.DeleteAsync($"timeslips/{timeslipId}", cancellationToken);
    }

    /// <summary>
    /// Starts a running timer on a timeslip.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated timeslip</returns>
    public async Task<Timeslip> StartTimerAsync(long timeslipId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeslipId);

        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await _requestClient.PostAsync<TimeslipResponse>($"timeslips/{timeslipId}/timer", content, cancellationToken);

        if (response.Timeslip is null)
        {
            throw new FreeAgentApiException("Timeslip data missing from API response");
        }

        return response.Timeslip;
    }

    /// <summary>
    /// Stops a running timer on a timeslip.
    /// </summary>
    /// <param name="timeslipId">Timeslip identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated timeslip</returns>
    public async Task<Timeslip> StopTimerAsync(long timeslipId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeslipId);

        var response = await _requestClient.DeleteAsync<TimeslipResponse>($"timeslips/{timeslipId}/timer", cancellationToken);

        if (response.Timeslip is null)
        {
            throw new FreeAgentApiException("Timeslip data missing from API response");
        }

        return response.Timeslip;
    }

    private string? ResolveUserFilter(UserReference? user, long? userId)
    {
        if (user is not null && userId is not null)
        {
            throw new ArgumentException("Specify either user or userId, not both.", nameof(user));
        }

        if (user is not null)
        {
            return user.Value.Uri;
        }

        if (userId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId.Value);
        return UserReference.ForEnvironment(_requestClient.Environment, userId.Value).Uri;
    }

    private string? ResolveTaskFilter(TaskReference? task, long? taskId)
    {
        if (task is not null && taskId is not null)
        {
            throw new ArgumentException("Specify either task or taskId, not both.", nameof(task));
        }

        if (task is not null)
        {
            return task.Value.Uri;
        }

        if (taskId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId.Value);
        return TaskReference.ForEnvironment(_requestClient.Environment, taskId.Value).Uri;
    }

    private string? ResolveProjectFilter(ProjectReference? project, long? projectId)
    {
        if (project is not null && projectId is not null)
        {
            throw new ArgumentException("Specify either project or projectId, not both.", nameof(project));
        }

        if (project is not null)
        {
            return project.Value.Uri;
        }

        if (projectId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId.Value);
        return ProjectReference.ForEnvironment(_requestClient.Environment, projectId.Value).Uri;
    }
}
