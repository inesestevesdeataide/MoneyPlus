namespace MoneyPlus.Pages.Transfers;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Transfer> Transfer { get;set; } = default!;

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (_context.Transfer != null)
        {
            Transfer = await _context.Transfer
            .Include(t => t.DestinationWallet)
            .Include(t => t.OriginWallet)
            .Include(t => t.Subcategory).Where(t => t.OriginWallet.UserId == user)
            .OrderByDescending(t => t.Date).ToListAsync();
        }
    }
}