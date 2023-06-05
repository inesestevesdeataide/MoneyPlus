namespace MoneyPlus.Pages.CashOutflows;

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
                     where cat.RecordType == RecordType.CashOutflow && cat.IsActive && sub.IsActive
                     select new SelectListItem()
                     {
                         Value = sub.Id.ToString(),
                         Text = sub.Name
                     };

        var subcategories = subs.ToList();

        var activeAssets = _context.Asset.Where(a => a.IsActive == true).ToList();

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewData["AssetId"] = new SelectList(activeAssets.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["OriginWalletId"] = new SelectList(_context.Wallet.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["PayeeId"] = new SelectList(_context.Payee.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    [BindProperty]
    public CashOutflow CashOutflow { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || CashOutflow.Amount < 0)
        {
            return RedirectToPage("./Create");
        }

        CashOutflow.Type = RecordType.CashOutflow;
        Wallet originWallet = _context.Wallet.Where(w => w.Id == CashOutflow.OriginWalletId).FirstOrDefault();
        originWallet.Balance -= CashOutflow.Amount;

        _context.CashOutflow.Add(CashOutflow);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}