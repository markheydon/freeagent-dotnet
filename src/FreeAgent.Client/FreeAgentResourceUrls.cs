using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client;

/// <summary>
/// Builds environment-correct FreeAgent resource URIs.
/// </summary>
public sealed class FreeAgentResourceUrls
{
    private readonly FreeAgentEnvironment _environment;

    internal FreeAgentResourceUrls(FreeAgentEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Creates a contact resource reference.
    /// </summary>
    /// <param name="contactId">Contact identifier.</param>
    /// <returns>Contact reference.</returns>
    public ContactReference Contact(long contactId) => ContactReference.ForEnvironment(_environment, contactId);

    /// <summary>
    /// Creates a project resource reference.
    /// </summary>
    /// <param name="projectId">Project identifier.</param>
    /// <returns>Project reference.</returns>
    public ProjectReference Project(long projectId) => ProjectReference.ForEnvironment(_environment, projectId);

    /// <summary>
    /// Creates a task resource reference.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <returns>Task reference.</returns>
    public TaskReference Task(long taskId) => TaskReference.ForEnvironment(_environment, taskId);
}
