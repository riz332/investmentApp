using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using InvestmentApp.Api.DTOs;

namespace InvestmentApp.Api.Tests;

public class CustomersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CustomersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCustomer_IsCreated_ThenListed()
    {
        using var client = _factory.CreateClient();

        var payload = new
        {
            FirstName = "Alice",
            LastName = "Anderson",
            Email = $"alice.{Guid.NewGuid():N}@example.com",
            Phone = "555-0101"
        };

        var createResp = await client.PostAsJsonAsync("/api/customers", payload);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // Ensure the created customer appears in GET /api/customers
        var listResp = await client.GetAsync("/api/customers");
        listResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await listResp.Content.ReadAsStringAsync();
        content.Should().Contain(payload.Email);
    }

    [Fact]
    public async Task GetCustomers_ReturnsEmptyOrMore()
    {
        using var client = _factory.CreateClient();

        var resp = await client.GetAsync("/api/customers");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await resp.Content.ReadAsStringAsync();
        // Response should be a JSON array; at minimum it should parse
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task GetCustomers_ReturnsSeededCustomers()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customers = await response.Content.ReadFromJsonAsync<List<CustomerResponse>>();

        customers.Should().NotBeNull();
        customers.Should().NotBeEmpty();

        // basic shape assertions to ensure DTO mapping is correct
        customers.ForEach(c =>
        {
            c.CustomerId.Should().NotBeEmpty();
            c.FirstName.Should().NotBeNullOrWhiteSpace();
            c.Email.Should().NotBeNullOrWhiteSpace();
            c.Portfolios.Should().NotBeNull();
        });
    }
}