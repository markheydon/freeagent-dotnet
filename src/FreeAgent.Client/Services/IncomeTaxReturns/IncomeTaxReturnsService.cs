using System.Globalization;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
namespace FreeAgent.Client.Services.IncomeTaxReturns;

/// <summary>
/// Service for income tax return endpoints (user-scoped self assessment returns on the wire).
/// </summary>
public sealed class IncomeTaxReturnsService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", System.Text.Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the income tax returns service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal IncomeTaxReturnsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists income tax returns for a user.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Income tax returns returned by FreeAgent</returns>
    public async Task<IReadOnlyList<IncomeTaxReturn>> ListAsync(long userId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

        var response = await _requestClient.GetAsync<IncomeTaxReturnsListResponse>(
            BuildCollectionEndpoint(userId),
            cancellationToken);

        if (response.IncomeTaxReturns is null)
        {
            throw new FreeAgentApiException("Income tax returns data missing from API response");
        }

        return response.IncomeTaxReturns;
    }

    /// <summary>
    /// Gets an income tax return for the period ending on the specified date.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Income tax return returned by FreeAgent</returns>
    public async Task<IncomeTaxReturn> GetAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

        var response = await _requestClient.GetAsync<IncomeTaxReturnResponse>(
            BuildPeriodEndpoint(userId, periodEndsOn),
            cancellationToken);

        if (response.IncomeTaxReturn is null)
        {
            throw new FreeAgentApiException("Income tax return data missing from API response");
        }

        return response.IncomeTaxReturn;
    }

    /// <summary>
    /// Marks an income tax return as filed.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated income tax return</returns>
    public Task<IncomeTaxReturn> MarkAsFiledAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken = default) =>
        TransitionReturnAsync(userId, periodEndsOn, "mark_as_filed", cancellationToken);

    /// <summary>
    /// Marks an income tax return as unfiled.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated income tax return</returns>
    public Task<IncomeTaxReturn> MarkAsUnfiledAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken = default) =>
        TransitionReturnAsync(userId, periodEndsOn, "mark_as_unfiled", cancellationToken);

    /// <summary>
    /// Marks an income tax return payment as paid.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="paymentDate">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated income tax return</returns>
    public Task<IncomeTaxReturn> MarkPaymentAsPaidAsync(
        long userId,
        DateOnly periodEndsOn,
        DateOnly paymentDate,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(userId, periodEndsOn, paymentDate, "mark_as_paid", cancellationToken);

    /// <summary>
    /// Marks an income tax return payment as unpaid.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="paymentDate">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated income tax return</returns>
    public Task<IncomeTaxReturn> MarkPaymentAsUnpaidAsync(
        long userId,
        DateOnly periodEndsOn,
        DateOnly paymentDate,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(userId, periodEndsOn, paymentDate, "mark_as_unpaid", cancellationToken);

    private async Task<IncomeTaxReturn> TransitionReturnAsync(
        long userId,
        DateOnly periodEndsOn,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var endpoint = $"{BuildPeriodEndpoint(userId, periodEndsOn)}/{action}";
        var response = await _requestClient.PutAsync<IncomeTaxReturnResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.IncomeTaxReturn is null)
        {
            throw new FreeAgentApiException("Income tax return data missing from API response");
        }

        return response.IncomeTaxReturn;
    }

    private async Task<IncomeTaxReturn> TransitionPaymentAsync(
        long userId,
        DateOnly periodEndsOn,
        DateOnly paymentDate,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var paymentSegment = paymentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endpoint =
            $"{BuildPeriodEndpoint(userId, periodEndsOn)}/payments/{paymentSegment}/{action}";

        var response = await _requestClient.PutAsync<IncomeTaxReturnResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.IncomeTaxReturn is null)
        {
            throw new FreeAgentApiException("Income tax return data missing from API response");
        }

        return response.IncomeTaxReturn;
    }

    private static string BuildCollectionEndpoint(long userId) =>
        $"users/{userId}/self_assessment_returns";

    private static string BuildPeriodEndpoint(long userId, DateOnly periodEndsOn) =>
        $"{BuildCollectionEndpoint(userId)}/{periodEndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
}
