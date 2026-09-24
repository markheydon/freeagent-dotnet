using System.Diagnostics.CodeAnalysis;
using FreeAgent.Client.Models.Notes;

namespace FreeAgent.Client.ConsoleSample.Samples;

/// <summary>
/// Notes endpoint examples.
/// </summary>
[ConsoleSamples(Category = "Notes")]
internal sealed class NoteSamples(SampleContext context) : IConsoleSampleProvider
{
    [ConsoleSample(Name = "List contact notes")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListContactNotesAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var notes = await context.Client.Notes.ListContactNotesAsync(
            contactId: contact.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Notes for contact {contact.ResourceId} ({notes.Count})");
        SampleOutput.WriteRows(notes, note => $"{note.ResourceId,8}  {Truncate(note.Content)}");
    }

    [ConsoleSample(Name = "List project notes")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task ListProjectNotesAsync(CancellationToken cancellationToken)
    {
        var project = await context.Data.GetFirstProjectAsync(cancellationToken);
        var notes = await context.Client.Notes.ListProjectNotesAsync(
            projectId: project.ResourceId,
            cancellationToken: cancellationToken);

        SampleOutput.WriteHeader($"Notes for project {project.Name} ({notes.Count})");
        SampleOutput.WriteRows(notes, note => $"{note.ResourceId,8}  {Truncate(note.Content)}");
    }

    [ConsoleSample(Name = "Get note detail")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task GetNoteDetailAsync(CancellationToken cancellationToken)
    {
        var note = await context.Data.GetFirstNoteAsync(cancellationToken);
        var detail = await context.Client.Notes.GetNoteAsync(note.ResourceId, cancellationToken: cancellationToken);

        SampleOutput.WriteHeader("Note detail");
        SampleOutput.WriteField("Id", detail.ResourceId);
        SampleOutput.WriteField("Content", detail.Content);
        SampleOutput.WriteField("Author", detail.Author);
        SampleOutput.WriteField("Contact ID", detail.ContactId);
        SampleOutput.WriteField("Project ID", detail.ProjectId);
    }

    [ConsoleSample(Name = "Create contact note")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateContactNoteAsync(CancellationToken cancellationToken)
    {
        var contact = await context.Data.GetFirstContactAsync(cancellationToken);
        var created = await context.Client.Notes.CreateContactNoteAsync(
            contact.ResourceId,
            CreateContactNoteRequest.Create($"Console sample contact note {DateTimeOffset.UtcNow:O}"),
            cancellationToken);

        SampleOutput.WriteHeader("Created contact note");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Content", created.Content);
    }

    [ConsoleSample(Name = "Create project note")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task CreateProjectNoteAsync(CancellationToken cancellationToken)
    {
        var project = await context.Data.GetFirstProjectAsync(cancellationToken);
        var created = await context.Client.Notes.CreateProjectNoteAsync(
            project.ResourceId,
            CreateProjectNoteRequest.Create($"Console sample project note {DateTimeOffset.UtcNow:O}"),
            cancellationToken);

        SampleOutput.WriteHeader("Created project note");
        SampleOutput.WriteField("Id", created.ResourceId);
        SampleOutput.WriteField("Content", created.Content);
    }

    [ConsoleSample(Name = "Update note")]
    [SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "Invoked via reflection by ConsoleSample attribute.")]
    private async Task UpdateNoteAsync(CancellationToken cancellationToken)
    {
        var note = await context.Data.GetFirstNoteAsync(cancellationToken);
        var updated = await context.Client.Notes.UpdateNoteAsync(
            note.ResourceId,
            UpdateNoteRequest.Create($"{note.Content} (updated by console sample)"),
            cancellationToken);

        SampleOutput.WriteHeader("Updated note");
        SampleOutput.WriteField("Id", updated.ResourceId);
        SampleOutput.WriteField("Content", updated.Content);
    }

    private static string Truncate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "(empty)";
        }

        return value.Length <= 60 ? value : $"{value[..57]}...";
    }
}
