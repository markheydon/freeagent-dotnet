namespace FreeAgent.Client;

/// <summary>
/// Options for <see cref="Services.BankFeeds.BankFeedsService.GetBankFeedAsync(long, BankFeedGetOptions?, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class BankFeedGetOptions
{
    /// <summary>
    /// When <see langword="true"/>, fetches the bank account when the bank feed response contains only a bank account URI.
    /// </summary>
    public bool IncludeBankAccount { get; init; }
}
