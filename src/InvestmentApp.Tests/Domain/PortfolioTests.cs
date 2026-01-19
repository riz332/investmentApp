using InvestmentApp.Domain;
using FluentAssertions;

public class PortfolioTests
{
    [Fact]
    public void AddingTransaction_UpdatesHoldingsCorrectly()
    {
        var portfolio = new Portfolio();

        portfolio.ApplyTransaction("AAPL", 10, 150m, TransactionType.Buy); // buy 10 @ 150

        portfolio.Holdings.First().Quantity.Should().Be(10);        
    }
}