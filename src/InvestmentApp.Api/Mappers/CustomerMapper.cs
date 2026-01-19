using AutoMapper;
using InvestmentApp.Api.DTOs;

namespace InvestmentApp.Api.Mappers
{
    public static class CustomerMapper
    {
        // Convenience wrapper if you prefer calling map helpers.
        public static CustomerResponse MapToDto(this IMapper mapper, Domain.Customer customer)
            => mapper.Map<CustomerResponse>(customer);
    }
}
