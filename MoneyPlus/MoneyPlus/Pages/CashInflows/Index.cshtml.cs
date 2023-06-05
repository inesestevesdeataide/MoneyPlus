namespace MoneyPlus.Pages.CashInflows;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<CashInflow> CashInflow { get;set; } = default!;

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (_context.CashInflow != null)
        {
            var cashInflows = await _context.CashInflow
            .Include(c => c.Asset)
            .Include(c => c.DestinationWallet)
            .Include(c => c.Subcategory).Where(c => c.DestinationWallet.UserId == user).ToListAsync();

            cashInflows.Sort((d1, d2) => (d2.Date).CompareTo(d1.Date));

            CashInflow = cashInflows;
        }
    }
}