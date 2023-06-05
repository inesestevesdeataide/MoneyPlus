namespace MoneyPlus.Pages.Assets;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly AssetRepository _repository;

    public DetailsModel(AssetRepository repository)
    {
        _repository = repository;
    }

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
}