using InvestmentApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.EntityConfigurations;

public class AssetPriceConfig : IEntityTypeConfiguration<AssetPrice>
{
    public void Configure(EntityTypeBuilder<AssetPrice> builder)
    {
        builder.HasKey(p => p.PriceId);
        builder.Property(p => p.Price).HasColumnType("decimal(18,4)");
        builder.Property(p => p.PriceDate).IsRequired();

        builder.HasOne(p => p.Asset)
               .WithMany(a => a.Prices)
               .HasForeignKey(p => p.AssetId);
    }
}