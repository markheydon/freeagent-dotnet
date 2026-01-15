using FreeAgent.Client.Http;
using Xunit;

namespace FreeAgent.Client.Tests.Http;

public class PaginatedResponseTests
{
    [Fact]
    public void HasNextPage_WithMorePages_ReturnsTrue()
    {
        var response = new PaginatedResponse<string>
        {
            Page = 1,
            PerPage = 10,
            Total = 25
        };

        Assert.True(response.HasNextPage);
    }

    [Fact]
    public void HasNextPage_WithoutMorePages_ReturnsFalse()
    {
        var response = new PaginatedResponse<string>
        {
            Page = 3,
            PerPage = 10,
            Total = 25
        };

        Assert.False(response.HasNextPage);
    }

    [Fact]
    public void NextPage_WithMorePages_ReturnsNextPageNumber()
    {
        var response = new PaginatedResponse<string>
        {
            Page = 1,
            PerPage = 10,
            Total = 25
        };

        Assert.Equal(2, response.NextPage);
    }

    [Fact]
    public void NextPage_WithoutMorePages_ReturnsNull()
    {
        var response = new PaginatedResponse<string>
        {
            Page = 3,
            PerPage = 10,
            Total = 25
        };

        Assert.Null(response.NextPage);
    }
}
