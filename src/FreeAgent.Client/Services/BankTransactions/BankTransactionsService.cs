using System.Globalization;
using System.Net.Http.Headers;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankTransactions;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Services.BankTransactions;

/// <summary>
/// Service for interacting with FreeAgent bank transactions.
/// </summary>
/// <remarks>
/// Minimum FreeAgent access level: Banking.
/// List and statement upload require a bank account. Nested explanations are returned on GET.
/// Explanation create, update, and delete are a separate resource.
/// </remarks>
public sealed class BankTransactionsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the bank transactions service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal BankTransactionsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists one page of bank transactions for a bank account.
    /// </summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="perPage">Page size. FreeAgent allows at most 100.</param>
    /// <param name="bankAccount">Bank account reference. Required when <paramref name="bankAccountId"/> is omitted.</param>
    /// <param name="bankAccountId">Bank account identifier. Required when <paramref name="bankAccount"/> is omitted.</param>
    /// <param name="view">Optional view filter.</param>
    /// <param name="fromDate">Optional inclusive start date.</param>
    /// <param name="toDate">Optional inclusive end date.</param>
    /// <param name="updatedSince">Optional timestamp filter.</param>
    /// <param name="lastUploaded">When set, filters to the latest statement upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>One page of bank transactions.</returns>
    public async Task<PaginatedResponse<BankTransaction>> ListAsync(
        int page = 1,
        int perPage = 25,
        BankAccountReference? bankAccount = null,
        long? bankAccountId = null,
        BankTransactionView? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        bool? lastUploaded = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(perPage, 100);

        var queryParameters = new List<KeyValuePair<string, string>>
        {
            new("bank_account", ResolveRequiredBankAccount(bankAccount, bankAccountId)),
            new("page", page.ToString(CultureInfo.InvariantCulture)),
            new("per_page", perPage.ToString(CultureInfo.InvariantCulture))
        };

        if (view is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>("view", EnumWireValue.Get(view.Value)));
        }

        if (fromDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "from_date",
                fromDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (toDate is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "to_date",
                toDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        if (updatedSince is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "updated_since",
                updatedSince.Value.ToString("o", CultureInfo.InvariantCulture)));
        }

        if (lastUploaded is not null)
        {
            queryParameters.Add(new KeyValuePair<string, string>(
                "last_uploaded",
                lastUploaded.Value ? "true" : "false"));
        }

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("bank_transactions", queryParameters);
        var response = await _requestClient.GetWithMetadataAsync<BankTransactionListResponse>(endpoint, cancellationToken);

        if (response.Data.BankTransactions is null)
        {
            throw new FreeAgentApiException("Bank transactions data missing from API response");
        }

        var total = FreeAgentPaginationHelper.GetTotalCountOrEstimate(
            response,
            page,
            perPage,
            response.Data.BankTransactions.Count);

        return new PaginatedResponse<BankTransaction>(
            page,
            perPage,
            total,
            response.Data.BankTransactions);
    }

    /// <summary>
    /// Lists all bank transactions for a bank account across all pages.
    /// </summary>
    /// <param name="perPage">Page size. FreeAgent allows at most 100.</param>
    /// <param name="bankAccount">Bank account reference. Required when <paramref name="bankAccountId"/> is omitted.</param>
    /// <param name="bankAccountId">Bank account identifier. Required when <paramref name="bankAccount"/> is omitted.</param>
    /// <param name="view">Optional view filter.</param>
    /// <param name="fromDate">Optional inclusive start date.</param>
    /// <param name="toDate">Optional inclusive end date.</param>
    /// <param name="updatedSince">Optional timestamp filter.</param>
    /// <param name="lastUploaded">When set, filters to the latest statement upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bank transactions across all pages.</returns>
    public async IAsyncEnumerable<BankTransaction> ListAutoPagingAsync(
        int perPage = 25,
        BankAccountReference? bankAccount = null,
        long? bankAccountId = null,
        BankTransactionView? view = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        DateTimeOffset? updatedSince = null,
        bool? lastUploaded = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;

        while (true)
        {
            var transactionsPage = await ListAsync(
                page,
                perPage,
                bankAccount,
                bankAccountId,
                view,
                fromDate,
                toDate,
                updatedSince,
                lastUploaded,
                cancellationToken);

            foreach (var transaction in transactionsPage.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return transaction;
            }

            if (!transactionsPage.HasNextPage)
            {
                yield break;
            }

            page = transactionsPage.NextPage!.Value;
        }
    }

    /// <summary>
    /// Gets a single bank transaction by identifier.
    /// </summary>
    /// <param name="bankTransactionId">Bank transaction identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bank transaction, including nested explanations when the API returns them.</returns>
    public async Task<BankTransaction> GetBankTransactionAsync(
        long bankTransactionId,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);

        var response = await _requestClient.GetAsync<BankTransactionResponse>(
            $"bank_transactions/{bankTransactionId}",
            cancellationToken);

        if (response.BankTransaction is null)
        {
            throw new FreeAgentApiException("Bank transaction data missing from API response");
        }

        return response.BankTransaction;
    }

    /// <summary>
    /// Deletes a bank transaction.
    /// </summary>
    /// <param name="bankTransactionId">Bank transaction identifier from the resource URL.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// FreeAgent deletes a transaction only when it is fully unexplained or has no explanations.
    /// The official docs page currently labels this operation as deleting an explanation and shows
    /// a singular <c>bank_transaction</c> path. The SDK calls <c>DELETE /v2/bank_transactions/:id</c>,
    /// which matches the rest of the resource.
    /// </remarks>
    public Task DeleteBankTransactionAsync(long bankTransactionId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankTransactionId);

        return _requestClient.DeleteAsync($"bank_transactions/{bankTransactionId}", cancellationToken);
    }

    /// <summary>
    /// Uploads a bank statement as an array of transactions.
    /// </summary>
    /// <param name="request">Statement lines.</param>
    /// <param name="bankAccount">Bank account reference. Required when <paramref name="bankAccountId"/> is omitted.</param>
    /// <param name="bankAccountId">Bank account identifier. Required when <paramref name="bankAccount"/> is omitted.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// A successful response means the upload was accepted. It does not confirm that every line was imported.
    /// List the account afterwards, or use <c>last_uploaded=true</c>, to see imported transactions.
    /// </remarks>
    public Task UploadBankStatementAsync(
        UploadBankStatementRequest request,
        BankAccountReference? bankAccount = null,
        long? bankAccountId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var endpoint = BuildStatementEndpoint(bankAccount, bankAccountId);
        var content = FreeAgentJsonSerializer.CreateContent(new BankStatementUploadPayload
        {
            Statement = request.Statement
        });

        return _requestClient.SendPostAsync(endpoint, content, cancellationToken);
    }

    /// <summary>
    /// Uploads a bank statement file (OFX, QIF, or a supported CSV).
    /// </summary>
    /// <param name="request">Statement file.</param>
    /// <param name="bankAccount">Bank account reference. Required when <paramref name="bankAccountId"/> is omitted.</param>
    /// <param name="bankAccountId">Bank account identifier. Required when <paramref name="bankAccount"/> is omitted.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// A successful response means the upload was accepted. It does not confirm that the file was imported.
    /// </remarks>
    public async Task UploadBankStatementFileAsync(
        UploadBankStatementFileRequest request,
        BankAccountReference? bankAccount = null,
        long? bankAccountId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var buffer = new MemoryStream();
        await request.Statement.CopyToAsync(buffer, cancellationToken);

        if (buffer.Length == 0)
        {
            throw new ArgumentException("Statement file must contain data.", nameof(request));
        }

        var endpoint = BuildStatementEndpoint(bankAccount, bankAccountId);
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(buffer.ToArray());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "statement", request.FileName);

        await _requestClient.SendPostAsync(endpoint, content, cancellationToken);
    }

    private string BuildStatementEndpoint(BankAccountReference? bankAccount, long? bankAccountId)
    {
        return FreeAgentQueryStringBuilder.BuildEndpoint(
            "bank_transactions/statement",
            [new KeyValuePair<string, string>("bank_account", ResolveRequiredBankAccount(bankAccount, bankAccountId))]);
    }

    private string ResolveRequiredBankAccount(BankAccountReference? bankAccount, long? bankAccountId)
    {
        if (bankAccount is not null && bankAccountId is not null)
        {
            throw new ArgumentException("Specify either bankAccount or bankAccountId, not both.", nameof(bankAccount));
        }

        if (bankAccount is not null)
        {
            return bankAccount.Value.Uri;
        }

        if (bankAccountId is null)
        {
            throw new ArgumentException(
                "A bank account is required. Specify bankAccount or bankAccountId.",
                nameof(bankAccountId));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankAccountId.Value);
        return BankAccountReference.ForEnvironment(_requestClient.Environment, bankAccountId.Value).Uri;
    }
}
