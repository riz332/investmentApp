using InvestmentApp.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
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
        // Ensure Identity model is configured first
        base.OnModelCreating(modelBuilder);

        // Apply the project's EF configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

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

        // Optional: configure relationship between ApplicationUser and Customer if you want cascade/constraints
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.HasOne(u => u.Customer)
             .WithOne()
             .HasForeignKey<ApplicationUser>(u => u.CustomerId)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }
}