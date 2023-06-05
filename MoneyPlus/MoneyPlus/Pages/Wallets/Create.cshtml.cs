namespace MoneyPlus.Pages.Wallets;

[Authorize]
public class CreateModel : PageModel
{
    private readonly WalletRepository _repository;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(WalletRepository repository, ILogger<CreateModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Wallet Wallet { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        Wallet.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var hasDuplicates = _repository.WalletIsDuplicated(Wallet, Wallet.UserId);

        if (!ModelState.IsValid || hasDuplicates)
        {
            _logger.LogError($"User {Wallet.UserId} inserted invalid fields when creating Wallet.");
            return Page();
        }

        await _repository.CreateAsync(Wallet);

        return RedirectToPage("./Index");
    }
}