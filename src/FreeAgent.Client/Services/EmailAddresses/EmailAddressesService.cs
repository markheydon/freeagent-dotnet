using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.EmailAddresses;

namespace FreeAgent.Client.Services.EmailAddresses;

/// <summary>
/// Service for interacting with FreeAgent verified sender email addresses.
/// </summary>
public sealed class EmailAddressesService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the email addresses service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal EmailAddressesService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists verified sender email addresses for the authenticated company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verified sender email addresses</returns>
    /// <remarks>
    /// Minimum FreeAgent access level: Time.
    /// Each entry is formatted as <c>Name &lt;email@example.com&gt;</c>.
    /// </remarks>
    public async Task<IReadOnlyList<string>> GetEmailAddressesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<EmailAddressesResponse>("email_addresses", cancellationToken);

        if (response.EmailAddresses is null)
        {
            throw new FreeAgentApiException("Email addresses missing from API response");
        }

        return response.EmailAddresses;
    }
}
