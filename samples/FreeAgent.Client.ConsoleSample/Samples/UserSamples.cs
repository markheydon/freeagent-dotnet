using System.Diagnostics.CodeAnalysis;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Users endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Users")]
internal sealed class UserSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List users")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListUsersAsync(CancellationToken cancellationToken)
    {
        var users = await context.Client.Users.ListAsync(cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Users ({users.Count})");
        SampleOutput.WriteRows(users, user => $"{user.ResourceId,8}  {user.DisplayName}  {user.Email}");
    }

    [ConsoleSample(Name = "Get current user")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var user = await context.Client.Users.GetCurrentUserAsync(cancellationToken);

        SampleOutput.WriteHeader("Current user");
        SampleOutput.WriteField("Id", user.ResourceId);
        SampleOutput.WriteField("Display name", user.DisplayName);
        SampleOutput.WriteField("Email", user.Email);
        SampleOutput.WriteField("Role", user.Role);
    }

    [ConsoleSample(Name = "Get user by ID")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetUserByIdAsync(CancellationToken cancellationToken)
    {
        var user = await context.Data.GetFirstUserAsync(cancellationToken);
        var detail = await context.Client.Users.GetUserAsync(user.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("User detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Display name", detail.DisplayName);
        SampleOutput.WriteField("Email", detail.Email);
        SampleOutput.WriteField("Role", detail.Role);
    }
}
