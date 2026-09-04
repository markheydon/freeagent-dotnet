using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Infrastructure.Serialization;
using FreeAgent.Client.Models.Users;

namespace FreeAgent.Client.Services.Users;

/// <summary>
/// Service for interacting with FreeAgent users.
/// </summary>
public sealed class UserService
{
    private readonly IFreeAgentRequestClient _requestClient;

    /// <summary>
    /// Initializes a new instance of the user service.
    /// </summary>
    /// <param name="requestClient">Internal FreeAgent request client dependency.</param>
    internal UserService(IFreeAgentRequestClient requestClient)
    {
        _requestClient = requestClient ?? throw new ArgumentNullException(nameof(requestClient));
    }

    /// <summary>
    /// Lists all users for the current company.
    /// </summary>
    /// <param name="view">Users view filter (for example: <see cref="UserViews.ActiveStaff"/>)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All users returned by FreeAgent for the selected view</returns>
    public async Task<IReadOnlyList<User>> GetUsersAsync(
        string view = UserViews.All,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(view);

        var endpoint = FreeAgentQueryStringBuilder.BuildEndpoint("users", [new KeyValuePair<string, string>("view", view)]);
        var response = await _requestClient.GetAsync<UsersResponse>(endpoint, cancellationToken);

        if (response.Users is null)
        {
            throw new FreeAgentApiException("Users data missing from API response");
        }

        return response.Users;
    }

    /// <summary>
    /// Gets a single user by identifier.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    public async Task<User> GetUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

        var response = await _requestClient.GetAsync<UserResponse>($"users/{userId}", cancellationToken);

        if (response.User is null)
        {
            throw new FreeAgentApiException("User data missing from API response");
        }

        return response.User;
    }

    /// <summary>
    /// Gets the authenticated user's personal profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user profile</returns>
    /// <remarks>
    /// Minimum FreeAgent access level: Time.
    /// </remarks>
    public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var response = await _requestClient.GetAsync<UserResponse>("users/me", cancellationToken);

        if (response.User is null)
        {
            throw new FreeAgentApiException("User data missing from API response");
        }

        return response.User;
    }

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="user">User attributes to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user</returns>
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var content = FreeAgentJsonSerializer.CreateContent(new UserRequest { User = UserWritePayload.FromUser(user) });
        var response = await _requestClient.PostAsync<UserResponse>("users", content, cancellationToken);

        if (response.User is null)
        {
            throw new FreeAgentApiException("User data missing from API response");
        }

        return response.User;
    }

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL</param>
    /// <param name="user">User attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    public async Task<User> UpdateUserAsync(long userId, User user, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentNullException.ThrowIfNull(user);

        var content = FreeAgentJsonSerializer.CreateContent(new UserRequest { User = UserWritePayload.FromUser(user) });
        var response = await _requestClient.PutAsync<UserResponse>($"users/{userId}", content, cancellationToken);

        if (response.User is null)
        {
            throw new FreeAgentApiException("User data missing from API response");
        }

        return response.User;
    }

    /// <summary>
    /// Updates the authenticated user's personal profile.
    /// </summary>
    /// <param name="user">User attributes to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user profile</returns>
    /// <remarks>
    /// Minimum FreeAgent access level: Time.
    /// </remarks>
    public async Task<User> UpdateCurrentUserAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var content = FreeAgentJsonSerializer.CreateContent(new UserRequest { User = UserWritePayload.FromUser(user) });
        var response = await _requestClient.PutAsync<UserResponse>("users/me", content, cancellationToken);

        if (response.User is null)
        {
            throw new FreeAgentApiException("User data missing from API response");
        }

        return response.User;
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="userId">User identifier from the resource URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

        return _requestClient.DeleteAsync($"users/{userId}", cancellationToken);
    }
}
