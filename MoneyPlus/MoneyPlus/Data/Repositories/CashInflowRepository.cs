namespace MoneyPlus.Data.Repositories;

public class CashInflowRepository
{
    private readonly ApplicationDbContext _context;

    public CashInflowRepository(ApplicationDbContext context)
    {
        _context = context;
    }
}