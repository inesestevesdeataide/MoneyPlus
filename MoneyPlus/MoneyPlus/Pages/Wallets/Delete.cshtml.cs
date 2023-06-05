namespace MoneyPlus.Pages.Wallets;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly WalletRepository _repository;

    public DeleteModel(WalletRepository repository)
    {
        _repository = repository;
    }

    [BindProperty]
    public Wallet Wallet { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _repository == null)
        {
            return NotFound();
        }

        var wallet = await _repository.GetWalletByIdAsync((int)id);

        if (wallet == null)
        {
            return NotFound();
        }
        else 
        {
            Wallet = wallet;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _repository == null)
        {
            return NotFound();
        }
        var wallet = await _repository.GetWalletByIdAsync((int)id);

        if (wallet != null)
        {
            await _repository.DeleteWallet(wallet);
        }

        return RedirectToPage("./Index");
    }
}