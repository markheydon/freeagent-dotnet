using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Shared;
using FreeAgent.Client.Models.Tasks;
namespace FreeAgent.Client.Services.Tasks;

/// <summary>
/// Service for interacting with FreeAgent tasks.
/// </summary>
/// <remarks>
/// Tasks belong to a project. List and create operations scope the parent via <c>project</c> query parameters.
/// See also <see cref="Projects.ProjectService"/> for project operations.
/// </remarks>
public sealed class TaskService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the task service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal TaskService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of tasks.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="TaskViews.Active"/>)</param>
    /// <param name="sort">Sort field (name, project, billing_rate, created_at, updated_at); prefix with <c>-</c> for descending</param>
    /// <param name="updatedSince">Return tasks updated on or after this date</param>
    /// <param name="project">Filter by parent project resource reference</param>
    /// <param name="projectId">Filter by parent project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated tasks response</returns>
    /// <exception cref="ArgumentException"><paramref name="project"/> and <paramref name="projectId"/> are both supplied.</exception>
    public async Task<PaginatedResponse<ProjectTask>> ListAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        string? sort = null,
        DateOnly? updatedSince = null,
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

        if (!string.IsNullOrWhiteSpace(sort))
        {
            queryParameters.Add(new KeyValuePair<string, string>("sort", sort));
        }

        if (updatedSince is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "updated_since",
                updatedSince.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        var projectFilter = ResolveProjectFilter(project, projectId);
        if (projectFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("project", projectFilter));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("tasks", queryParameters);

        var response = await _requestClient.GetWithMetadataAsync<TasksResponse>(endpoint, cancellationToken);

        if (response.Data.Tasks is null)
        {
            throw new FreeAgentApiException("Tasks data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(response, page, perPage, response.Data.Tasks.Count);

        return new PaginatedResponse<ProjectTask>(
            page,
            perPage,
            total,
            response.Data.Tasks);
    }

    /// <summary>
    /// Lists all tasks across all pages.
    /// </summary>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="TaskViews.Active"/>)</param>
    /// <param name="sort">Sort field (name, project, billing_rate, created_at, updated_at); prefix with <c>-</c> for descending</param>
    /// <param name="updatedSince">Return tasks updated on or after this date</param>
    /// <param name="project">Filter by parent project resource reference</param>
    /// <param name="projectId">Filter by parent project identifier (equivalent to <c>client.Urls.Project(projectId)</c>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async stream of tasks</returns>
    public async IAsyncEnumerable<ProjectTask> ListAutoPagingAsync(
        int perPage = 25,
        string? view = null,
        string? sort = null,
        DateOnly? updatedSince = null,
        ProjectReference? project = null,
        long? projectId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var tasksPage = await ListAsync(page, perPage, view, sort, updatedSince, project, projectId, cancellationToken);

            foreach (var task in tasksPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return task;
            }

            if (!tasksPage.HasNextPage)
            {
                yield break;
            }

            page = tasksPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single task by identifier.
    /// </summary>
    /// <param name="taskId">Task identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task details</returns>
    public Task<ProjectTask> GetTaskAsync(long taskId, CancellationToken cancellationToken = default) =>
        GetTaskAsync(taskId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single task by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="taskId">Task identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task details</returns>
    public async Task<ProjectTask> GetTaskAsync(
        long taskId,
        TaskGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);

        var response = await _requestClient.GetAsync<TaskResponse>($"tasks/{taskId}", cancellationToken);

        if (response.Task is null)
        {
            throw new FreeAgentApiException("Task data missing from API response");
        }

        await LinkedResourceHydration.HydrateProjectAsync(
            response.Task,
            _requestClient,
            options?.IncludeProject == true,
            cancellationToken);

        return response.Task;
    }

    /// <summary>
    /// Creates a task under a project.
    /// </summary>
    /// <param name="projectId">Parent project identifier</param>
    /// <param name="task">Task attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created task</returns>
    public Task<ProjectTask> CreateTaskAsync(long projectId, ProjectTask task, CancellationToken cancellationToken = default) =>
        CreateTaskAsync(ProjectReference.ForEnvironment(_requestClient.Environment, projectId), task, cancellationToken);

    /// <summary>
    /// Creates a task under a project.
    /// </summary>
    /// <param name="project">Parent project resource reference</param>
    /// <param name="task">Task attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created task</returns>
    public async Task<ProjectTask> CreateTaskAsync(
        ProjectReference project,
        ProjectTask task,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            "tasks",
            [new KeyValuePair<string, string>("project", project.Uri)]);

        var content = FreeAgentJsonSerializer.CreateContent(new TaskRequest { Task = TaskWritePayload.FromTask(task) });
        var response = await _requestClient.PostAsync<TaskResponse>(endpoint, content, cancellationToken);

        if (response.Task is null)
        {
            throw new FreeAgentApiException("Task data missing from API response");
        }

        return response.Task;
    }

    /// <summary>
    /// Updates a task.
    /// </summary>
    /// <param name="taskId">Task identifier from the resource URL</param>
    /// <param name="task">Task attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated task</returns>
    public async Task<ProjectTask> UpdateTaskAsync(long taskId, ProjectTask task, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);
        ArgumentNullException.ThrowIfNull(task);

        var content = FreeAgentJsonSerializer.CreateContent(new TaskRequest { Task = TaskWritePayload.FromTask(task) });
        var response = await _requestClient.PutAsync<TaskResponse>($"tasks/{taskId}", content, cancellationToken);

        if (response.Task is null)
        {
            throw new FreeAgentApiException("Task data missing from API response");
        }

        return response.Task;
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="taskId">Task identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public System.Threading.Tasks.Task DeleteTaskAsync(long taskId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);

        return _requestClient.DeleteAsync($"tasks/{taskId}", cancellationToken);
    }

    private string? ResolveProjectFilter(ProjectReference? project, long? projectId)
    {
        if (project is not null && projectId is not null)
        {
            throw new ArgumentException(
                "Specify either project or projectId, not both.",
                nameof(project));
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
