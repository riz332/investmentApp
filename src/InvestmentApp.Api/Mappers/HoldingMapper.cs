using AutoMapper;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Domain;

namespace InvestmentApp.Api.Mappers;

public static class HoldingMapper
{
    // Convenience wrapper if you prefer calling map helpers.
    public static HoldingResponse MapToDto(this IMapper mapper, PortfolioHolding holding)
        => mapper.Map<HoldingResponse>(holding);
}