using System.Globalization;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.VatReturns;

/// <summary>
/// Service for VAT return read endpoints.
/// </summary>
public sealed class VatReturnsService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", System.Text.Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the VAT returns service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal VatReturnsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists VAT returns for the company.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>VAT returns returned by FreeAgent</returns>
    public async Task<IReadOnlyList<VatReturn>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<VatReturnsListResponse>("vat_returns", cancellationToken);

        if (response.VatReturns is null)
        {
            throw new FreeAgentApiException("VAT returns data missing from API response");
        }

        return response.VatReturns;
    }

    /// <summary>
    /// Gets a VAT return for the period ending on the specified date.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>VAT return returned by FreeAgent</returns>
    public async Task<VatReturn> GetVatReturnAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<VatReturnResponse>(
            BuildPeriodEndpoint(periodEndsOn),
            cancellationToken);

        if (response.VatReturn is null)
        {
            throw new FreeAgentApiException("VAT return data missing from API response");
        }

        return response.VatReturn;
    }

    /// <summary>
    /// Marks a VAT return as filed.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated VAT return</returns>
    public Task<VatReturn> MarkAsFiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionReturnAsync(periodEndsOn, "mark_as_filed", cancellationToken);

    /// <summary>
    /// Marks a VAT return as unfiled.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated VAT return</returns>
    public Task<VatReturn> MarkAsUnfiledAsync(DateOnly periodEndsOn, CancellationToken cancellationToken = default) =>
        TransitionReturnAsync(periodEndsOn, "mark_as_unfiled", cancellationToken);

    /// <summary>
    /// Marks a VAT return payment as paid.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="paymentDueOn">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated VAT return</returns>
    public Task<VatReturn> MarkPaymentAsPaidAsync(
        DateOnly periodEndsOn,
        DateOnly paymentDueOn,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(periodEndsOn, paymentDueOn, "mark_as_paid", cancellationToken);

    /// <summary>
    /// Marks a VAT return payment as unpaid.
    /// </summary>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="paymentDueOn">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated VAT return</returns>
    public Task<VatReturn> MarkPaymentAsUnpaidAsync(
        DateOnly periodEndsOn,
        DateOnly paymentDueOn,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(periodEndsOn, paymentDueOn, "mark_as_unpaid", cancellationToken);

    private async Task<VatReturn> TransitionReturnAsync(
        DateOnly periodEndsOn,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var endpoint = $"{BuildPeriodEndpoint(periodEndsOn)}/{action}";
        var response = await _requestClient.PutAsync<VatReturnResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.VatReturn is null)
        {
            throw new FreeAgentApiException("VAT return data missing from API response");
        }

        return response.VatReturn;
    }

    private async Task<VatReturn> TransitionPaymentAsync(
        DateOnly periodEndsOn,
        DateOnly paymentDueOn,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var paymentSegment = paymentDueOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endpoint = $"{BuildPeriodEndpoint(periodEndsOn)}/payments/{paymentSegment}/{action}";

        var response = await _requestClient.PutAsync<VatReturnResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.VatReturn is null)
        {
            throw new FreeAgentApiException("VAT return data missing from API response");
        }

        return response.VatReturn;
    }

    private static string BuildPeriodEndpoint(DateOnly periodEndsOn) =>
        $"vat_returns/{periodEndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
}
