using FreeAgent.Client.Models.BankTransactionExplanations;
using FreeAgent.Client.Models.EcMoss;
using FreeAgent.Client.Models.Invoices;

namespace FreeAgent.Client.Tests.Models.EcMoss;

public class EcMossWriteRequirementsTests
{
    [Fact]
    public void ValidateInvoiceEcStatus_MossWithoutPlaceOfSupply_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EcMossWriteRequirements.ValidateInvoiceEcStatus(InvoiceEcStatus.EcVatMoss, null));

        Assert.Contains("place_of_supply", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateBankExplanation_MossWithoutSalesTaxRate_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            EcMossWriteRequirements.ValidateBankExplanation(
                BankTransactionExplanationEcStatus.EcVatMoss,
                EcMossPlaceOfSupply.Austria,
                null));

        Assert.Contains("sales_tax_rate", exception.Message, StringComparison.Ordinal);
    }
}
