using InvestmentApp.Domain;

namespace InvestmentApp.Api.DTOs;

public record TransactionResponse(
    Guid TransactionId,
    string Symbol,
    decimal Quantity,
    decimal PricePerUnit,
    decimal TotalAmount,
    TransactionType Type,
    DateTime ExecutedAt
);