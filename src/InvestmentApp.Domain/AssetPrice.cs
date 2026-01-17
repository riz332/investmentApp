namespace InvestmentApp.Domain;

public class AssetPrice
{
    public Guid PriceId { get; set; }
    public Guid AssetId { get; set; }
    public decimal Price { get; set; }
    public DateTime PriceDate { get; set; }

    public Asset Asset { get; set; } = default!;
}