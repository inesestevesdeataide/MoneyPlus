namespace MoneyPlus.Pages.CashInflows;

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
                     where cat.RecordType == RecordType.CashInflow && cat.IsActive && sub.IsActive
                     select new SelectListItem()
                     {
                         Value = sub.Id.ToString(),
                         Text = sub.Name
                     };

        var subcategories = subs.ToList();

        var activeAssets = _context.Asset.Where(a => a.IsActive == true).ToList();

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewData["AssetId"] = new SelectList(activeAssets.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["DestinationWalletId"] = new SelectList(_context.Wallet.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        // Porque quero apresentar subcategorias filtradas
        //ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    [BindProperty]
    public CashInflow CashInflow { get; set; }
    
    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || CashInflow.Amount < 0)
        {
            return RedirectToPage("./Create");
        }

        CashInflow.Type = RecordType.CashInflow;
        Wallet destinationWallet = _context.Wallet.Where(w => w.Id == CashInflow.DestinationWalletId).FirstOrDefault();
        destinationWallet.Balance += CashInflow.Amount;

        _context.CashInflow.Add(CashInflow);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}