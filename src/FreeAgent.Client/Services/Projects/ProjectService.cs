using System.Globalization;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.Projects;

/// <summary>
/// Service for interacting with FreeAgent projects.
/// </summary>
public sealed class ProjectService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the project service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal ProjectService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets one page of projects.
    /// </summary>
    /// <param name="page">1-based page number</param>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="ProjectViews.Active"/>)</param>
    /// <param name="sort">Sort field (name, contact_name, contact_display_name, created_at, updated_at); prefix with <c>-</c> for descending</param>
    /// <param name="contact">Filter by billing contact resource reference</param>
    /// <param name="contactId">Filter by billing contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="nested">When <see langword="true"/>, return full contact details nested in each project</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated projects response</returns>
    public async Task<PaginatedResponse<Project>> GetProjectsPageAsync(
        int page = 1,
        int perPage = 25,
        string? view = null,
        string? sort = null,
        ContactReference? contact = null,
        long? contactId = null,
        bool? nested = null,
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

        var contactFilter = ResolveContactFilter(contact, contactId);
        if (contactFilter is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("contact", contactFilter));
        }

        if (nested is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "nested",
                nested.Value ? "true" : "false"));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("projects", queryParameters);

        var response = await _requestClient.GetWithMetadataAsync<ProjectsResponse>(endpoint, cancellationToken);

        if (response.Data.Projects is null)
        {
            throw new FreeAgentApiException("Projects data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(response, page, perPage, response.Data.Projects.Count);

        return new PaginatedResponse<Project>(
            page,
            perPage,
            total,
            response.Data.Projects);
    }

    /// <summary>
    /// Iterates all projects across all pages.
    /// </summary>
    /// <param name="perPage">Items per page (maximum 100)</param>
    /// <param name="view">Optional view filter (for example: <see cref="ProjectViews.Active"/>)</param>
    /// <param name="sort">Sort field (name, contact_name, contact_display_name, created_at, updated_at); prefix with <c>-</c> for descending</param>
    /// <param name="contact">Filter by billing contact resource reference</param>
    /// <param name="contactId">Filter by billing contact identifier (equivalent to <c>client.Urls.Contact(contactId)</c>)</param>
    /// <param name="nested">When <see langword="true"/>, return full contact details nested in each project</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async stream of projects</returns>
    public async IAsyncEnumerable<Project> GetAllProjectsAsync(
        int perPage = 25,
        string? view = null,
        string? sort = null,
        ContactReference? contact = null,
        long? contactId = null,
        bool? nested = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var projectsPage = await GetProjectsPageAsync(page, perPage, view, sort, contact, contactId, nested, cancellationToken);

            foreach (var project in projectsPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return project;
            }

            if (!projectsPage.HasNextPage)
            {
                yield break;
            }

            page = projectsPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single project by identifier.
    /// </summary>
    /// <param name="projectId">Project identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project details</returns>
    public Task<Project> GetProjectAsync(long projectId, CancellationToken cancellationToken = default) =>
        GetProjectAsync(projectId, options: null, cancellationToken);

    /// <summary>
    /// Gets a single project by identifier with optional linked-resource hydration.
    /// </summary>
    /// <param name="projectId">Project identifier from the resource URL</param>
    /// <param name="options">Optional hydration settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Project details</returns>
    public async Task<Project> GetProjectAsync(
        long projectId,
        ProjectGetOptions? options,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId);

        var response = await _requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        await LinkedResourceHydration.HydrateBillingContactAsync(
            response.Project,
            _requestClient,
            options?.IncludeBillingContact == true,
            cancellationToken);

        return response.Project;
    }

    /// <summary>
    /// Creates a project.
    /// </summary>
    /// <param name="project">Project attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created project</returns>
    public async Task<Project> CreateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        var content = FreeAgentJsonSerializer.CreateContent(new ProjectRequest { Project = ProjectWritePayload.FromProject(project) });
        var response = await _requestClient.PostAsync<ProjectResponse>("projects", content, cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        return response.Project;
    }

    /// <summary>
    /// Updates a project.
    /// </summary>
    /// <param name="projectId">Project identifier from the resource URL</param>
    /// <param name="project">Project attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated project</returns>
    public async Task<Project> UpdateProjectAsync(long projectId, Project project, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId);
        ArgumentNullException.ThrowIfNull(project);

        var content = FreeAgentJsonSerializer.CreateContent(new ProjectRequest { Project = ProjectWritePayload.FromProject(project) });
        var response = await _requestClient.PutAsync<ProjectResponse>($"projects/{projectId}", content, cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        return response.Project;
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="projectId">Project identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteProjectAsync(long projectId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(projectId);

        return _requestClient.DeleteAsync($"projects/{projectId}", cancellationToken);
    }

    private string? ResolveContactFilter(ContactReference? contact, long? contactId)
    {
        if (contact is not null)
        {
            return contact.Value.Uri;
        }

        if (contactId is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(contactId.Value);
        return ContactReference.ForEnvironment(_requestClient.Environment, contactId.Value).Uri;
    }
}
