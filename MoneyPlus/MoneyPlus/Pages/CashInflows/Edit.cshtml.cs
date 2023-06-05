namespace MoneyPlus.Pages.CashInflows;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public CashInflow CashInflow { get; set; } = default!;
    [BindProperty]
    public double InitialAmount { get; set; }
    [BindProperty]
    public int InitialDestinationWalletId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.CashInflow == null)
        {
            return NotFound();
        }

        var subs = from cat in _context.Category
                     join sub in _context.Subcategory.Include(c => c.Category) on cat.Id equals sub.CategoryId
                     where cat.RecordType == RecordType.CashInflow && cat.IsActive && sub.IsActive
                     select new SelectListItem()
                     {
                         Value = sub.Id.ToString(),
                         Text = sub.Name
                     };

        var subcategories = subs.ToList();

        var activeAssets = _context.Asset.Where(a => a.IsActive == true).ToList();

        var cashInflow =  await _context.CashInflow.FirstOrDefaultAsync(c => c.Id == id);
        InitialDestinationWalletId = cashInflow.DestinationWalletId;
        InitialAmount = cashInflow.Amount;

        CashInflow = cashInflow;

        if (cashInflow == null)
        {
            return NotFound();
        }

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewData["AssetId"] = new SelectList(activeAssets.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["DestinationWalletId"] = new SelectList(_context.Wallet.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || CashInflow.Amount < 0)
        {
            return RedirectToPage("./Edit");
        }

        _context.Attach(CashInflow).State = EntityState.Modified;

        if (CashInflow.DestinationWalletId != InitialDestinationWalletId)
        {
            var intialWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == InitialDestinationWalletId);
            intialWallet.Balance -= InitialAmount;

            _context.Attach(intialWallet).State = EntityState.Modified;

            var finalWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == CashInflow.DestinationWalletId);
            finalWallet.Balance += CashInflow.Amount;

            _context.Attach(intialWallet).State = EntityState.Modified;
        }
        else if (CashInflow.Amount != InitialAmount)
        {
            var wallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == CashInflow.DestinationWalletId);
            wallet.Balance -= InitialAmount;
            wallet.Balance += CashInflow.Amount;

            _context.Attach(CashInflow).State = EntityState.Modified;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CashInflowExists(CashInflow.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool CashInflowExists(int id)
    {
      return _context.CashInflow.Any(e => e.Id == id);
    }
}