namespace MoneyPlus.Pages.Investments;

[Authorize]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Investment Investment { get; set; } = default!;
    [BindProperty]
    public double InitialAmount { get; set; }
    [BindProperty]
    public int InitialOriginWalletId { get; set; }
    [BindProperty]
    public string WalletName { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Investment == null)
        {
            return NotFound();
        }

        var subs = from cat in _context.Category
                   join sub in _context.Subcategory.Include(s => s.Category) on cat.Id equals sub.CategoryId
                   where cat.RecordType == RecordType.Investment && cat.IsActive && sub.IsActive
                   select new SelectListItem()
                   {
                       Value = sub.Id.ToString(),
                       Text = sub.Name
                   };

        var subcategories = subs.ToList();

        var investment = await _context.Investment.FirstOrDefaultAsync(i => i.Id == id);

        InitialOriginWalletId = investment.OriginWalletId;
        InitialAmount = investment.Amount;

        var destinationWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == investment.DestinationWalletId);
        WalletName = destinationWallet.Name;

        Investment = investment;

        if (investment == null)
        {
            return NotFound();
        }

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewData["DestinationWalletId"] = new SelectList(_context.Wallet.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["OriginWalletId"] = new SelectList(_context.Wallet.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["PayeeId"] = new SelectList(_context.Payee.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!(Investment.PayeeId > 0 && Investment.OriginWalletId > 0 && WalletName != "" && Investment.Amount > 0 && Investment.Date != null && Investment.SubcategoryId > 0))
        {
            return RedirectToPage("./Edit");
        }

        _context.Attach(Investment).State = EntityState.Modified;

        var destinationWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == Investment.DestinationWalletId);

        var initialOriginWallet= await _context.Wallet.FirstOrDefaultAsync(w => w.Id == InitialOriginWalletId);

        if (Investment.OriginWalletId != InitialOriginWalletId || Investment.Amount != InitialAmount)
        {
            initialOriginWallet.Balance += InitialAmount;

            if (Investment.OriginWalletId != InitialOriginWalletId)
            {
                var finalOriginWallet = await _context.Wallet.FirstOrDefaultAsync(w => w.Id == Investment.OriginWalletId);
                finalOriginWallet.Balance -= Investment.Amount;

                _context.Attach(finalOriginWallet).State = EntityState.Modified;
            }
            else
            {
                initialOriginWallet.Balance -= Investment.Amount;
            }

            _context.Attach(initialOriginWallet).State = EntityState.Modified;

            destinationWallet.Balance -= InitialAmount;
            destinationWallet.Balance += Investment.Amount;
        }
        
        destinationWallet.Name = WalletName;

        _context.Attach(destinationWallet).State = EntityState.Modified;

        _context.Attach(Investment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InvestmentExists(Investment.Id))
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

    private bool InvestmentExists(int id)
    {
      return _context.Investment.Any(e => e.Id == id);
    }
}