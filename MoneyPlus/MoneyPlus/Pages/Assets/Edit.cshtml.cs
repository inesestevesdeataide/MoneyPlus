namespace MoneyPlus.Pages.Assets;

[Authorize]
public class EditModel : PageModel
{
    private readonly AssetRepository _repository;
    private readonly ILogger<EditModel> _logger;

    public EditModel(AssetRepository repository,ILogger<EditModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [BindProperty]
    public Asset Asset { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _repository == null)
        {
            return NotFound();
        }

        var asset =  await _repository.GetAssetByIdAsync((int)id);

        if (asset == null)
        {
            return NotFound();
        }

        Asset = asset;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var hasDuplicates = _repository.AssetIsDuplicated(Asset, Asset.UserId);

        if (!ModelState.IsValid || hasDuplicates)
        {
            _logger.LogError($"User {Asset.UserId} inserted invalid fields when editing Asset.");
            return Page();
        }

        try
        {
            await _repository.UpdateAssetAsync(Asset);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_repository.AssetExists(Asset.Id))
            {
                _logger.LogCritical($"Not found by user {Asset.UserId}");
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