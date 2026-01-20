using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using InvestmentApp.Api.DTOs;
using Xunit;

namespace InvestmentApp.Api.Tests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    // Use xUnit fixture injection so the test host is reused per-class if desired
    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Returns_Token()
    {
        using var client = _factory.CreateClient();

        var register = new RegisterRequest
        {
            Email = "test.register@example.com",
            Password = "Passw0rd!",
            FirstName = "Test",
            LastName = "Register"
        };

        var resp = await client.PostAsJsonAsync("/api/auth/register", register);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>();
        auth.Should().NotBeNull();
        auth!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_Returns_Token_After_Register()
    {
        using var client = _factory.CreateClient();

        var email = "test.login@example.com";
        var password = "Passw0rd!";

        var register = new RegisterRequest
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "Login"
        };

        // Register first
        var regResp = await client.PostAsJsonAsync("/api/auth/register", register);
        regResp.EnsureSuccessStatusCode();

        // Then login
        var login = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var loginResp = await client.PostAsJsonAsync("/api/auth/login", login);
        loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
        auth.Should().NotBeNull();
        auth!.Token.Should().NotBeNullOrEmpty();
    }
}