namespace MoneyPlus.Pages.Wallets;

[Authorize]
public class EditModel : PageModel
{
    private readonly WalletRepository _repository;
    private readonly ILogger<EditModel> _logger;

    public EditModel(WalletRepository repository, ILogger<EditModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [BindProperty]
    public Wallet Wallet { get; set; } = default!;

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

        Wallet = wallet;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var hasDuplicates = _repository.WalletIsDuplicated(Wallet, Wallet.UserId);

        if (!ModelState.IsValid || hasDuplicates)
        {
            _logger.LogError($"User {Wallet.UserId} inserted invalid fields when editing Wallet.");
            return Page();
        }

        try
        {
            await _repository.UpdateWalletAsync(Wallet);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_repository.WalletExists(Wallet.Id))
            {
                _logger.LogCritical($"Not found by user {Wallet.UserId}");
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToPage("./Index");
    }
}