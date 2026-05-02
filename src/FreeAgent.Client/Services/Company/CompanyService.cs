using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Company;
using CompanyModel = FreeAgent.Client.Models.Company.Company;

namespace FreeAgent.Client.Services.Company;

/// <summary>
/// Service for interacting with the FreeAgent Company API.
/// </summary>
public sealed class CompanyService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the CompanyService.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal CompanyService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Gets the company information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Company information</returns>
    public async Task<CompanyModel> GetCompanyAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<CompanyResponse>("company", cancellationToken);

        if (response.Company == null)
        {
            throw new FreeAgentApiException("Company data missing from API response");
        }

        return response.Company;
    }

    /// <summary>
    /// Lists all business categories that can be used for company classification.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Business categories</returns>
    public async Task<IReadOnlyList<string>> GetBusinessCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<BusinessCategoriesResponse>("company/business_categories", cancellationToken);

        if (response.BusinessCategories is null)
        {
            throw new FreeAgentApiException("Business categories missing from API response");
        }

        return response.BusinessCategories;
    }

    /// <summary>
    /// Gets upcoming tax timeline events for the authenticated company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tax timeline items</returns>
    /// <remarks>
    /// Minimum FreeAgent access level: Tax, Accounting and Users.
    /// </remarks>
    public async Task<IReadOnlyList<TaxTimelineItem>> GetTaxTimelineAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<TaxTimelineResponse>("company/tax_timeline", cancellationToken);

        if (response.TimelineItems is null)
        {
            throw new FreeAgentApiException("Tax timeline items missing from API response");
        }

        return response.TimelineItems;
    }
}
