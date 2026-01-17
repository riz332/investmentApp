namespace InvestmentApp.Domain;

public class Asset
{
    public Guid AssetId { get; set; }
    public string Symbol { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string AssetType { get; set; } = default!;
    public string Currency { get; set; } = "USD";

    public ICollection<PortfolioHolding> Holdings { get; set; } = new List<PortfolioHolding>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<AssetPrice> Prices { get; set; } = new List<AssetPrice>();
}