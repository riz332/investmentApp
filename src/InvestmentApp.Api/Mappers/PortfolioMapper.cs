using AutoMapper;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Domain;

namespace InvestmentApp.Api.Mappers;

public static class PortfolioMapper
{
    public static PortfolioResponse MapToDto(this IMapper mapper, Portfolio portfolio)
        => mapper.Map<PortfolioResponse>(portfolio);
}