using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.BankAccounts;

namespace FreeAgent.Client.Services.BankAccounts;

/// <summary>
/// Service for interacting with FreeAgent bank accounts.
/// </summary>
/// <remarks>
/// Minimum FreeAgent access level: Banking.
/// </remarks>
public sealed class BankAccountsService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the bank accounts service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal BankAccountsService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all bank accounts for the current company.
    /// </summary>
    /// <param name="view">Optional view filter (for example <see cref="BankAccountViews.StandardBankAccounts"/>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All bank accounts returned by FreeAgent</returns>
    public async Task<IReadOnlyList<BankAccount>> ListAsync(
        string? view = null,
        CancellationToken cancellationToken = default)
    {
        var endpoint = string.IsNullOrWhiteSpace(view)
            ? "bank_accounts"
            : FreeAgentQueryStringBuilder.BuildEndpoint(
                "bank_accounts",
                [new KeyValuePair<string, string>("view", view)]);

        var response = await _requestClient.GetAsync<BankAccountListResponse>(endpoint, cancellationToken);

        if (response.BankAccounts is null)
        {
            throw new FreeAgentApiException("Bank accounts data missing from API response");
        }

        return response.BankAccounts;
    }

    /// <summary>
    /// Gets a single bank account by identifier.
    /// </summary>
    /// <param name="bankAccountId">Bank account identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bank account details</returns>
    public async Task<BankAccount> GetBankAccountAsync(long bankAccountId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankAccountId);

        var response = await _requestClient.GetAsync<BankAccountResponse>($"bank_accounts/{bankAccountId}", cancellationToken);

        if (response.BankAccount is null)
        {
            throw new FreeAgentApiException("Bank account data missing from API response");
        }

        return response.BankAccount;
    }

    /// <summary>
    /// Creates a standard bank account.
    /// </summary>
    /// <param name="request">Standard bank account attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created bank account</returns>
    public Task<BankAccount> CreateStandardBankAccountAsync(
        CreateStandardBankAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateBankAccountAsync(BankAccountWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Creates a credit card account.
    /// </summary>
    /// <param name="request">Credit card account attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created bank account</returns>
    public Task<BankAccount> CreateCreditCardBankAccountAsync(
        CreateCreditCardBankAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateBankAccountAsync(BankAccountWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Creates an e-commerce bank account.
    /// </summary>
    /// <param name="request">E-commerce bank account attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created bank account</returns>
    public Task<BankAccount> CreateEcommerceBankAccountAsync(
        CreateEcommerceBankAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return CreateBankAccountAsync(BankAccountWritePayloadMapper.FromCreate(request), cancellationToken);
    }

    /// <summary>
    /// Updates a bank account.
    /// </summary>
    /// <param name="bankAccountId">Bank account identifier from the resource URL</param>
    /// <param name="request">Bank account attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated bank account</returns>
    public async Task<BankAccount> UpdateBankAccountAsync(
        long bankAccountId,
        UpdateBankAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankAccountId);
        ArgumentNullException.ThrowIfNull(request);

        var content = FreeAgentJsonSerializer.CreateContent(new BankAccountRequest
        {
            BankAccount = BankAccountWritePayloadMapper.FromUpdate(request)
        });

        var response = await _requestClient.PutAsync<BankAccountResponse>(
            $"bank_accounts/{bankAccountId}",
            content,
            cancellationToken);

        if (response.BankAccount is null)
        {
            throw new FreeAgentApiException("Bank account data missing from API response");
        }

        return response.BankAccount;
    }

    /// <summary>
    /// Deletes a bank account.
    /// </summary>
    /// <param name="bankAccountId">Bank account identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteBankAccountAsync(long bankAccountId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bankAccountId);

        return _requestClient.DeleteAsync($"bank_accounts/{bankAccountId}", cancellationToken);
    }

    private async Task<BankAccount> CreateBankAccountAsync(
        BankAccountWritePayload payload,
        CancellationToken cancellationToken)
    {
        var content = FreeAgentJsonSerializer.CreateContent(new BankAccountRequest
        {
            BankAccount = payload
        });

        var response = await _requestClient.PostAsync<BankAccountResponse>("bank_accounts", content, cancellationToken);

        if (response.BankAccount is null)
        {
            throw new FreeAgentApiException("Bank account data missing from API response");
        }

        return response.BankAccount;
    }
}
