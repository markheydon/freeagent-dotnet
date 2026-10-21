using System.Text.Json;
namespace FreeAgent.Client.Tests.Models.Attachments;

public class AttachmentWritePayloadSerializationTests
{
    [Fact]
    public void ExpenseAttachmentWritePayload_SerialisesExpectedFields()
    {
        var json = JsonSerializer.Serialize(new ExpenseAttachmentWritePayload
        {
            Data = "aGVsbG8=",
            FileName = "receipt.png",
            ContentType = BillAttachmentContentType.Png,
            Description = "Scan"
        });

        Assert.Contains("\"data\":\"aGVsbG8=\"", json, StringComparison.Ordinal);
        Assert.Contains("\"file_name\":\"receipt.png\"", json, StringComparison.Ordinal);
        Assert.Contains("\"content_type\":\"image/png\"", json, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"Scan\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void BillAttachmentWritePayload_SerialisesExpectedFields()
    {
        var json = JsonSerializer.Serialize(new BillAttachmentWritePayload
        {
            Data = "cGRm",
            FileName = "invoice.pdf",
            ContentType = BillAttachmentContentType.Pdf
        });

        Assert.Contains("\"data\":\"cGRm\"", json, StringComparison.Ordinal);
        Assert.Contains("\"file_name\":\"invoice.pdf\"", json, StringComparison.Ordinal);
        Assert.Contains("\"content_type\":\"application/x-pdf\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void BankTransactionExplanationAttachmentWritePayload_SerialisesExpectedFields()
    {
        var json = JsonSerializer.Serialize(new BankTransactionExplanationAttachmentWritePayload
        {
            Data = "ZmlsZQ==",
            FileName = "statement.png",
            ContentType = BillAttachmentContentType.Png
        });

        Assert.Contains("\"data\":\"ZmlsZQ==\"", json, StringComparison.Ordinal);
        Assert.Contains("\"file_name\":\"statement.png\"", json, StringComparison.Ordinal);
        Assert.Contains("\"content_type\":\"image/png\"", json, StringComparison.Ordinal);
    }
}
