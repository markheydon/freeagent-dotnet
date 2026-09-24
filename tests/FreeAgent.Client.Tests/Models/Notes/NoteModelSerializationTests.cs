using System.Text.Json;
using FreeAgent.Client.Models.Notes;

namespace FreeAgent.Client.Tests.Models.Notes;

public class NoteModelSerializationTests
{
    [Fact]
    public void Deserialize_ReadsContentTimestampsAndParentUrl()
    {
        const string json = """
        {
          "url": "https://api.freeagent.com/v2/notes/1",
          "note": "A new note",
          "parent_url": "https://api.freeagent.com/v2/contacts/1",
          "author": "Development Team",
          "created_at": "2012-05-30T10:22:34Z",
          "updated_at": "2012-05-30T10:22:34Z"
        }
        """;

        var note = JsonSerializer.Deserialize<Note>(json);

        Assert.Equal("https://api.freeagent.com/v2/notes/1", note!.Url);
        Assert.Equal("A new note", note.Content);
        Assert.Equal(1, note.ContactId);
        Assert.Null(note.ProjectId);
        Assert.Equal("Development Team", note.Author);
        Assert.Equal(new DateTimeOffset(2012, 5, 30, 10, 22, 34, TimeSpan.Zero), note.CreatedAt);
        Assert.Equal(new DateTimeOffset(2012, 5, 30, 10, 22, 34, TimeSpan.Zero), note.UpdatedAt);
    }

    [Fact]
    public void Deserialize_ProjectParentUrl_ParsesProjectId()
    {
        const string json = """
        {
          "parent_url": "https://api.freeagent.com/v2/project/9"
        }
        """;

        var note = JsonSerializer.Deserialize<Note>(json);

        Assert.Equal(9, note!.ProjectId);
        Assert.Null(note.ContactId);
    }

    [Fact]
    public void Deserialize_ProjectsParentUrl_ParsesProjectId()
    {
        const string json = """
        {
          "parent_url": "https://api.freeagent.com/v2/projects/9"
        }
        """;

        var note = JsonSerializer.Deserialize<Note>(json);

        Assert.Equal(9, note!.ProjectId);
        Assert.Null(note.ContactId);
    }
}
