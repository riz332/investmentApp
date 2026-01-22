using InvestmentApp.Api.DTOs;
using InvestmentApp.Domain;
using InvestmentApp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvestmentApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public MeController(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateMyCustomer(CreateCustomerRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Unauthorized();

            if (user.CustomerId != null)
                return BadRequest("Customer profile already exists.");

            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            user.CustomerId = customer.CustomerId;
            await _userManager.UpdateAsync(user);

            return CreatedAtAction(nameof(CustomerResponse), new { id = customer.CustomerId }, customer);
        }
    }
}
