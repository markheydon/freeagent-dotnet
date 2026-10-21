using System.Globalization;
using System.Net.Http;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Payroll;

namespace FreeAgent.Client.Services.Payroll;

/// <summary>
/// Service for FreeAgent payroll RTI data and liability payments.
/// </summary>
public sealed class PayrollService
{
    private static readonly StringContent EmptyJsonContent =
        new("{}", System.Text.Encoding.UTF8, "application/json");

    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the payroll service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal PayrollService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all payroll periods and liability payments for the given tax year end.
    /// </summary>
    /// <param name="taxYearEnd">Tax year end (for example 2026 for April 2025–March 2026).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Periods and payments for the tax year</returns>
    public async Task<PayrollTaxYear> ListAllPeriodsForTaxYearAsync(
        int taxYearEnd,
        CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<PayrollTaxYear>(
            BuildTaxYearEndpoint(taxYearEnd),
            cancellationToken);

        if (response.Periods is null)
        {
            throw new FreeAgentApiException("Payroll periods missing from API response");
        }

        return response;
    }

    /// <summary>
    /// Lists all payslips for the given tax year end and period number.
    /// </summary>
    /// <param name="taxYearEnd">Tax year end.</param>
    /// <param name="periodNumber">Pay period index within the tax year (0–11).</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payroll period including payslips</returns>
    public async Task<PayrollPeriod> ListAllPayslipsForPeriodAsync(
        int taxYearEnd,
        int periodNumber,
        CancellationToken cancellationToken = default)
    {
        ValidatePeriodNumber(periodNumber);

        var response = await _requestClient.GetAsync<PayrollPeriodResponse>(
            BuildPeriodEndpoint(taxYearEnd, periodNumber),
            cancellationToken);

        if (response.Period is null)
        {
            throw new FreeAgentApiException("Payroll period data missing from API response");
        }

        return response.Period;
    }

    /// <summary>
    /// Marks a payroll liability payment as paid.
    /// </summary>
    /// <param name="taxYearEnd">Tax year end.</param>
    /// <param name="paymentDate">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated periods and payments for the tax year</returns>
    public Task<PayrollTaxYear> MarkPaymentAsPaidAsync(
        int taxYearEnd,
        DateOnly paymentDate,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(taxYearEnd, paymentDate, "mark_as_paid", usePut: true, cancellationToken);

    /// <summary>
    /// Marks a payroll liability payment as unpaid.
    /// </summary>
    /// <param name="taxYearEnd">Tax year end.</param>
    /// <param name="paymentDate">Payment due date used in the API path.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated periods and payments for the tax year</returns>
    public Task<PayrollTaxYear> MarkPaymentAsUnpaidAsync(
        int taxYearEnd,
        DateOnly paymentDate,
        CancellationToken cancellationToken = default) =>
        TransitionPaymentAsync(taxYearEnd, paymentDate, "mark_as_unpaid", usePut: false, cancellationToken);

    private async Task<PayrollTaxYear> TransitionPaymentAsync(
        int taxYearEnd,
        DateOnly paymentDate,
        string action,
        bool usePut,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var paymentSegment = paymentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var endpoint = $"{BuildTaxYearEndpoint(taxYearEnd)}/payments/{paymentSegment}/{action}";

        PayrollTaxYear response;
        if (usePut)
        {
            response = await _requestClient.PutAsync<PayrollTaxYear>(
                endpoint,
                EmptyJsonContent,
                cancellationToken);
        }
        else
        {
            response = await _requestClient.GetAsync<PayrollTaxYear>(endpoint, cancellationToken);
        }

        if (response.Periods is null)
        {
            throw new FreeAgentApiException("Payroll periods missing from API response");
        }

        return response;
    }

    private static void ValidatePeriodNumber(int periodNumber)
    {
        if (periodNumber is < 0 or > 11)
        {
            throw new ArgumentOutOfRangeException(
                nameof(periodNumber),
                periodNumber,
                "Payroll period must be between 0 and 11.");
        }
    }

    private static string BuildTaxYearEndpoint(int taxYearEnd) => $"payroll/{taxYearEnd}";

    private static string BuildPeriodEndpoint(int taxYearEnd, int periodNumber) =>
        $"{BuildTaxYearEndpoint(taxYearEnd)}/{periodNumber}";
}
