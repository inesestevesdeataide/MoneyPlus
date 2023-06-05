namespace MoneyPlus.Pages.CashOutflows;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly MoneyPlus.Data.ApplicationDbContext _context;

    public DetailsModel(MoneyPlus.Data.ApplicationDbContext context)
    {
        _context = context;
    }

  public CashOutflow CashOutflow { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.CashOutflow == null)
        {
            return NotFound();
        }

        var cashOutflow = await _context.CashOutflow.FirstOrDefaultAsync(c => c.Id == id);
        cashOutflow.Payee = await _context.Payee.FirstOrDefaultAsync(p => p.Id == cashOutflow.PayeeId);
        cashOutflow.OriginWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == cashOutflow.OriginWalletId);
        cashOutflow.Subcategory = await _context.Subcategory.FirstOrDefaultAsync(s => s.Id == cashOutflow.SubcategoryId);
        cashOutflow.Asset = await _context.Asset.FirstOrDefaultAsync(a => a.Id == cashOutflow.AssetId);

        if (cashOutflow == null)
        {
            return NotFound();
        }
        else 
        {
            CashOutflow = cashOutflow;
        }
        return Page();
    }
}