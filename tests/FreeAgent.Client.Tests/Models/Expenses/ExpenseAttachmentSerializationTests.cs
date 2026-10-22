using System.Text.Json;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Tests.Models.Expenses;

public class ExpenseAttachmentSerializationTests
{
    [Fact]
    public void Deserialize_KnownContentType_MapsToEnum()
    {
        const string json = """
            {
              "content_type": "image/png",
              "file_name": "receipt.png"
            }
            """;

        var attachment = JsonSerializer.Deserialize<ExpenseAttachment>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(attachment);
        Assert.Equal(BillAttachmentContentType.Png, attachment.ContentType);
    }

    [Fact]
    public void Deserialize_UnknownContentType_MapsToUnknown()
    {
        const string json = """
            {
              "content_type": "application/octet-stream"
            }
            """;

        var attachment = JsonSerializer.Deserialize<ExpenseAttachment>(json, FreeAgentJsonSerializer.Options);

        Assert.NotNull(attachment);
        Assert.Equal(BillAttachmentContentType.Unknown, attachment.ContentType);
    }
}
