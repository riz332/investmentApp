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
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Symbol : string.Empty));

        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Symbol : string.Empty))
            // Transaction.TransactionType is an int FK; convert to enum for DTO
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (TransactionType)src.TransactionType));

        CreateMap<Portfolio, PortfolioResponse>()
            .ForMember(dest => dest.Holdings, opt => opt.MapFrom(src => src.Holdings))
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

        // Customer mappings
        CreateMap<Domain.Customer, CustomerResponse>()
            .ForMember(dest => dest.Portfolios, opt => opt.MapFrom(src => src.Portfolios));

        CreateMap<CreateCustomerRequest, Domain.Customer>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Portfolios, opt => opt.Ignore());

        // Portfolio -> PerformanceResponse uses a converter because it contains custom aggregation logic
        CreateMap<Portfolio, PerformanceResponse>().ConvertUsing<PortfolioToPerformanceConverter>();
    }
}