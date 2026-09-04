using FreeAgent.Client;
using FreeAgent.Client.Models.Contacts;

namespace FreeAgent.Client.BlazorSample.Services;

/// <summary>
/// Creates sample contacts used to exercise SDK field coverage in the probe UI.
/// </summary>
public sealed class SampleContactSeeder
{
    public async Task<SampleSeedResult> CreateFullDetailProbeAsync(
        FreeAgentClient client,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var (contact, action) = await ContactSeederSupport.UpsertByEmailAsync(
            client,
            SampleContactFixtures.FullDetailProbeEmail,
            SampleContactFixtures.CreateFullDetailProbeContact(),
            cancellationToken);

        return new SampleSeedResult(contact, "Full-detail probe contact", action);
    }
}

public sealed record SampleSeedResult(Contact Contact, string DisplayName, ContactSeedAction Action);
