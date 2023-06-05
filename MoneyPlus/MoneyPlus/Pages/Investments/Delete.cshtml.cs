using MoneyPlus.Data.Entities;

namespace MoneyPlus.Pages.Investments;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Investment Investment { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Investment == null)
        {
            return NotFound();
        }

        var investment = await _context.Investment.FirstOrDefaultAsync(i => i.Id == id);
        investment.Payee = await _context.Payee.FirstOrDefaultAsync(p => p.Id == id);
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Investment == null)
        {
            return NotFound();
        }
        var investment = await _context.Investment.FindAsync(id);

        if (investment != null)
        {
            Investment = investment;
            _context.Investment.Remove(Investment);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}