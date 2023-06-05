namespace MoneyPlus.Pages.Categories;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly CategoryRepository _repository;

    public IndexModel (CategoryRepository repository)
    {
        _repository = repository;
    }

    public IList<Category> Category { get;set; } = default!;

    public async Task OnGetAsync()
    {
        if (_repository != null)
        {
           Category = await _repository.GetAllCategoriesAsync();
        }
    }
}