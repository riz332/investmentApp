using InvestmentApp.Domain;
using InvestmentApp.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
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
    }
}