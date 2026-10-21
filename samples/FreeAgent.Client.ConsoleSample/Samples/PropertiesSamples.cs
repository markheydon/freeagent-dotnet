using System.Diagnostics.CodeAnalysis;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Landlord properties endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Properties")]
internal sealed class PropertiesSamples(SampleContext context) : IConsoleSampleProvider
{
    private const string ProbeAddress1 = "SDK-CONSOLE-PROBE-PROPERTY";

    [ConsoleSample(Name = "List properties")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListPropertiesAsync(CancellationToken cancellationToken)
    {
        await EnsureLandlordCompanyAsync(cancellationToken);

        var items = await context.Client.Properties.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Properties ({items.Count})");
        SampleOutput.WriteRows(
            items,
            item => $"{item.ResourceId,8}  {item.Address1}  {item.Town}  {item.Postcode}");
    }

    [ConsoleSample(Name = "Get property detail with probe create and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetPropertyDetailAsync(CancellationToken cancellationToken)
    {
        await EnsureLandlordCompanyAsync(cancellationToken);

        var (probeId, createdForProbe) = await ResolveProbePropertyIdAsync(cancellationToken);
        try
        {
            var detail = await context.Client.Properties.GetPropertyAsync(probeId, cancellationToken);

            SampleOutput.WriteHeader("Property detail");
            SampleOutput.WriteField("Id", detail.ResourceId);
            SampleOutput.WriteField("Address1", detail.Address1);
            SampleOutput.WriteField("Town", detail.Town);
            SampleOutput.WriteField("Postcode", detail.Postcode);
        }
        finally
        {
            if (createdForProbe)
            {
                await context.Client.Properties.DeletePropertyAsync(probeId, cancellationToken);
            }
        }
    }

    [ConsoleSample(Name = "Update property address", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdatePropertyAddressAsync(CancellationToken cancellationToken)
    {
        await EnsureLandlordCompanyAsync(cancellationToken);

        var (probeId, createdForProbe) = await ResolveProbePropertyIdAsync(cancellationToken);
        try
        {
            var existing = await context.Client.Properties.GetPropertyAsync(probeId, cancellationToken);
            var updated = await context.Client.Properties.UpdatePropertyAsync(
                probeId,
                new UpdatePropertyRequest
                {
                    Address2 = $"{existing.Address2} (console sample touch)".Trim()
                },
                cancellationToken);

            SampleOutput.WriteHeader("Updated property");
            SampleOutput.WriteField("Id", updated.ResourceId);
            SampleOutput.WriteField("Address2", updated.Address2);
        }
        finally
        {
            if (createdForProbe)
            {
                await context.Client.Properties.DeletePropertyAsync(probeId, cancellationToken);
            }
        }
    }

    [ConsoleSample(Name = "Create probe property and delete", MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbePropertyAsync(CancellationToken cancellationToken)
    {
        await EnsureLandlordCompanyAsync(cancellationToken);

        var created = await context.Client.Properties.CreatePropertyAsync(
            new CreatePropertyRequest
            {
                Address1 = ProbeAddress1,
                Town = "Consoleville",
                Postcode = "AB1 2CD"
            },
            cancellationToken);

        SampleOutput.WriteHeader("Created probe property");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Address1", created.Address1);

        await context.Client.Properties.DeletePropertyAsync(created.ResourceId, cancellationToken);
        SampleOutput.WriteHeader("Deleted probe property");
    }

    private async Task EnsureLandlordCompanyAsync(CancellationToken cancellationToken)
    {
        var company = await context.Client.Company.GetCompanyAsync(cancellationToken);
        if (company.Type != CompanyType.UkUnincorporatedLandlord)
        {
            SampleContext.Skip("company type is not UkUnincorporatedLandlord");
        }
    }

    private async Task<(long ProbeId, bool CreatedForProbe)> ResolveProbePropertyIdAsync(CancellationToken cancellationToken)
    {
        var items = await context.Client.Properties.ListAsync(cancellationToken: cancellationToken);
        var existing = items.FirstOrDefault(item => string.Equals(item.Address1, ProbeAddress1, StringComparison.Ordinal));
        if (existing is not null)
        {
            return (existing.ResourceId, false);
        }

        var created = await context.Client.Properties.CreatePropertyAsync(
            new CreatePropertyRequest
            {
                Address1 = ProbeAddress1,
                Town = "Consoleville",
                Postcode = "AB1 2CD"
            },
            cancellationToken);

        return (created.ResourceId, true);
    }
}
