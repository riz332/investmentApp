namespace InvestmentApp.Domain;

public class Customer
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

    public Portfolio CreatePortfolio(string name, string? description)
    {
        var portfolio = new Portfolio
        {
            PortfolioId = Guid.NewGuid(),
            CustomerId = this.CustomerId,
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            Customer = this
        };

        Portfolios.Add(portfolio);
        return portfolio;
    }
}