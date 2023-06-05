namespace MoneyPlus.Pages.Transfers;

[Authorize]
public class SellModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SellModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Investment Investment { get; set; } = default!;
    [BindProperty]
    public Transfer Transfer { get; set; }
    [BindProperty]
    public double InitialAmount { get; set; }
    [BindProperty]
    public int GainsDestinationWalletId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Transfer == null)
        {
            return NotFound();
        }

        Transfer = new Transfer();
        Transfer.Date = DateTime.Now;
        Transfer.Type = RecordType.Transfer;

        var investment = await _context.Investment.FirstOrDefaultAsync(i => i.Id == id);
        Investment = investment;

        if (investment == null)
        {
            return NotFound();
        }

        InitialAmount = Investment.Amount;
        Transfer.OriginWalletId = Investment.DestinationWalletId;

        var subs = from cat in _context.Category
                   join sub in _context.Subcategory.Include(c => c.Category) on cat.Id equals sub.CategoryId
                   where cat.RecordType == RecordType.Transfer && cat.IsActive && sub.IsActive
                   select new SelectListItem()
                   {
                       Value = sub.Id.ToString(),
                       Text = sub.Name
                   };

        var subcategories = subs.ToList();

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var availableWallets = _context.Wallet.Where(w => w.UserId == user && w.Name.StartsWith("Investment - ") == false).ToList();

        ViewData["DestinationWalletId"] = new SelectList(availableWallets.Where(w => w.IsActive == true), "Id", "Name");
        ViewData["GainsDestinationWalletId"] = new SelectList(availableWallets.Where(w => w.IsActive == true), "Id", "Name");
        ViewData["SubcategoryId"] = subcategories;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Transfer.Amount < 0)
        {
            return Page();
        }

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        Wallet originWallet = _context.Wallet.Where(w => w.Id == Transfer.OriginWalletId).FirstOrDefault();

        if (originWallet.IsActive)
        {
            originWallet.Balance = 0;
            originWallet.IsActive = false;
        }
        
        _context.Attach(originWallet).State = EntityState.Modified;

        // Recebe o valor do investimento inicial se o houver para receber. 
        if (Transfer.Amount >= InitialAmount)
        {
            Transfer.Type = RecordType.Transfer;
            Transfer.Description = "Capital Invested";
            Wallet destinationWallet = _context.Wallet.Where(w => w.Id == Transfer.DestinationWalletId).FirstOrDefault();
            destinationWallet.Balance += InitialAmount;

            _context.Attach(destinationWallet).State = EntityState.Modified;

            var transfer2 = new Transfer
            {
                Description = "Capital Gains",
                Type = RecordType.Transfer,
                Amount = Transfer.Amount - InitialAmount,
                Date = DateTime.Now,
                SubcategoryId = Transfer.SubcategoryId,
                OriginWalletId = Transfer.OriginWalletId,
                DestinationWallet = _context.Wallet.Where(w => w.Id == GainsDestinationWalletId).FirstOrDefault()
            };
            transfer2.DestinationWallet.Balance += (Transfer.Amount - InitialAmount);

            _context.Transfer.Add(Transfer);
            _context.Transfer.Add(transfer2);
        }
        else
        {
            // Recebe o valor da venda.
            Transfer.Description = "Sell With Loss";
            Transfer.Type = RecordType.Transfer;
            Wallet destinationWallet = _context.Wallet.Where(w => w.Id == Transfer.DestinationWalletId).FirstOrDefault();
            destinationWallet.Balance += Transfer.Amount;

            // Financeiramente considerou-se que não faria sentido atribuir uma perda a uma wallet já que iria afetar o
            // património pessoal do user dado que as wallets representam património.
            //Wallet gainsDestinationWallet = _context.Wallet.Where(w => w.Id == GainsDestinationWalletId).FirstOrDefault();
            //gainsDestinationWallet.Balance -= InitialAmount - Transfer.Amount;

            _context.Attach(destinationWallet).State = EntityState.Modified;
            _context.Transfer.Add(Transfer);
        }

        _context.Investment.Remove(Investment);
        await _context.SaveChangesAsync();

        return RedirectToPage(".././Investments/Index");
    }
}