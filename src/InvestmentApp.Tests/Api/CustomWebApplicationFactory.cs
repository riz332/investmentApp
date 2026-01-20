using InvestmentApp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentApp.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _inMemoryDbName;

    // Parameterless ctor required by xUnit when it creates fixtures via reflection
    public CustomWebApplicationFactory()
    {
        _inMemoryDbName = Guid.NewGuid().ToString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Provide JWT settings via in-memory configuration (simulates user-secrets / env)
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
            // Remove existing AppDbContext registration(s) so we can replace with InMemory
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext))
                .ToList();

            foreach (var d in descriptors) services.Remove(d);

            // Build an isolated EF service provider for the InMemory provider to avoid provider conflicts
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Register AppDbContext using InMemory and the isolated EF service provider
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_inMemoryDbName);
                options.UseInternalServiceProvider(efServiceProvider);
            });

            // Build the service provider and ensure DB is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}