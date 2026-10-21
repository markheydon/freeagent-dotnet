using System.Diagnostics.CodeAnalysis;
namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// CIS settings endpoint examples.
/// </summary>
[ConsoleSamples(Category = "CIS settings")]
internal sealed class CisSettingsSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "Get CIS settings")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCisSettingsAsync(CancellationToken cancellationToken)
    {
        var settings = await context.Client.CisSettings.GetCisSettingsAsync(cancellationToken);

        SampleOutput.WriteHeader("CIS settings");
        SampleOutput.WriteField("Contractor registered", settings.ContractorDetails is not null);
        SampleOutput.WriteField("Subcontractor registered", settings.SubcontractorDetails is not null);
        if (settings.SubcontractorDetails?.CisDeductionRates is { Count: > 0 } rates)
        {
            SampleOutput.WriteField("Deduction rates", string.Join(", ", rates));
        }
    }

    [ConsoleSample(Name = "Round-trip CIS prior deductions update", ExcludeFromRunAll = true, MutatesData = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task RoundTripPriorDeductionsUpdateAsync(CancellationToken cancellationToken)
    {
        var current = await context.Client.CisSettings.GetCisSettingsAsync(cancellationToken);
        var prior = current.SubcontractorDetails?.PriorDeductions;
        if (prior?.StartDate is null || prior.InitialBalance is null)
        {
            SampleContext.Skip("subcontractor prior deductions are not recorded on this company");
        }

        var update = new CisSettingsUpdate
        {
            SubcontractorDetails = PatchValue.Set(new CisSubcontractorDetailsUpdate
            {
                PriorDeductions = PatchValue.Set(new CisPriorDeductions
                {
                    StartDate = prior.StartDate,
                    InitialBalance = prior.InitialBalance
                })
            })
        };

        var updated = await context.Client.CisSettings.UpdateCisSettingsAsync(update, cancellationToken);
        SampleOutput.WriteHeader("CIS settings updated (prior deductions round-trip)");
        SampleOutput.WriteField("Start date", updated.SubcontractorDetails?.PriorDeductions?.StartDate);
        SampleOutput.WriteField("Initial balance", updated.SubcontractorDetails?.PriorDeductions?.InitialBalance);
    }
}
