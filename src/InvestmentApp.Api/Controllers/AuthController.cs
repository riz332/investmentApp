using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InvestmentApp.Domain;
using InvestmentApp.Infrastructure;
using InvestmentApp.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace InvestmentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    AppDbContext db,
    IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and Password are required." });

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null) return BadRequest(new { error = "Email already in use." });

        // Create domain Customer if name info present (optional)
        var customer = new Customer
        {
            CustomerId = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            CustomerId = customer.CustomerId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        var token = await GenerateJwtToken(user);
        return Ok(token);
    }

    [HttpPost("registerAdmin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthResponse>> RegisterAdmin(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and Password are required." });

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null) return BadRequest(new { error = "Email already in use." });

        // Create domain Customer if name info present (optional)
        var customer = new Customer
        {
            CustomerId = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            CustomerId = customer.CustomerId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "Admin");

        var token = await GenerateJwtToken(user);
        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and Password are required." });

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) return Unauthorized(new { error = "Invalid credentials." });

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid) return Unauthorized(new { error = "Invalid credentials." });

        var token = await GenerateJwtToken(user);
        return Ok(token);
    }

    private async Task<AuthResponse> GenerateJwtToken(ApplicationUser user)
    {
        var keyStr = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        if (string.IsNullOrEmpty(keyStr)) throw new InvalidOperationException("Jwt:Key is not configured.");
        if (string.IsNullOrEmpty(issuer)) throw new InvalidOperationException("Jwt:Issuer is not configured.");
        if (string.IsNullOrEmpty(audience)) throw new InvalidOperationException("Jwt:Audience is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
    };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse { Token = tokenString, ExpiresAt = expires };
    }
}