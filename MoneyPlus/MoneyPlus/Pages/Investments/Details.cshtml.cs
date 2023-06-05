namespace MoneyPlus.Pages.Investments;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

  public Investment Investment { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {

        if (id == null || _context.Investment == null)
        {
            return NotFound();
        }

        var investment = await _context.Investment.FirstOrDefaultAsync(m => m.Id == id);
        investment.Payee = await _context.Payee.FirstOrDefaultAsync(p => p.Id == investment.PayeeId);
        investment.OriginWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == investment.OriginWalletId);
        investment.DestinationWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == investment.DestinationWalletId);
        investment.Subcategory = await _context.Subcategory.FirstOrDefaultAsync(s => s.Id == investment.SubcategoryId);

        if (investment == null)
        {
            return NotFound();
        }
        else 
        {
            Investment = investment;
        }
        return Page();
    }
}