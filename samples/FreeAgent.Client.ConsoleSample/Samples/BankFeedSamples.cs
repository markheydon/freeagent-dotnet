using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.BankFeeds;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Bank feeds endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Bank feeds")]
internal sealed class BankFeedSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List bank feeds")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListBankFeedsAsync(CancellationToken cancellationToken)
    {
        var bankFeeds = await context.Client.BankFeeds.ListAsync(cancellationToken);

        SampleOutput.WriteHeader($"Bank feeds ({bankFeeds.Count})");
        SampleOutput.WriteRows(
            bankFeeds,
            feed => $"{feed.ResourceId,8}  account {feed.BankAccountId}  {feed.FeedType}  {feed.BankServiceName}  ({feed.State})");
    }

    [ConsoleSample(Name = "Get bank feed detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetBankFeedDetailAsync(CancellationToken cancellationToken)
    {
        var bankFeeds = await context.Client.BankFeeds.ListAsync(cancellationToken);
        if (bankFeeds.Count == 0)
        {
            SampleOutput.WriteHeader("Bank feed detail");
            SampleOutput.WriteField("Result", "No bank feeds returned.");
            return;
        }

        var bankFeedId = bankFeeds[0].ResourceId;
        var detail = await context.Client.BankFeeds.GetBankFeedAsync(bankFeedId, cancellationToken);

        SampleOutput.WriteHeader("Bank feed detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Bank account id", detail.BankAccountId);
        SampleOutput.WriteField("Feed type", detail.FeedType?.ToString());
        SampleOutput.WriteField("Bank service", detail.BankServiceName);
        SampleOutput.WriteField("State", detail.State);
        SampleOutput.WriteField("SCA expires at", detail.ScaExpiresAt);
    }
}
