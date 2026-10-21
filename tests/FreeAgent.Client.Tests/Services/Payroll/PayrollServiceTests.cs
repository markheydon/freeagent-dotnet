using System.Net;
using System.Net.Http;
using FreeAgent.Client;
using FreeAgent.Client.Infrastructure.Http;
using FreeAgent.Client.Models.Payroll;
using FreeAgent.Client.Services.Payroll;
using FreeAgent.Client.Tests.TestSupport;

namespace FreeAgent.Client.Tests.Services.Payroll;

public class PayrollServiceTests
{
    private const int TaxYearEnd = 2026;

    [Fact]
    public async Task ListAsync_CallsDocumentedEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal($"/v2/payroll/{TaxYearEnd}", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "periods": [
                    {
                      "url": "https://api.freeagent.com/v2/payroll/2026/0",
                      "period": 0,
                      "frequency": "Monthly",
                      "dated_on": "2025-04-25",
                      "status": "filed"
                    }
                  ],
                  "payments": [
                    {
                      "due_on": "2025-05-22",
                      "amount_due": "738.19",
                      "status": "unpaid"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var taxYear = await service.ListAsync(TaxYearEnd);

        Assert.Single(taxYear.Periods!);
        Assert.Equal(0, taxYear.Periods![0].PeriodNumber);
        Assert.Equal(PayrollPeriodFilingStatus.Filed, taxYear.Periods[0].Status);
        Assert.Equal(PayrollPaymentStatus.Unpaid, taxYear.Payments![0].Status);
    }

    [Fact]
    public async Task ListPayslipsAsync_CallsDocumentedEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal($"/v2/payroll/{TaxYearEnd}/0", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "period": {
                    "url": "https://api.freeagent.com/v2/payroll/2026/0",
                    "period": 0,
                    "payslips": [
                      {
                        "user": "https://api.freeagent.com/v2/users/86",
                        "tax_code": "1100L",
                        "dated_on": "2025-04-25",
                        "basic_pay": "2500.0",
                        "ni_calc_type": "Director",
                        "student_loan_deductions_plan": "Plan 1"
                      }
                    ]
                  }
                }
                """)
            };
        });

        var service = CreateService(handler);
        var period = await service.ListPayslipsAsync(TaxYearEnd, 0);

        Assert.Equal(86, period.Payslips![0].UserId);
        Assert.Equal(PayrollNiCalculationType.Director, period.Payslips[0].NiCalcType);
        Assert.Equal(PayrollStudentLoanDeductionsPlan.Plan1, period.Payslips[0].StudentLoanDeductionsPlan);
    }

    [Fact]
    public async Task ListPayslipsAsync_ThrowsWhenPeriodOutOfRange()
    {
        var service = CreateService(new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ListPayslipsAsync(TaxYearEnd, 12));
    }

    [Fact]
    public async Task ListAsync_ThrowsWhenPeriodsMissing()
    {
        var handler = new QueueHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("""{ "payments": [] }""")
        });

        var service = CreateService(handler);

        await Assert.ThrowsAsync<FreeAgentApiException>(() => service.ListAsync(TaxYearEnd));
    }

    [Fact]
    public async Task MarkPaymentAsPaidAsync_CallsDocumentedPutEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal($"/v2/payroll/{TaxYearEnd}/payments/2025-05-22/mark_as_paid", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "periods": [],
                  "payments": [
                    {
                      "due_on": "2025-05-22",
                      "amount_due": "738.19",
                      "status": "paid"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var result = await service.MarkPaymentAsPaidAsync(TaxYearEnd, new DateOnly(2025, 5, 22));

        Assert.Equal(PayrollPaymentStatus.Paid, result.Payments![0].Status);
    }

    [Fact]
    public async Task MarkPaymentAsUnpaidAsync_CallsDocumentedGetEndpoint()
    {
        var handler = new QueueHttpMessageHandler(request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal($"/v2/payroll/{TaxYearEnd}/payments/2025-05-22/mark_as_unpaid", request.RequestUri!.AbsolutePath);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "periods": [],
                  "payments": [
                    {
                      "due_on": "2025-05-22",
                      "amount_due": "738.19",
                      "status": "unpaid"
                    }
                  ]
                }
                """)
            };
        });

        var service = CreateService(handler);
        var result = await service.MarkPaymentAsUnpaidAsync(TaxYearEnd, new DateOnly(2025, 5, 22));

        Assert.Equal(PayrollPaymentStatus.Unpaid, result.Payments![0].Status);
    }

    private static PayrollService CreateService(QueueHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.freeagent.com/v2/") };
        var client = new FreeAgentHttpClient(httpClient, "test-token", new FreeAgentHttpClientOptions { MinimumRequestSpacing = TimeSpan.Zero });
        return new PayrollService(client);
    }
}
