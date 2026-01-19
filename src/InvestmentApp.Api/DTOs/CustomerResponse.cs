namespace InvestmentApp.Api.DTOs;

public record CustomerResponse(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    DateTime CreatedAt,
    IEnumerable<PortfolioResponse> Portfolios
);