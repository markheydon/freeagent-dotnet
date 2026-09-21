using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Projects;

namespace FreeAgent.Client.Infrastructure.Http;

/// <summary>
/// Optional hydration of linked resources after single-resource GET responses.
/// </summary>
internal static class LinkedResourceHydration
{
    public static async Task HydrateBillingContactAsync(
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
}
