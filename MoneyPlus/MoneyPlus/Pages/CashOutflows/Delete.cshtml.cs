namespace MoneyPlus.Pages.CashOutflows;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.CashOutflow == null)
        {
            return NotFound();
        }

        var cashOutflow = await _context.CashOutflow.FindAsync(id);

        if (cashOutflow != null)
        {
            CashOutflow = cashOutflow;
            _context.CashOutflow.Remove(CashOutflow);

            var wallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == CashOutflow.OriginWalletId);
            wallet.Balance += CashOutflow.Amount;

            _context.Attach(wallet).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}