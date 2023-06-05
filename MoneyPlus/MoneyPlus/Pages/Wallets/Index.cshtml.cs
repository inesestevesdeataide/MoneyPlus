namespace MoneyPlus.Pages.Wallets;

[Authorize]
public class IndexModel : PageModel
{
    private readonly WalletRepository _repository;

    public IndexModel(WalletRepository repository)
    {
        _repository = repository;
    }

    public IList<Wallet> Wallet { get;set; } = default!;

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (_repository != null)
        {
            Wallet = await _repository.GetWalletsByUserAsync(user);
        }
    }
}