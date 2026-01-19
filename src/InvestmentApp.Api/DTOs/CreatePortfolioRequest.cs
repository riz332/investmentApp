namespace InvestmentApp.Api.DTOs;

public record CreatePortfolioRequest(
    Guid CustomerId,
    string Name,
    string? Description
);