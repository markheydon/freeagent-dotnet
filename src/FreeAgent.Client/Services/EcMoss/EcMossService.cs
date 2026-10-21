using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
namespace FreeAgent.Client.Services.EcMoss;

/// <summary>
/// EC VAT MOSS helper endpoints.
/// </summary>
public sealed class EcMossService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the EC VAT MOSS service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal EcMossService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists MOSS sales tax rates for a country and transaction date.
    /// </summary>
    /// <param name="country">EU member place of supply country name.</param>
    /// <param name="date">Transaction date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>MOSS sales tax rate bands for the country and date.</returns>
    public async Task<IReadOnlyList<EcMossSalesTaxRate>> ListSalesTaxRatesAsync(
        EcMossPlaceOfSupply country,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint(
            "ec_moss/sales_tax_rates",
            [
                new KeyValuePair<string, string>("country", EnumWireValue.Get(country)),
                new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))
            ]);

        var response = await _requestClient.GetAsync<EcMossSalesTaxRatesListResponse>(endpoint, cancellationToken);

        if (response.SalesTaxRates is null)
        {
            throw new FreeAgentApiException("EC VAT MOSS sales tax rates data missing from API response");
        }

        return response.SalesTaxRates;
    }
}
