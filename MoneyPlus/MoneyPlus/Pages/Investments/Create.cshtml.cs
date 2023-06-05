namespace MoneyPlus.Pages.Investments;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        var subs = from cat in _context.Category
                   join sub in _context.Subcategory.Include(c => c.Category) on cat.Id equals sub.CategoryId
                   where cat.RecordType == RecordType.Investment && cat.IsActive && sub.IsActive
                   select new SelectListItem()
                   {
                       Value = sub.Id.ToString(),
                       Text = sub.Name
                   };

        var subcategories = subs.ToList();

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewData["OriginWalletId"] = new SelectList(_context.Wallet.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["PayeeId"] = new SelectList(_context.Payee.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    [BindProperty]
    public Investment Investment { get; set; }
    [BindProperty]
    public Wallet Wallet { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        Investment.Type = RecordType.Investment;

        if (!(Investment.PayeeId > 0 && Investment.OriginWalletId > 0 && Wallet.Name != "" && Investment.Amount > 0 && Investment.Date != null && Investment.SubcategoryId > 0))
        {
            return RedirectToPage("./Create");
        }

        Wallet.Name = "Investment - " + Wallet.Name;
        Wallet.Balance = Investment.Amount;
        Wallet.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Wallet.IsActive = true;

        _context.Wallet.Add(Wallet);
        await _context.SaveChangesAsync();

        Investment.DestinationWalletId = Wallet.Id;

        Wallet originWallet = _context.Wallet.Where(w => w.Id == Investment.OriginWalletId).FirstOrDefault();
        originWallet.Balance -= Investment.Amount;

        _context.Attach(originWallet).State = EntityState.Modified;

        _context.Investment.Add(Investment);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}