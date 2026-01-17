namespace InvestmentApp.Domain;

public class PortfolioHolding
{
    public Guid HoldingId { get; set; }
    public Guid PortfolioId { get; set; }
    public Guid AssetId { get; set; }
    public decimal Quantity { get; set; }
    public decimal AveragePrice { get; set; }
    public DateTime CreatedAt { get; set; }

    public Portfolio Portfolio { get; set; } = default!;
    public Asset Asset { get; set; } = default!;
}