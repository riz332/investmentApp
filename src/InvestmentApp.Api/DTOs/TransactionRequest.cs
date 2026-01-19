using InvestmentApp.Domain;

namespace InvestmentApp.Api.DTOs;

public record TransactionRequest(
    Guid AssetId,
    decimal Quantity,
    decimal PricePerUnit,
    TransactionType Type
);