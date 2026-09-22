using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Tasks;
using TaskModel = FreeAgent.Client.Models.Tasks.Task;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Optional hydration of linked resources after single-resource GET responses.
/// </summary>
internal static class LinkedResourceHydration
{
    public static async System.Threading.Tasks.Task HydrateBillingContactAsync(
        Project project,
        IFreeAgentRequestClient requestClient,
        bool includeBillingContact,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeBillingContact || project.Contact is not null || project.ContactId is not long contactId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ContactResponse>($"contacts/{contactId}", cancellationToken);

        if (response.Contact is null)
        {
            throw new FreeAgentApiException("Contact data missing from API response");
        }

        project.AttachContact(response.Contact);
    }

    public static async System.Threading.Tasks.Task HydrateProjectAsync(
        TaskModel task,
        IFreeAgentRequestClient requestClient,
        bool includeProject,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(requestClient);

        if (!includeProject || task.Project is not null || task.ProjectId is not long projectId)
        {
            return;
        }

        var response = await requestClient.GetAsync<ProjectResponse>($"projects/{projectId}", cancellationToken);

        if (response.Project is null)
        {
            throw new FreeAgentApiException("Project data missing from API response");
        }

        task.AttachProject(response.Project);
    }
}
