#if NET10_0
using FreeAgent.Client.BlazorSample.Navigation;

namespace FreeAgent.Client.Tests.Samples.Blazor;

public sealed class SdkExplorerNavCatalogTests
{
    [Fact]
    public void TotalItemCount_MatchesCatalogEntries()
    {
        Assert.Equal(82, SdkExplorerNavMatching.TotalItemCount(SdkExplorerNavCatalog.Groups));
    }

    [Fact]
    public void Filter_inv_MatchesInvoiceAndRecurringInvoiceLinksOnly()
    {
        var matches = SdkExplorerNavCatalog.Groups
            .SelectMany(group => SdkExplorerNavMatching.GetVisibleItems(group, "inv"))
            .Select(item => item.Title)
            .OrderBy(title => title)
            .ToList();

        Assert.Equal(
            [
                "Invoice CRUD",
                "Invoices list",
                "Recurring invoice detail",
                "Recurring invoices list",
                "Turpinverse seed",
            ],
            matches);
    }

    [Fact]
    public void RouteMatches_NestedTaxAndPayrollPaths()
    {
        var taxes = SdkExplorerNavCatalog.Groups.Single(group => group.Title == "Taxes");
        var corpDetail = taxes.Items.Single(item => item.Href == "/corporation-tax-returns/detail");
        var payroll = SdkExplorerNavCatalog.Groups.Single(group => group.Title == "Payroll");
        var period = payroll.Items.Single(item => item.Href == "/payroll/period");

        Assert.True(SdkExplorerNavMatching.RouteMatches("corporation-tax-returns/detail", corpDetail));
        Assert.True(SdkExplorerNavMatching.RouteMatches("payroll/period", period));
        Assert.False(SdkExplorerNavMatching.RouteMatches("payroll-profiles", period));
    }

    [Fact]
    public void RegroupedDomains_ContainExpectedHrefs()
    {
        static IReadOnlyList<string> Hrefs(string groupTitle) =>
            SdkExplorerNavCatalog.Groups.Single(g => g.Title == groupTitle).Items.Select(i => i.Href).ToList();

        Assert.Contains("/payroll", Hrefs("Payroll"));
        Assert.Contains("/cis-settings", Hrefs("CIS"));
        Assert.Contains("/vat-returns", Hrefs("Taxes"));
        Assert.Contains("/sales-tax-periods", Hrefs("Taxes"));
        Assert.DoesNotContain("/payroll", Hrefs("Assets and journals"));
        Assert.DoesNotContain("/vat-returns", Hrefs("Reports"));
    }
}
#endif
