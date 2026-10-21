using System.Diagnostics.CodeAnalysis;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Capital asset types endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Capital asset types")]
internal sealed class CapitalAssetTypeSamples(SampleContext context) : IConsoleSampleProvider
{
    private const string ProbeName = "SDK-CONSOLE-CAPITAL-ASSET-TYPE";

    [ConsoleSample(Name = "List capital asset types")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListCapitalAssetTypesAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.CapitalAssetTypes.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Capital asset types ({items.Count})");
        SampleOutput.WriteRows(items, item => $"{item.ResourceId,8}  {item.Name}  system={item.SystemDefault}");
    }

    [ConsoleSample(Name = "Get capital asset type detail with probe create and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCapitalAssetTypeDetailAsync(CancellationToken cancellationToken)
    {
        var (probeId, createdForProbe) = await ResolveProbeCapitalAssetTypeIdAsync(cancellationToken);
        try
        {
            var detail = await context.Client.CapitalAssetTypes.GetCapitalAssetTypeAsync(probeId, cancellationToken);

            SampleOutput.WriteHeader("Capital asset type detail");
            SampleOutput.WriteField("Id", detail.ResourceId);
            SampleOutput.WriteField("Name", detail.Name);
            SampleOutput.WriteField("System default", detail.SystemDefault);
        }
        finally
        {
            if (createdForProbe)
            {
                await context.Client.CapitalAssetTypes.DeleteCapitalAssetTypeAsync(probeId, cancellationToken);
            }
        }
    }

    [ConsoleSample(Name = "Update capital asset type name", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCapitalAssetTypeNameAsync(CancellationToken cancellationToken)
    {
        var (probeId, createdForProbe) = await ResolveProbeCapitalAssetTypeIdAsync(cancellationToken);
        try
        {
            var existing = await context.Client.CapitalAssetTypes.GetCapitalAssetTypeAsync(probeId, cancellationToken);
            var updated = await context.Client.CapitalAssetTypes.UpdateCapitalAssetTypeAsync(
                probeId,
                new UpdateCapitalAssetTypeRequest
                {
                    Name = $"{existing.Name} (console sample touch)"
                },
                cancellationToken);

            SampleOutput.WriteHeader("Updated capital asset type");
            SampleOutput.WriteField("Id", updated.ResourceId);
            SampleOutput.WriteField("Name", updated.Name);
        }
        finally
        {
            if (createdForProbe)
            {
                await context.Client.CapitalAssetTypes.DeleteCapitalAssetTypeAsync(probeId, cancellationToken);
            }
        }
    }

    [ConsoleSample(Name = "Create probe capital asset type and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeCapitalAssetTypeAsync(CancellationToken cancellationToken)
    {
        var created = await context.Client.CapitalAssetTypes.CreateCapitalAssetTypeAsync(
            new CreateCapitalAssetTypeRequest { Name = ProbeName },
            cancellationToken);

        SampleOutput.WriteHeader("Created probe capital asset type");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Name", created.Name);

        await context.Client.CapitalAssetTypes.DeleteCapitalAssetTypeAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteHeader("Deleted probe capital asset type");
    }

    private async Task<(long ProbeId, bool CreatedForProbe)> ResolveProbeCapitalAssetTypeIdAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.CapitalAssetTypes.ListAsync(cancellationToken: cancellationToken);
        var existing = items.FirstOrDefault(item => string.Equals(item.Name, ProbeName, StringComparison.Ordinal));
        if (existing is not null)
        {
            return (existing.ResourceId, false);
        }

        var created = await context.Client.CapitalAssetTypes.CreateCapitalAssetTypeAsync(
            new CreateCapitalAssetTypeRequest { Name = ProbeName },
            cancellationToken);

        return (created.ResourceId, true);
    }
}
