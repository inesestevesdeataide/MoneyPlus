namespace MoneyPlus.Data.Repositories;

public class InvestmentRepository
{
    private readonly ApplicationDbContext _context;

    public InvestmentRepository(ApplicationDbContext context)
	{
        _context = context;
    }

    public async Task<List<Investment>> GetInvestmentsByUserAsync(string user)
    {
        var investments = await _context.Investment
        .Include(i => i.DestinationWallet)
        .Include(i => i.OriginWallet)
        .Include(i => i.Payee)
        .Include(i => i.Subcategory).Where(c => c.OriginWallet.UserId == user).ToListAsync();

        investments.Sort((p1, p2) => p2.Date.CompareTo(p1.Date));

        return investments;
    }        
}