using AutoMapper;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Domain;

namespace InvestmentApp.Api.Mappers;

public static class PerformanceMapper
{
    public static PerformanceResponse MapToDto(this IMapper mapper, Portfolio portfolio)
        => mapper.Map<PerformanceResponse>(portfolio);
}