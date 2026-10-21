namespace FreeAgent.Client;

/// <summary>
/// Documented EC VAT MOSS write constraints from FreeAgent sales tax guidance.
/// </summary>
public static class EcMossWriteRequirements
{
    /// <summary>
    /// Validates MOSS fields for invoice-style documents (invoice, estimate, credit note, recurring invoice).
    /// </summary>
    /// <param name="ecStatus">Document EC status.</param>
    /// <param name="placeOfSupply">Place of supply when MOSS applies.</param>
    /// <param name="placeOfSupplyParameterName">Parameter name for exception messaging.</param>
    public static void ValidateInvoiceEcStatus(InvoiceEcStatus? ecStatus, EcMossPlaceOfSupply? placeOfSupply, string placeOfSupplyParameterName = "placeOfSupply")
    {
        if (ecStatus == InvoiceEcStatus.EcVatMoss && placeOfSupply is null)
        {
            throw new ArgumentException(
                "place_of_supply is required when ec_status is EC VAT MOSS.",
                placeOfSupplyParameterName);
        }
    }

    /// <summary>
    /// Validates MOSS fields for estimates.
    /// </summary>
    /// <param name="ecStatus">Estimate EC status.</param>
    /// <param name="placeOfSupply">Place of supply when MOSS applies.</param>
    /// <param name="placeOfSupplyParameterName">Parameter name for exception messaging.</param>
    public static void ValidateEstimateEcStatus(EstimateEcStatus? ecStatus, EcMossPlaceOfSupply? placeOfSupply, string placeOfSupplyParameterName = "placeOfSupply")
    {
        if (ecStatus == EstimateEcStatus.EcVatMoss && placeOfSupply is null)
        {
            throw new ArgumentException(
                "place_of_supply is required when ec_status is EC VAT MOSS.",
                placeOfSupplyParameterName);
        }
    }

    /// <summary>
    /// Validates MOSS fields for bank transaction explanations.
    /// </summary>
    /// <param name="ecStatus">Explanation EC status.</param>
    /// <param name="placeOfSupply">Place of supply when MOSS applies.</param>
    /// <param name="salesTaxRate">Sales tax rate when MOSS applies.</param>
    public static void ValidateBankExplanation(
        BankTransactionExplanationEcStatus? ecStatus,
        EcMossPlaceOfSupply? placeOfSupply,
        decimal? salesTaxRate)
    {
        if (ecStatus != BankTransactionExplanationEcStatus.EcVatMoss)
        {
            return;
        }

        if (placeOfSupply is null)
        {
            throw new ArgumentException(
                "place_of_supply is required when ec_status is EC VAT MOSS.",
                nameof(placeOfSupply));
        }

        if (salesTaxRate is null)
        {
            throw new ArgumentException(
                "sales_tax_rate is required when ec_status is EC VAT MOSS.",
                nameof(salesTaxRate));
        }
    }
}
