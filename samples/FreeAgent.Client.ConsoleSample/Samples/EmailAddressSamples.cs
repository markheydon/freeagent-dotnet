using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Email addresses endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Email addresses")]
internal sealed class EmailAddressSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List email addresses")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListEmailAddressesAsync(CancellationToken cancellationToken)
    {
        var addresses = await context.Client.EmailAddresses.ListAsync(cancellationToken);

        SampleOutput.WriteHeader($"Email addresses ({addresses.Count})");
        SampleOutput.WriteRows(addresses, address => address);
    }
}
