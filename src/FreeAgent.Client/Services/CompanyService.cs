using FreeAgent.Client.Http;
using FreeAgent.Client.Models;

namespace FreeAgent.Client.Services;

/// <summary>
/// Service for interacting with the FreeAgent Company API.
/// </summary>
public class CompanyService
{
    private readonly FreeAgentHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the CompanyService.
    /// </summary>
    /// <param name="httpClient">FreeAgent HTTP client</param>
    public CompanyService(FreeAgentHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Gets the company information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company information</returns>
    public async Task<Company> GetCompanyAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync<CompanyResponse>("/company", cancellationToken);
        return response.Company;
    }
}
