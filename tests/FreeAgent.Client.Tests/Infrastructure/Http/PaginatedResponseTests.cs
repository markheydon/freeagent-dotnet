using FreeAgent.Client;
using Xunit;

namespace FreeAgent.Client.Tests.Infrastructure.Http;

public class PaginatedResponseTests
{
    [Fact]
    public void HasNextPage_WithMorePages_ReturnsTrue()
    {
        var response = new PaginatedResponse<string>(1, 10, 25, Array.Empty<string>());

        Assert.True(response.HasNextPage);
    }

    [Fact]
    public void HasNextPage_WithoutMorePages_ReturnsFalse()
    {
        var response = new PaginatedResponse<string>(3, 10, 25, Array.Empty<string>());

        Assert.False(response.HasNextPage);
    }

    [Fact]
    public void NextPage_WithMorePages_ReturnsNextPageNumber()
    {
        var response = new PaginatedResponse<string>(1, 10, 25, Array.Empty<string>());

        Assert.Equal(2, response.NextPage);
    }

    [Fact]
    public void NextPage_WithoutMorePages_ReturnsNull()
    {
        var response = new PaginatedResponse<string>(3, 10, 25, Array.Empty<string>());

        Assert.Null(response.NextPage);
    }
}
