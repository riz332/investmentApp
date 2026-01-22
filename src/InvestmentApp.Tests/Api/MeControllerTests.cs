using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InvestmentApp.Api.Tests;

public class MeControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MeControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateMyCustomer_ReturnsCreated_WhenValidRequest()
    {
        // Arrange
        var client = await _factory.CreateAuthenticatedClientAsync();
        var request = new
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Phone = "555-0100"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Me", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateMyCustomer_ReturnsBadRequest_WhenCustomerAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var client = await _factory.CreateAuthenticatedClientAsync(userId: userId);
        var request = new
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            Phone = "555-0101"
        };

        // Act - First request to create customer
        var firstResponse = await client.PostAsJsonAsync("/api/Me", request);
        firstResponse.EnsureSuccessStatusCode();

        // Act - Second request
        var secondResponse = await client.PostAsJsonAsync("/api/Me", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task CreateMyCustomer_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var client = _factory.CreateClient(); // Unauthenticated
        var request = new
        {
            FirstName = "Ghost",
            LastName = "User",
            Email = "ghost@example.com",
            Phone = "000-0000"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Me", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}