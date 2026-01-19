using AutoMapper;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Domain;

namespace InvestmentApp.Api.Mappers;

public static class TransactionMapper
{
    public static TransactionResponse MapToDto(this IMapper mapper, Transaction transaction)
        => mapper.Map<TransactionResponse>(transaction);
}