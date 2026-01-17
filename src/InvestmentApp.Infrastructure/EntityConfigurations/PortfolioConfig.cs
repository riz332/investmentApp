using InvestmentApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.EntityConfigurations;

public class PortfolioConfig : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.HasKey(p => p.PortfolioId);
        builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("NOW()");

        builder.HasOne(p => p.Customer)
               .WithMany(c => c.Portfolios)
               .HasForeignKey(p => p.CustomerId);
    }
}