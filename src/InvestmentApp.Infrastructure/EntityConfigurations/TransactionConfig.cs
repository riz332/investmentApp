using InvestmentApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.EntityConfigurations;

public class TransactionConfig : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.TransactionId);
        builder.Property(t => t.TransactionType).HasMaxLength(10).IsRequired();
        builder.Property(t => t.Quantity).HasColumnType("decimal(18,6)");
        builder.Property(t => t.PricePerUnit).HasColumnType("decimal(18,4)");
        builder.Property(t => t.TotalAmount).HasColumnType("decimal(18,4)");
        builder.Property(t => t.ExecutedAt).IsRequired();

        builder.HasOne(t => t.Portfolio)
               .WithMany(p => p.Transactions)
               .HasForeignKey(t => t.PortfolioId);

        builder.HasOne(t => t.Asset)
               .WithMany(a => a.Transactions)
               .HasForeignKey(t => t.AssetId);
    }
}