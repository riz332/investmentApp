using System.Net.Http.Json;
using System.Text.Json;
using InvestmentApp.Api.DTOs;
using Xunit;
using System.Net.Http.Headers;

namespace InvestmentApp.Api.Tests;

public class MeControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MeControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateMyCustomer_WithoutAuth_ReturnsUnauthorized()
    {
        var request = new CreateCustomerRequest("John", "Doe", "john@example.com", "123-456-7890");
        var response = await _client.PostAsJsonAsync("/api/me", request);
        
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateMyCustomer_AsUser_ReturnsCreated()
    {
        var userToken = await RegisterUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var request = new CreateCustomerRequest("Jane", "Smith", $"jane{Guid.NewGuid()}@example.com", "987-654-3210");
        var response = await _client.PostAsJsonAsync("/api/me", request);
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }

    private async Task<string> RegisterUserAsync()
    {
        var registerRequest = new RegisterRequest
        {
            Email = $"user{Guid.NewGuid()}@example.com",
            Password = "User123!",
            FirstName = "Test",
            LastName = "User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return authResponse!.Token;
    }
}