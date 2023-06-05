namespace MoneyPlus.Pages.Assets;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly AssetRepository _repository;

    public DeleteModel(AssetRepository repository)
    {
        _repository = repository;
    }

    [BindProperty]
    public Asset Asset { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _repository == null)
        {
            return NotFound();
        }

        var asset = await _repository.GetAssetByIdAsync((int)id);

        if (asset == null)
        {
            return NotFound();
        }
        else 
        {
            Asset = asset;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _repository == null)
        {
            return NotFound();
        }

        var asset = await _repository.GetAssetByIdAsync((int)id);

        if (asset != null)
        {
            await _repository.DeleteAsset(asset);
        }

        return RedirectToPage("./Index");
    }
}