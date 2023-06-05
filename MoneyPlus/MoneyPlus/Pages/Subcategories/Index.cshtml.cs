namespace MoneyPlus.Pages.Subcategories;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly SubcategoryRepository _repository;

    public IndexModel(SubcategoryRepository repository)
    {
        _repository = repository;
    }

    public IList<Subcategory> Subcategory { get;set; } = default!;

    public async Task OnGetAsync()
    {
        if (_repository != null)
        {
            Subcategory = await _repository.GetAllSubcategoriesAsync();
        }
    }
}