using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Serialization;

namespace FreeAgent.Client.Tests.Infrastructure.Serialization;

public class ApiSortTests
{
    [Fact]
    public void ToWireValue_Ascending_UsesFieldWireName()
    {
        var sort = new ApiSort<InvoiceSortField>(InvoiceSortField.UpdatedAt);
        Assert.Equal("updated_at", sort.ToWireValue());
    }

    [Fact]
    public void ToWireValue_Descending_PrefixesHyphen()
    {
        var sort = new ApiSort<ContactSortField>(ContactSortField.Name, descending: true);
        Assert.Equal("-name", sort.ToWireValue());
    }
}
