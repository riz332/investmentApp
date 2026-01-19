namespace InvestmentApp.Api.DTOs;

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone
);