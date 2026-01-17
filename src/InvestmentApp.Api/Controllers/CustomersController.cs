using InvestmentApp.Domain;
using InvestmentApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        var customers = await _db.Customers
            .Include(c => c.Portfolios)
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Customer>> GetCustomer(Guid id)
    {
        var customer = await _db.Customers
            .Include(c => c.Portfolios)
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer is null) return NotFound();
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
    {
        customer.CustomerId = Guid.NewGuid();
        customer.CreatedAt = DateTime.UtcNow;

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
    }
}