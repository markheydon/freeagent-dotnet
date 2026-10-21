using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.CisSettings;
using CisSettingsModel = FreeAgent.Client.Models.CisSettings.CisSettings;

namespace FreeAgent.Client.Services.CisSettings;

/// <summary>
/// Service for Construction Industry Scheme company settings.
/// </summary>
public sealed class CisSettingsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the CIS settings service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CisSettingsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets CIS settings for the company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CIS settings returned by FreeAgent</returns>
    public async Task<CisSettingsModel> GetCisSettingsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<CisSettingsResponse>("cis_settings", cancellationToken);

        if (response.CisSettings is null)
        {
            throw new FreeAgentApiException("CIS settings data missing from API response");
        }

        return response.CisSettings;
    }

    /// <summary>
    /// Updates CIS settings. Unspecified sections are left unchanged; specify null on a section to deregister or clear prior deductions.
    /// </summary>
    /// <param name="update">Partial CIS settings update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated CIS settings</returns>
    public async Task<CisSettingsModel> UpdateCisSettingsAsync(CisSettingsUpdate update, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(update);

        var content = CisSettingsUpdateSerializer.CreateContent(update);
        var response = await _requestClient.PutAsync<CisSettingsResponse>("cis_settings", content, cancellationToken);

        if (response.CisSettings is null)
        {
            throw new FreeAgentApiException("CIS settings data missing from API response");
        }

        return response.CisSettings;
    }
}
