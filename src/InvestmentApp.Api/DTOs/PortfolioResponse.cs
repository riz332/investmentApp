namespace InvestmentApp.Api.DTOs;

public record PortfolioResponse(
    Guid PortfolioId,
    string Name,
    string? Description,
    DateTime CreatedAt,
    IEnumerable<HoldingResponse> Holdings,
    IEnumerable<TransactionResponse> Transactions
);