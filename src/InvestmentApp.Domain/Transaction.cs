namespace InvestmentApp.Domain;

public class Transaction
{
    public Guid TransactionId { get; set; }
    public Guid PortfolioId { get; set; }
    public Guid AssetId { get; set; }
    public string TransactionType { get; set; } = default!; // "Buy" / "Sell"
    public decimal Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime ExecutedAt { get; set; }

    public Portfolio Portfolio { get; set; } = default!;
    public Asset Asset { get; set; } = default!;
}