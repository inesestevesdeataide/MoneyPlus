namespace MoneyPlus.Pages.Wallets;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly WalletRepository _repository;

    public DetailsModel(WalletRepository repository)
    {
        _repository = repository;
    }

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
}