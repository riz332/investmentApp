using FluentAssertions;
using InvestmentApp.Api;
using InvestmentApp.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

public class CustomersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CustomersControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomers_ReturnsSeededCustomers()
    {
        var response = await _client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();

        customers.Should().NotBeNull();
        customers.Should().NotBeEmpty();
    }
}