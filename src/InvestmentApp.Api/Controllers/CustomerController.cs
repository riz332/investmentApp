using AutoMapper;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Domain;
using InvestmentApp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InvestmentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CustomerController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetCustomers()
    {
        var customers = await _db.Customers
            .Include(c => c.Portfolios)
                .ThenInclude(p => p.Holdings)
                    .ThenInclude(h => h.Asset)
            .Include(c => c.Portfolios)
                .ThenInclude(p => p.Transactions)
                    .ThenInclude(t => t.Asset)
            .ToListAsync();

        var result = _mapper.Map<IEnumerable<CustomerResponse>>(customers);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetCustomer(Guid id)
    {
        var customer = await _db.Customers
            .Include(c => c.Portfolios)
                .ThenInclude(p => p.Holdings)
                    .ThenInclude(h => h.Asset)
            .Include(c => c.Portfolios)
                .ThenInclude(p => p.Transactions)
                    .ThenInclude(t => t.Asset)
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customer.CustomerId.ToString() != userId)
        {
            return Forbid();
        }

        var dto = _mapper.Map<CustomerResponse>(customer);
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer(CreateCustomerRequest request)
    {
        var customer = _mapper.Map<Customer>(request);
        customer.CustomerId = Guid.NewGuid();
        customer.CreatedAt = DateTime.UtcNow;

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var dto = _mapper.Map<CustomerResponse>(customer);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, dto);
    }
}