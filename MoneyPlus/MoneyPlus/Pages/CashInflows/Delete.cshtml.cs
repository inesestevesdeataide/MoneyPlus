namespace MoneyPlus.Pages.CashInflows;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public CashInflow CashInflow { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.CashInflow == null)
        {
            return NotFound();
        }

        var cashInflow = await _context.CashInflow.FirstOrDefaultAsync(c => c.Id == id);
        cashInflow.DestinationWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == cashInflow.DestinationWalletId);
        cashInflow.Subcategory = await _context.Subcategory.FirstOrDefaultAsync(s => s.Id == cashInflow.SubcategoryId);
        cashInflow.Asset = await _context.Asset.FirstOrDefaultAsync(a => a.Id == cashInflow.AssetId);

        if (cashInflow == null)
        {
            return NotFound();
        }
        else 
        {
            CashInflow = cashInflow;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.CashInflow == null)
        {
            return NotFound();
        }

        var cashInflow = await _context.CashInflow.FindAsync(id);

        if (cashInflow != null)
        {
            CashInflow = cashInflow;
            _context.CashInflow.Remove(CashInflow);

            var wallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == CashInflow.DestinationWalletId);
            wallet.Balance -= CashInflow.Amount;

            _context.Attach(wallet).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}