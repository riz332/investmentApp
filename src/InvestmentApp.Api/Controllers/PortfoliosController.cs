using InvestmentApp.Api.DTOs;
using InvestmentApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace InvestmentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public PortfoliosController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // GET: api/portfolios/customer/{customerId}
    [HttpGet("customer/{customerId:guid}")]
    public async Task<ActionResult<IEnumerable<PortfolioResponse>>> GetCustomerPortfolios(Guid customerId)
    {
        var portfolios = await _db.Portfolios
            .Include(p => p.Holdings).ThenInclude(h => h.Asset)
            .Include(p => p.Transactions).ThenInclude(t => t.Asset)
            .Where(p => p.CustomerId == customerId)
            .ToListAsync();

        var responses = portfolios.Select(p => _mapper.Map<PortfolioResponse>(p));
        return Ok(responses);
    }

    // GET: api/portfolios/{portfolioId}
    [HttpGet("{portfolioId:guid}")]
    public async Task<ActionResult<PortfolioResponse>> GetPortfolio(Guid portfolioId)
    {
        var portfolio = await _db.Portfolios
            .Include(p => p.Holdings).ThenInclude(h => h.Asset)
            .Include(p => p.Transactions).ThenInclude(t => t.Asset)
            .FirstOrDefaultAsync(p => p.PortfolioId == portfolioId);

        if (portfolio == null)
            return NotFound();

        return Ok(_mapper.Map<PortfolioResponse>(portfolio));
    }

    // POST: api/portfolios
    [HttpPost]
    public async Task<ActionResult<PortfolioResponse>> CreatePortfolio(CreatePortfolioRequest request)
    {
        var customer = await _db.Customers
            .Include(c => c.Portfolios)
            .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId);

        if (customer == null)
            return NotFound("Customer not found");

        var portfolio = customer.CreatePortfolio(request.Name, request.Description);

        _db.Portfolios.Add(portfolio);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPortfolio), new { portfolioId = portfolio.PortfolioId },
            _mapper.Map<PortfolioResponse>(portfolio));
    }

    // POST: api/portfolios/{portfolioId}/transactions
    [HttpPost("{portfolioId:guid}/transactions")]
    public async Task<ActionResult<TransactionResponse>> AddTransaction(Guid portfolioId, TransactionRequest request)
    {
        var portfolio = await _db.Portfolios
            .Include(p => p.Holdings)
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.PortfolioId == portfolioId);

        if (portfolio == null)
            return NotFound("Portfolio not found");

        var asset = await _db.Assets.FirstOrDefaultAsync(a => a.AssetId == request.AssetId);
        if (asset == null)
            return NotFound("Asset not found");

        portfolio.ApplyTransaction(
            symbol: asset.Symbol,
            quantity: request.Quantity,
            price: request.PricePerUnit,
            type: request.Type
        );

        await _db.SaveChangesAsync();

        var transaction = portfolio.Transactions.Last();
        return Ok(_mapper.Map<TransactionResponse>(transaction));
    }

    // GET: api/portfolios/{portfolioId}/performance
    [HttpGet("{portfolioId:guid}/performance")]
    public async Task<ActionResult<PerformanceResponse>> GetPerformance(Guid portfolioId)
    {
        var portfolio = await _db.Portfolios
            .Include(p => p.Holdings).ThenInclude(h => h.Asset).ThenInclude(a => a.Prices)
            .FirstOrDefaultAsync(p => p.PortfolioId == portfolioId);

        if (portfolio == null)
            return NotFound();

        var response = _mapper.Map<PerformanceResponse>(portfolio);
        return Ok(response);
    }
}