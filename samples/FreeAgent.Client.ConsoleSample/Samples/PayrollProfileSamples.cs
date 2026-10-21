using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Payroll profiles endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Payroll profiles")]
internal sealed class PayrollProfileSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List payroll profiles for current tax year end")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListProfilesAsync(CancellationToken cancellationToken)
    {
        var taxYearEnd = ResolveCurrentUkTaxYearEnd();
        var profiles = await context.Client.PayrollProfiles.ListAsync(taxYearEnd, cancellationToken);

        SampleOutput.WriteHeader($"Payroll profiles ({taxYearEnd}) — {profiles.Count} profile(s)");
        SampleOutput.WriteRows(
            profiles,
            profile => $"user={profile.UserId}  ref={profile.PayrollReference}  title={profile.Title}");
    }

    [ConsoleSample(Name = "List payroll profile for first user")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListProfileForFirstUserAsync(CancellationToken cancellationToken)
    {
        var users = await context.Client.Users.ListAsync(cancellationToken: cancellationToken);
        if (users.Count == 0)
        {
            SampleContext.Skip("no users in the connected company");
        }

        var user = users[0];
        var taxYearEnd = ResolveCurrentUkTaxYearEnd();
        var userReference = UserReference.ForEnvironment(context.Client.Environment, user.ResourceId);
        var profiles = await context.Client.PayrollProfiles.ListProfilesForUserAsync(
            taxYearEnd,
            userReference,
            cancellationToken);

        SampleOutput.WriteHeader($"Payroll profile for user {user.ResourceId} ({taxYearEnd})");
        SampleOutput.WriteField("Profiles returned", profiles.Count);
        if (profiles.Count > 0)
        {
            SampleOutput.WriteField("Payroll reference", profiles[0].PayrollReference);
        }
    }

    private static int ResolveCurrentUkTaxYearEnd()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return today.Month >= 4 ? today.Year + 1 : today.Year;
    }
}
