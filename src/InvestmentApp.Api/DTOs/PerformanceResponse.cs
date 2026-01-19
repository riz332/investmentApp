namespace InvestmentApp.Api.DTOs;

public record PerformanceResponse(
    decimal TotalValue,
    decimal TotalCost,
    decimal UnrealizedGain,
    decimal ReturnPercentage
);