using System.Globalization;
using System.Text.Json;
namespace FreeAgent.Client.Tests.Models.Attachments;

public class AttachmentModelSerializationTests
{
    [Fact]
    public void Deserialize_Attachment_ParsesAllDocumentedFields()
    {
        const string json = """
            {
              "url": "https://api.freeagent.com/v2/attachments/3",
              "content_src": "https://s3.amazonaws.com/freeagent-dev/attachments/1/original.png",
              "content_src_medium": "https://example.com/medium",
              "content_src_small": "https://example.com/small",
              "expires_at": "2020-12-03T10:27:52.000Z",
              "content_type": "image/png",
              "file_name": "barcode.png",
              "file_size": 7673,
              "description": "Receipt for coffee"
            }
            """;

        var attachment = JsonSerializer.Deserialize<Attachment>(json, FreeAgentJsonTestSupport.Options);

        Assert.NotNull(attachment);
        Assert.Equal(3, attachment.ResourceId);
        Assert.Equal(
            DateTimeOffset.Parse("2020-12-03T10:27:52.000Z", CultureInfo.InvariantCulture),
            attachment.ExpiresAt);
        Assert.Equal("image/png", attachment.ContentType);
        Assert.Equal("barcode.png", attachment.FileName);
        Assert.Equal(7673, attachment.FileSize);
        Assert.Equal("Receipt for coffee", attachment.Description);
    }
}
