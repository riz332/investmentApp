using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InvestmentApp.Api.Tests;

public class PortfoliosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PortfoliosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<Guid> CreateTestCustomerAsync(HttpClient client, string email)
    {
        var payload = new
        {
            FirstName = "Bob",
            LastName = "Builder",
            Email = email,
            Phone = "555-0202"
        };

        var createResp = await client.PostAsJsonAsync("/api/customers", payload);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // Read created customer from body (fallback to listing)
        var createdJson = await createResp.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(createdJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(createdJson);

                // Explicitly check each property name separately to avoid unassigned out variables
                if (doc.RootElement.TryGetProperty("customerId", out var idProp))
                {
                    if (Guid.TryParse(idProp.GetString(), out var id)) return id;
                }
                else if (doc.RootElement.TryGetProperty("CustomerId", out var idPropUpper))
                {
                    if (Guid.TryParse(idPropUpper.GetString(), out var id)) return id;
                }
            }
            catch { /* ignore and fall back */ }
        }

        // fallback: get first customer with matching email
        var listResp = await client.GetFromJsonAsync<JsonElement[]>("/api/customers");
        var match = listResp!.FirstOrDefault(e =>
            (e.TryGetProperty("email", out var p) && string.Equals(p.GetString(), email, StringComparison.OrdinalIgnoreCase)) ||
            (e.TryGetProperty("Email", out var p2) && string.Equals(p2.GetString(), email, StringComparison.OrdinalIgnoreCase)));

        if (match.ValueKind != JsonValueKind.Undefined)
        {
            // Check both possible property names explicitly to avoid use of an unassigned out variable
            if (match.TryGetProperty("customerId", out var idProp))
            {
                if (Guid.TryParse(idProp.GetString(), out var id)) return id;
            }

            if (match.TryGetProperty("CustomerId", out var idProp2))
            {
                if (Guid.TryParse(idProp2.GetString(), out var id)) return id;
            }
        }

        throw new InvalidOperationException("Could not determine created customer id.");
    }

    [Fact]
    public async Task CreatePortfolio_ReturnsCreated_And_Listed()
    {
        using var client = _factory.CreateClient();

        var email = $"bob.{Guid.NewGuid():N}@example.com";
        var customerId = await CreateTestCustomerAsync(client, email);

        var portfolioPayload = new
        {
            CustomerId = customerId,
            Name = "Test Portfolio",
            Description = "Created by tests"
        };

        var createResp = await client.PostAsJsonAsync("/api/portfolios", portfolioPayload);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // GET portfolios for customer
        var listResp = await client.GetAsync($"/api/portfolios/customer/{customerId}");
        listResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await listResp.Content.ReadAsStringAsync();
        json.Should().Contain("Test Portfolio");
    }

    [Fact]
    public async Task GetPortfolio_NotFound_Returns404()
    {
        using var client = _factory.CreateClient();

        var missingId = Guid.NewGuid();
        var resp = await client.GetAsync($"/api/portfolios/{missingId}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}