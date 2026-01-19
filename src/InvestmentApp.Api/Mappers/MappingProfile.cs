using AutoMapper;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Domain;

namespace InvestmentApp.Api.Mappers;

internal class PortfolioToPerformanceConverter : ITypeConverter<Portfolio, PerformanceResponse>
{
    public PerformanceResponse Convert(Portfolio source, PerformanceResponse? destination, ResolutionContext context)
    {
        decimal totalValue = 0m;
        decimal totalCost = 0m;

        foreach (var h in source.Holdings)
        {
            var latest = h.Asset.Prices
                .OrderByDescending(x => x.PriceDate)
                .FirstOrDefault();

            if (latest == null)
                continue;

            totalValue += latest.Price * h.Quantity;
            totalCost += h.AveragePrice * h.Quantity;
        }

        var gain = totalValue - totalCost;
        var pct = totalCost == 0 ? 0 : gain / totalCost;

        return new PerformanceResponse(totalValue, totalCost, gain, pct);
    }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PortfolioHolding, HoldingResponse>()
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Asset.Symbol));

        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Asset.Symbol))
            // Transaction.TransactionType is now an int FK; convert it to the enum used by the DTO
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (TransactionType)src.TransactionType));

        CreateMap<Portfolio, PortfolioResponse>()
            // record constructor parameter names match property names; AutoMapper will bind them.
            .ForMember(dest => dest.Holdings, opt => opt.MapFrom(src => src.Holdings))
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

        // Portfolio -> PerformanceResponse uses a converter because it contains custom aggregation logic
        CreateMap<Portfolio, PerformanceResponse>().ConvertUsing<PortfolioToPerformanceConverter>();
    }
}