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

    public void ApplyTransaction(string symbol, decimal quantity, decimal price, TransactionType type)
    {
        // Basic implementation used by tests:
        // - If buying, increase or add a holding and set average price
        // - If selling, decrease quantity (no negative checks for simplicity)

        // Find existing holding by asset symbol via the Asset navigation if present
        var holding = Holdings.FirstOrDefault(h => h.Asset != null && h.Asset.Symbol == symbol);

        if (type == TransactionType.Buy)
        {
            if (holding == null)
            {
                holding = new PortfolioHolding
                {
                    HoldingId = Guid.NewGuid(),
                    PortfolioId = this.PortfolioId,
                    Quantity = quantity,
                    AveragePrice = price,
                    CreatedAt = DateTime.UtcNow,
                    Portfolio = this
                };

                Holdings.Add(holding);
            }
            else
            {
                // update average price
                var totalCost = holding.AveragePrice * holding.Quantity + price * quantity;
                holding.Quantity += quantity;
                holding.AveragePrice = holding.Quantity == 0 ? 0 : totalCost / holding.Quantity;
            }
        }
        else if (type == TransactionType.Sell)
        {
            if (holding != null)
            {
                holding.Quantity -= quantity;
                if (holding.Quantity < 0) holding.Quantity = 0;
            }
        }

        // Record a transaction entry (minimal fields)
        var transaction = new Transaction
        {
            TransactionId = Guid.NewGuid(),
            PortfolioId = this.PortfolioId,
            Quantity = quantity,
            PricePerUnit = price,
            TotalAmount = quantity * price,
            ExecutedAt = DateTime.UtcNow,
            TransactionType = (int)type
        };

        Transactions.Add(transaction);
    }
}