namespace InvestmentApp.Domain;

public class Portfolio
{
    public Guid PortfolioId { get; set; }
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public Customer Customer { get; set; } = default!;
    public ICollection<PortfolioHolding> Holdings { get; set; } = new List<PortfolioHolding>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}