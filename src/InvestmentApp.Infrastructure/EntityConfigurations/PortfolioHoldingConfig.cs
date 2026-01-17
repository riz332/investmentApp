using InvestmentApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.EntityConfigurations;

public class PortfolioHoldingConfig : IEntityTypeConfiguration<PortfolioHolding>
{
    public void Configure(EntityTypeBuilder<PortfolioHolding> builder)
    {
        builder.HasKey(h => h.HoldingId);
        builder.Property(h => h.Quantity).HasColumnType("decimal(18,6)");
        builder.Property(h => h.AveragePrice).HasColumnType("decimal(18,4)");
        builder.Property(h => h.CreatedAt).HasDefaultValueSql("NOW()");

        builder.HasOne(h => h.Portfolio)
               .WithMany(p => p.Holdings)
               .HasForeignKey(h => h.PortfolioId);

        builder.HasOne(h => h.Asset)
               .WithMany(a => a.Holdings)
               .HasForeignKey(h => h.AssetId);
    }
}