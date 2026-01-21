using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InvestmentApp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace InvestmentApp.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _inMemoryDbName;

    public CustomWebApplicationFactory()
    {
        _inMemoryDbName = Guid.NewGuid().ToString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test_jwt_key_which_is_long_enough_32_bytes_min",
                ["Jwt:Issuer"] = "InvestmentApp",
                ["Jwt:Audience"] = "InvestmentAppClient"
            };
            conf.AddInMemoryCollection(dict);
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing AppDbContext registrations
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            // Add EF InMemory provider
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_inMemoryDbName);
                options.UseInternalServiceProvider(efServiceProvider);
            });

            // Build provider and ensure DB exists
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }

    // -----------------------------
    // 🔐 AUTH HELPERS FOR TESTS
    // -----------------------------
    public async Task<string> GenerateJwtForTestUserAsync(bool asAdmin)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Ensure roles exist
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole<Guid>("User"));

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));

        // Create test user
        var user = new ApplicationUser
        {
            UserName = $"testuser_{Guid.NewGuid():N}@example.com",
            Email = $"testuser_{Guid.NewGuid():N}@example.com"
        };

        await userManager.CreateAsync(user, "Password123!");

        // Assign role
        var role = asAdmin ? "Admin" : "User";
        await userManager.AddToRoleAsync(user, role);

        // Build JWT
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, role)
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(bool asAdmin = false)
    {
        var client = CreateClient();
        var jwt = await GenerateJwtForTestUserAsync(asAdmin);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

        return client;
    }
}