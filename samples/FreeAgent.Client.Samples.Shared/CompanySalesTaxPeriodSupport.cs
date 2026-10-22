namespace FreeAgent.Client.Samples.Shared;

/// <summary>
/// Company-type guard for the sales tax periods API (US and universal companies only).
/// </summary>
public static class CompanySalesTaxPeriodSupport
{
    /// <summary>
    /// Whether <c>GET /v2/sales_tax_periods</c> is available for the company type (per FreeAgent docs).
    /// </summary>
    public static bool SupportsSalesTaxPeriods(CompanyType? companyType) =>
        companyType is CompanyType.UsLimitedLiabilityCompany
            or CompanyType.UsPartnership
            or CompanyType.UsSoleProprietor
            or CompanyType.UsCCorp
            or CompanyType.UsSCorp
            or CompanyType.UniversalCompany;
}
