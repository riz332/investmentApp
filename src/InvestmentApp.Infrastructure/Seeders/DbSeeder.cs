using InvestmentApp.Domain;
using InvestmentApp.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration config)
    {
        // Seed transaction types (static lookup)
        if (!db.TransactionTypes.Any())
        {
            db.TransactionTypes.AddRange(
                new TransactionTypeLookup { TransactionTypeId = 0, Name = "Buy" },
                new TransactionTypeLookup { TransactionTypeId = 1, Name = "Sell" }
            );
            await db.SaveChangesAsync();
        }

        if (!db.Customers.Any())
        {
            var customer = new Customer
            {
                CustomerId = Guid.NewGuid(),
                FirstName = "Rizwan",
                LastName = "Ahmed",
                Email = "rizwan@example.com",
                CreatedAt = DateTime.UtcNow,
                Portfolios = new List<Portfolio>
                {
                    new Portfolio
                    {
                        PortfolioId = Guid.NewGuid(),
                        Name = "Main Portfolio",
                        CreatedAt = DateTime.UtcNow,
                    }
                }
            };

            db.Customers.Add(customer);
            await db.SaveChangesAsync();
        }

        await SeedAdminUser(userManager, roleManager, config);
    }// appsettings.json (or appsettings.Development.json)

    private static async Task SeedAdminUser(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration config)
    {
        var adminEmail = config["SeedAdmin:Email"] ?? "admin@investmentapp.local";
        var adminPassword = config["SeedAdmin:Password"] ?? "Admin123!";
        var adminRole = config["SeedAdmin:Role"] ?? "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
        }

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new Exception("Admin email or password is not configured properly in appsettings.");
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                MustChangePassword = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );
            }
        }
        else if (!adminUser.MustChangePassword)
        {
            // Optional: enforce reset again if reseeded
            adminUser.MustChangePassword = true;
            await userManager.UpdateAsync(adminUser);
        }


        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}