using FreeAgent.Client.Models.Contacts;
using FreeAgent.Client.Models.Projects;
using FreeAgent.Client.Models.Tasks;
using FreeAgent.Client.Models.Timeslips;
using FreeAgent.Client.Models.Users;
using TaskModel = FreeAgent.Client.Models.Tasks.Task;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Resolves sandbox entities for examples without prompting the user.
/// </summary>
internal sealed class SampleData
{
    private readonly SampleContext _context;

    /// <summary>
    /// Initialises sample data helpers.
    /// </summary>
    /// <param name="context">Shared sample context.</param>
    public SampleData(SampleContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Returns the first contact from the active contacts list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A contact.</returns>
    public async Task<Contact> GetFirstContactAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Contacts.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no contacts found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first project from the projects list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A project.</returns>
    public async Task<Project> GetFirstProjectAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Projects.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no projects found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns a project picked from the first page of results.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A project.</returns>
    public async Task<Project> GetRandomProjectAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Projects.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no projects found in sandbox account");
        }

        var index = Random.Shared.Next(page.Items.Count);
        return page.Items[index];
    }

    /// <summary>
    /// Returns the first task from the tasks list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task.</returns>
    public async Task<TaskModel> GetFirstTaskAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Tasks.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no tasks found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first timeslip from the timeslips list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A timeslip.</returns>
    public async Task<Timeslip> GetFirstTimeslipAsync(CancellationToken cancellationToken = default)
    {
        var page = await _context.Client.Timeslips.ListAsync(perPage: 25, cancellationToken: cancellationToken);
        if (page.Items.Count == 0)
        {
            SampleContext.Skip("no timeslips found in sandbox account");
        }

        return page.Items[0];
    }

    /// <summary>
    /// Returns the first user from the users list.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A user.</returns>
    public async Task<User> GetFirstUserAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Client.Users.ListAsync(cancellationToken: cancellationToken);
        if (users.Count == 0)
        {
            SampleContext.Skip("no users found in sandbox account");
        }

        return users[0];
    }
}
