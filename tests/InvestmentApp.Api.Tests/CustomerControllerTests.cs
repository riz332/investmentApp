using System.Net.Http.Json;
using System.Text.Json;
using InvestmentApp.Api.DTOs;
using Xunit;
using System.Net.Http.Headers;

namespace InvestmentApp.Api.Tests;

public class CustomerControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CustomerControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCustomers_WithoutAuth_ReturnsUnauthorized()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/customer");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_AsAdmin_ReturnsCreated()
    {
        using var client = _factory.CreateClient();
        
        // Register admin user
        var adminToken = await RegisterAdminUserAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var request = new CreateCustomerRequest("John", "Doe", $"john{Guid.NewGuid()}@example.com", "123-456-7890");
        var response = await client.PostAsJsonAsync("/api/customer", request);
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var customer = JsonSerializer.Deserialize<CustomerResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(customer);
        Assert.Equal("John", customer.FirstName);
        Assert.Equal("Doe", customer.LastName);
    }

    private async Task<string> RegisterAdminUserAsync(HttpClient client)
    {
        var registerRequest = new RegisterRequest
        {
            Email = $"admin{Guid.NewGuid()}@example.com",
            Password = "Admin123!",
            FirstName = "Admin",
            LastName = "User"
        };

        var response = await client.PostAsJsonAsync("/api/auth/registerAdmin", registerRequest);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return authResponse!.Token;
    }
}