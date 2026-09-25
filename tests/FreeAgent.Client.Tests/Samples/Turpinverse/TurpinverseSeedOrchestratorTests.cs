#if NET10_0
using FreeAgent.Client.Samples.Shared.Turpinverse;

namespace FreeAgent.Client.Tests.Samples.Turpinverse;

public class TurpinverseSeedOrchestratorTests
{
    [Fact]
    public async Task SeedAllAsync_NullClient_ThrowsArgumentNullException()
    {
        var orchestrator = CreateOrchestrator();

        await Assert.ThrowsAsync<ArgumentNullException>(() => orchestrator.SeedAllAsync(null!));
    }

    private static TurpinverseSeedOrchestrator CreateOrchestrator()
    {
        var canonClient = new TurpinverseCanonClient(
            new HttpClient(),
            Microsoft.Extensions.Options.Options.Create(new TurpinverseCanonOptions()));
        var contactCatalog = new TurpinverseContactCatalog(canonClient);
        var projectCatalog = new TurpinverseProjectCatalog(canonClient);
        var taskSeeder = new TurpinverseTaskSeeder(projectCatalog);
        var companyDates = new TurpinverseCompanyDates();

        var invoiceCatalog = new TurpinverseInvoiceCatalog(canonClient);
        var quoteCatalog = new TurpinverseQuoteCatalog(canonClient);
        var creditNoteCatalog = new TurpinverseCreditNoteCatalog(canonClient);

        return new TurpinverseSeedOrchestrator(
            new TurpinverseCanonCoordinator(
                canonClient,
                contactCatalog,
                projectCatalog,
                invoiceCatalog,
                quoteCatalog,
                creditNoteCatalog),
            new TurpinverseContactSeeder(contactCatalog),
            new TurpinverseProjectSeeder(projectCatalog, contactCatalog),
            taskSeeder,
            new TurpinverseInvoiceSeeder(
                invoiceCatalog,
                contactCatalog,
                projectCatalog,
                companyDates),
            new TurpinverseQuoteSeeder(
                quoteCatalog,
                contactCatalog,
                projectCatalog,
                companyDates),
            new TurpinverseCreditNoteSeeder(
                creditNoteCatalog,
                contactCatalog,
                companyDates),
            new TurpinverseNoteSeeder(contactCatalog, projectCatalog),
            new TurpinverseTimeslipSeeder(taskSeeder));
    }
}

#endif
