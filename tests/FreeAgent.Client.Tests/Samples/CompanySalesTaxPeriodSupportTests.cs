#if NET10_0
using FreeAgent.Client.Samples.Shared;

namespace FreeAgent.Client.Tests.Samples;

public sealed class CompanySalesTaxPeriodSupportTests
{
    [Theory]
    [InlineData(CompanyType.UsLimitedLiabilityCompany, true)]
    [InlineData(CompanyType.UsPartnership, true)]
    [InlineData(CompanyType.UsSoleProprietor, true)]
    [InlineData(CompanyType.UsCCorp, true)]
    [InlineData(CompanyType.UsSCorp, true)]
    [InlineData(CompanyType.UniversalCompany, true)]
    [InlineData(CompanyType.UkLimitedCompany, false)]
    [InlineData(CompanyType.UkSoleTrader, false)]
    [InlineData(CompanyType.Unknown, false)]
    [InlineData(null, false)]
    public void SupportsSalesTaxPeriods_MatchesDocumentedCompanyTypes(CompanyType? companyType, bool expected)
    {
        Assert.Equal(expected, CompanySalesTaxPeriodSupport.SupportsSalesTaxPeriods(companyType));
    }
}
#endif
