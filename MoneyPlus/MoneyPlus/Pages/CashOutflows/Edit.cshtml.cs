namespace MoneyPlus.Pages.CashOutflows;

[Authorize]
public class EditModel : PageModel
{
    private readonly MoneyPlus.Data.ApplicationDbContext _context;

    public EditModel(MoneyPlus.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public CashOutflow CashOutflow { get; set; } = default!;
    [BindProperty]
    public double InitialAmount { get; set; }
    [BindProperty]
    public int InitialOriginWalletId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.CashOutflow == null)
        {
            return NotFound();
        }

        var subs = from cat in _context.Category
                     join sub in _context.Subcategory.Include(c => c.Category) on cat.Id equals sub.CategoryId
                     where cat.RecordType == RecordType.CashOutflow && cat.IsActive && sub.IsActive
                     select new SelectListItem()
                     {
                         Value = sub.Id.ToString(),
                         Text = sub.Name
                     };

        var subcategories = subs.ToList();

        var activeAssets = _context.Asset.Where(a => a.IsActive == true).ToList();

        var cashOutflow =  await _context.CashOutflow.FirstOrDefaultAsync(m => m.Id == id);

        InitialOriginWalletId = cashOutflow.OriginWalletId;
        InitialAmount = cashOutflow.Amount;

        CashOutflow = cashOutflow;

        if (cashOutflow == null)
        {
            return NotFound();
        }
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewData["AssetId"] = new SelectList(activeAssets.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["OriginWalletId"] = new SelectList(_context.Wallet.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["PayeeId"] = new SelectList(_context.Payee.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || CashOutflow.Amount < 0)
        {
            return RedirectToPage("./Create");
        }

        _context.Attach(CashOutflow).State = EntityState.Modified;

        if (CashOutflow.OriginWalletId != InitialOriginWalletId)
        {
            var intialWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == InitialOriginWalletId);
            intialWallet.Balance += InitialAmount;

            _context.Attach(intialWallet).State = EntityState.Modified;

            var finalWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == CashOutflow.OriginWalletId);
            finalWallet.Balance -= CashOutflow.Amount;

            _context.Attach(intialWallet).State = EntityState.Modified;
        }
        else if (CashOutflow.Amount != InitialAmount)
        {
            var wallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == CashOutflow.OriginWalletId);
            wallet.Balance += InitialAmount;
            wallet.Balance -= CashOutflow.Amount;

            _context.Attach(CashOutflow).State = EntityState.Modified;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CashOutflowExists(CashOutflow.Id))
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

    private bool CashOutflowExists(int id)
    {
      return _context.CashOutflow.Any(e => e.Id == id);
    }
}