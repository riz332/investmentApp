using System.Net.Http.Json;
using System.Text.Json;
using InvestmentApp.Api.DTOs;
using Xunit;

namespace InvestmentApp.Api.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_ValidRequest_ReturnsToken()
    {
        await using var factory = new CustomWebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        
        var request = new RegisterRequest
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "Test123!",
            FirstName = "Test",
            LastName = "User"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", request);
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.Token);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        await using var factory = new CustomWebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        
        var email = $"login{Guid.NewGuid()}@example.com";
        
        // First register a user
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = "Test123!",
            FirstName = "Login",
            LastName = "User"
        };
        await client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Then login
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = "Test123!"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.Token);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        await using var factory = new CustomWebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}