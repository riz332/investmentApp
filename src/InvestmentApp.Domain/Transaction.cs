namespace InvestmentApp.Domain;

public class Transaction
{
    public Guid TransactionId { get; set; }
    public Guid PortfolioId { get; set; }
    public Guid AssetId { get; set; }

    // FK column (integer) that points to TransactionTypes.TransactionTypeId
    public int TransactionType { get; set; }

    public decimal Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime ExecutedAt { get; set; }

    public Portfolio Portfolio { get; set; } = default!;
    public Asset Asset { get; set; } = default!;

    // Navigation to the lookup table
    public TransactionTypeLookup TransactionTypeLookup { get; set; } = default!;
}