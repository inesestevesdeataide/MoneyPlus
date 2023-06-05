namespace MoneyPlus.Pages.Assets;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AssetRepository _repository;

    public IndexModel(AssetRepository repository)
    {
        _repository = repository;
    }

    public IList<Asset> Asset { get;set; } = default!;

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (_repository != null)
        {
            Asset = await _repository.GetAssetsByUserAsync(user);
        }
    }
}