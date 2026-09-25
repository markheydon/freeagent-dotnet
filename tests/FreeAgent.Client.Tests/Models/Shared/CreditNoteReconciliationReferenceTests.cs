using FreeAgent.Client;
using FreeAgent.Client.Models.Shared;

namespace FreeAgent.Client.Tests.Models.Shared;

public class CreditNoteReconciliationReferenceTests
{
    [Fact]
    public void ForEnvironment_BuildsExpectedUri()
    {
        var reference = CreditNoteReconciliationReference.ForEnvironment(FreeAgentEnvironment.Production, 42);

        Assert.Equal("https://api.freeagent.com/v2/credit_note_reconciliations/42", reference.Uri);
        Assert.Equal(42, reference.Id);
    }

    [Fact]
    public void Parse_ValidUri_ReturnsReference()
    {
        var reference = CreditNoteReconciliationReference.Parse(
            "https://api.freeagent.com/v2/credit_note_reconciliations/7");

        Assert.Equal(7, reference.Id);
        Assert.Equal("https://api.freeagent.com/v2/credit_note_reconciliations/7", reference.Uri);
    }
}
