using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InvestmentApp.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configBuilder = new ConfigurationBuilder();

        // Prefer the API project's appsettings if it exists (common layout: ../InvestmentApp.Api/)
        var apiSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "InvestmentApp.Api", $"appsettings.{env}.json");
        var apiSettingsFallback = Path.Combine(Directory.GetCurrentDirectory(), "..", "InvestmentApp.Api", "appsettings.json");

        if (File.Exists(apiSettingsPath) || File.Exists(apiSettingsFallback))
        {
            // Use the API project's folder as base
            var apiFolder = Path.GetDirectoryName(apiSettingsPath) ?? Path.GetDirectoryName(apiSettingsFallback)!;
            configBuilder.SetBasePath(apiFolder)
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                         .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);
        }
        else
        {
            // Fallback to the current directory
            configBuilder.SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                         .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);
        }

        configBuilder.AddEnvironmentVariables();
        var configuration = configBuilder.Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
                             ?? Environment.GetEnvironmentVariable("DefaultConnection")
                             ?? configuration["ConnectionStrings:DefaultConnection"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'. Set it in appsettings or via the environment variable 'DefaultConnection'.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}