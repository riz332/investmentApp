using InvestmentApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.EntityConfigurations;

public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(a => a.AssetId);
        builder.Property(a => a.Symbol).HasMaxLength(20).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(255).IsRequired();
        builder.Property(a => a.AssetType).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Currency).HasMaxLength(10).IsRequired();
        builder.HasIndex(a => a.Symbol);
    }
}