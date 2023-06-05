namespace MoneyPlus.Pages.Transfers;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet(int? id)
    {
        if (id == null || _context.Transfer == null)
        {
            return NotFound();
        }

        Transfer = new Transfer();
        Transfer.Date = DateTime.Now;

        Transfer.OriginWalletId = (int)id;

        var subs = from cat in _context.Category
                   join sub in _context.Subcategory.Include(c => c.Category) on cat.Id equals sub.CategoryId
                   where cat.RecordType == RecordType.Transfer && cat.IsActive && sub.IsActive
                   select new SelectListItem()
                   {
                       Value = sub.Id.ToString(),
                       Text = sub.Name
                   };

        var subcategories = subs.ToList();

        var availableWallets = _context.Wallet.Where(w => w.Id != id).ToList();

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ViewData["DestinationWalletId"] = new SelectList(availableWallets.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    [BindProperty]
    public Transfer Transfer { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Transfer.Amount < 0)
        {
            return RedirectToPage("./Create");
        }

        Transfer.Type = RecordType.Transfer;

        Wallet destinationWallet = _context.Wallet.Where(w => w.Id == Transfer.DestinationWalletId).FirstOrDefault();
        destinationWallet.Balance += Transfer.Amount;

        _context.Attach(destinationWallet).State = EntityState.Modified;

        Wallet originWallet = _context.Wallet.Where(w => w.Id == Transfer.OriginWalletId).FirstOrDefault();
        originWallet.Balance -= Transfer.Amount;

        _context.Attach(originWallet).State = EntityState.Modified;

        _context.Transfer.Add(Transfer);
        await _context.SaveChangesAsync();

        return RedirectToPage(".././Wallets/Index");
    }
}