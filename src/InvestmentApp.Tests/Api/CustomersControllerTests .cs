using FluentAssertions;
using InvestmentApp.Api;
using InvestmentApp.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

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