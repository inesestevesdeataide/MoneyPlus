namespace MoneyPlus.Pages.Assets;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AssetRepository _repository;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(AssetRepository repository, ILogger<CreateModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Asset Asset { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        Asset.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var hasDuplicates = _repository.AssetIsDuplicated(Asset, Asset.UserId);

        if (!ModelState.IsValid || hasDuplicates)
        {
            _logger.LogError($"User {Asset.UserId} inserted invalid fields when creating Asset.");
            return Page();
        }

        await _repository.CreateAsync(Asset);

        return RedirectToPage("./Index");
    }
}