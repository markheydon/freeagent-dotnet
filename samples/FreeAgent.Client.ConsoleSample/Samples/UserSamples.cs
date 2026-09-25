using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FreeAgent.Client.Models.Users;

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

    [ConsoleSample(Name = "Create probe user and delete", ExcludeFromRunAll = true)]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProbeUserAndDeleteAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleUserAsync(cancellationToken);

        SampleOutput.WriteHeader("Created probe user");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Email", created.Email);

        await context.Client.Users.DeleteUserAsync(created.ResourceId, cancellationToken);

        SampleOutput.WriteHeader("Deleted probe user");
        SampleOutput.WriteField("Id", created.ResourceId);
    }

    [ConsoleSample(Name = "Update user last name")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateUserLastNameAsync(CancellationToken cancellationToken)
    {
        var created = await CreateSampleUserAsync(cancellationToken);
        created.LastName = $"{created.LastName} (updated)";

        var updated = await context.Client.Users.UpdateUserAsync(created.ResourceId, created, cancellationToken);

        SampleOutput.WriteHeader("Updated user last name");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Display name", updated.DisplayName);

        await context.Client.Users.DeleteUserAsync(updated.ResourceId, cancellationToken);
    }

    [ConsoleSample(Name = "Update current user opening mileage")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateCurrentUserOpeningMileageAsync(CancellationToken cancellationToken)
    {
        var current = await context.Client.Users.GetCurrentUserAsync(cancellationToken);
        var previousMileage = current.OpeningMileage ?? 0;
        current.OpeningMileage = previousMileage + 1;

        var updated = await context.Client.Users.UpdateCurrentUserAsync(current, cancellationToken);

        SampleOutput.WriteHeader("Updated current user opening mileage");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Opening mileage", updated.OpeningMileage);

        current.OpeningMileage = previousMileage;
        await context.Client.Users.UpdateCurrentUserAsync(current, cancellationToken);
    }

    private Task<User> CreateSampleUserAsync(CancellationToken cancellationToken)
    {
        var uniqueSuffix = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        return context.Client.Users.CreateUserAsync(
            new User
            {
                Email = $"console-probe-{uniqueSuffix}@example.com",
                FirstName = "Console",
                LastName = "Probe User",
                Role = UserRole.Employee,
                OpeningMileage = 0,
                SendInvitation = false,
                PermissionLevel = UserPermissionLevel.Time
            },
            cancellationToken);
    }
}
