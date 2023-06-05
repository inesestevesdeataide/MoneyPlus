namespace MoneyPlus.Pages.CashInflows;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

  public CashInflow CashInflow { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.CashInflow == null)
        {
            return NotFound();
        }

        var cashinflow = await _context.CashInflow.FirstOrDefaultAsync(c => c.Id == id);
        cashinflow.DestinationWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == cashinflow.DestinationWalletId);
        cashinflow.Subcategory = await _context.Subcategory.FirstOrDefaultAsync(s => s.Id == cashinflow.SubcategoryId);
        cashinflow.Asset = await _context.Asset.FirstOrDefaultAsync(a => a.Id == cashinflow.AssetId);

        if (cashinflow == null)
        {
            return NotFound();
        }
        else 
        {
            CashInflow = cashinflow;
        }
        return Page();
    }
}