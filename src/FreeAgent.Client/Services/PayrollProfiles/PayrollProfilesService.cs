using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.PayrollProfiles;

/// <summary>
/// Service for read-only payroll profiles in a tax year.
/// </summary>
public sealed class PayrollProfilesService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the payroll profiles service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal PayrollProfilesService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all payroll profiles for the given tax year end.
    /// </summary>
    /// <param name="taxYearEnd">Tax year end (for example 2021 for April 2020–March 2021).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payroll profiles for the tax year</returns>
    public async Task<IReadOnlyList<PayrollProfile>> ListAsync(
        int taxYearEnd,
        CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<PayrollProfilesListResponse>(
            BuildTaxYearEndpoint(taxYearEnd),
            cancellationToken);

        if (response.Profiles is null)
        {
            throw new FreeAgentApiException("Payroll profiles missing from API response");
        }

        return response.Profiles;
    }

    /// <summary>
    /// Lists payroll profiles for a particular user in the given tax year end.
    /// </summary>
    /// <param name="taxYearEnd">Tax year end.</param>
    /// <param name="user">User resource reference for the <c>user</c> query parameter.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching payroll profiles (typically zero or one)</returns>
    public async Task<IReadOnlyList<PayrollProfile>> ListProfilesForUserAsync(
        int taxYearEnd,
        UserReference user,
        CancellationToken cancellationToken = default)
    {
        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            BuildTaxYearEndpoint(taxYearEnd),
            [new KeyValuePair<string, string>("user", user.Uri)]);
        var response = await _requestClient.GetAsync<PayrollProfilesListResponse>(endpoint, cancellationToken);

        if (response.Profiles is null)
        {
            throw new FreeAgentApiException("Payroll profiles missing from API response");
        }

        return response.Profiles;
    }

    private static string BuildTaxYearEndpoint(int taxYearEnd) => $"payroll_profiles/{taxYearEnd}";
}
