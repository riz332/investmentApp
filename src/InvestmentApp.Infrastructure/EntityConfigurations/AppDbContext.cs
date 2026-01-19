using InvestmentApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<PortfolioHolding> PortfolioHoldings => Set<PortfolioHolding>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<AssetPrice> AssetPrices => Set<AssetPrice>();

    public DbSet<TransactionTypeLookup> TransactionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(b =>
        {
            b.HasOne(t => t.TransactionTypeLookup)
             .WithMany(tt => tt.Transactions)
             .HasForeignKey(t => t.TransactionType)     // uses the int property named TransactionType
             .HasPrincipalKey(tt => tt.TransactionTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(t => t.TransactionType)
             .IsRequired()
             .HasColumnType("integer")
             .HasColumnName("TransactionType"); // keeps the DB column name as requested
        });

        modelBuilder.Entity<TransactionTypeLookup>(b =>
        {
            b.HasKey(tt => tt.TransactionTypeId);

            // Ensure the PK is not created as an identity/auto-generated value
            b.Property(tt => tt.TransactionTypeId)
             .HasColumnType("integer")
             .ValueGeneratedNever();

            b.Property(tt => tt.Name)
             .IsRequired()
             .HasMaxLength(50)
             .HasColumnType("character varying(50)");

            b.ToTable("TransactionTypes");
        });
    }
}