using System.Globalization;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.SelfAssessmentReturns;

namespace FreeAgent.Client.Services.SelfAssessmentReturns;

/// <summary>
/// Service for self assessment return endpoints (user-scoped self assessment returns on the wire).
/// </summary>
public sealed class SelfAssessmentReturnsService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", System.Text.Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the self assessment returns service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal SelfAssessmentReturnsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists self assessment returns for a user.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Self assessment returns returned by FreeAgent</returns>
    public async Task<IReadOnlyList<SelfAssessmentReturn>> ListAsync(long userId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

        var response = await _requestClient.GetAsync<SelfAssessmentReturnsListResponse>(
            BuildCollectionEndpoint(userId),
            cancellationToken);

        if (response.SelfAssessmentReturns is null)
        {
            throw new FreeAgentApiException("Self assessment returns data missing from API response");
        }

        return response.SelfAssessmentReturns;
    }

    /// <summary>
    /// Gets an self assessment return for the period ending on the specified date.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Self assessment return returned by FreeAgent</returns>
    public async Task<SelfAssessmentReturn> GetAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

        var response = await _requestClient.GetAsync<SelfAssessmentReturnResponse>(
            BuildPeriodEndpoint(userId, periodEndsOn),
            cancellationToken);

        if (response.SelfAssessmentReturn is null)
        {
            throw new FreeAgentApiException("Self assessment return data missing from API response");
        }

        return response.SelfAssessmentReturn;
    }

    /// <summary>
    /// Marks an self assessment return as filed.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated self assessment return</returns>
    public Task<SelfAssessmentReturn> MarkAsFiledAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken = default) =>
        TransitionReturnAsync(userId, periodEndsOn, "mark_as_filed", cancellationToken);

    /// <summary>
    /// Marks an self assessment return as unfiled.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated self assessment return</returns>
    public Task<SelfAssessmentReturn> MarkAsUnfiledAsync(
        long userId,
        DateOnly periodEndsOn,
        CancellationToken cancellationToken = default) =>
        TransitionReturnAsync(userId, periodEndsOn, "mark_as_unfiled", cancellationToken);

    /// <summary>
    /// Marks an self assessment return payment as paid.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="paymentDate">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated self assessment return</returns>
    public Task<SelfAssessmentReturn> MarkPaymentAsPaidAsync(
        long userId,
        DateOnly periodEndsOn,
        DateOnly paymentDate,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(userId, periodEndsOn, paymentDate, "mark_as_paid", cancellationToken);

    /// <summary>
    /// Marks an self assessment return payment as unpaid.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL.</param>
    /// <param name="periodEndsOn">Period end date used in the API path.</param>
    /// <param name="paymentDate">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated self assessment return</returns>
    public Task<SelfAssessmentReturn> MarkPaymentAsUnpaidAsync(
        long userId,
        DateOnly periodEndsOn,
        DateOnly paymentDate,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(userId, periodEndsOn, paymentDate, "mark_as_unpaid", cancellationToken);

    private async Task<SelfAssessmentReturn> TransitionReturnAsync(
        long userId,
        DateOnly periodEndsOn,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var endpoint = $"{BuildPeriodEndpoint(userId, periodEndsOn)}/{action}";
        var response = await _requestClient.PutAsync<SelfAssessmentReturnResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.SelfAssessmentReturn is null)
        {
            throw new FreeAgentApiException("Self assessment return data missing from API response");
        }

        return response.SelfAssessmentReturn;
    }

    private async Task<SelfAssessmentReturn> TransitionPaymentAsync(
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

        var response = await _requestClient.PutAsync<SelfAssessmentReturnResponse>(
            endpoint,
            EmptyJsonContent,
            cancellationToken);

        if (response.SelfAssessmentReturn is null)
        {
            throw new FreeAgentApiException("Self assessment return data missing from API response");
        }

        return response.SelfAssessmentReturn;
    }

    private static string BuildCollectionEndpoint(long userId) =>
        $"users/{userId}/self_assessment_returns";

    private static string BuildPeriodEndpoint(long userId, DateOnly periodEndsOn) =>
        $"{BuildCollectionEndpoint(userId)}/{periodEndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
}
