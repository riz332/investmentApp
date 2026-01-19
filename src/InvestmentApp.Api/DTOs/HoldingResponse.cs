namespace InvestmentApp.Api.DTOs;

public record HoldingResponse(
    Guid HoldingId,
    string Symbol,
    decimal Quantity,
    decimal AveragePrice
);