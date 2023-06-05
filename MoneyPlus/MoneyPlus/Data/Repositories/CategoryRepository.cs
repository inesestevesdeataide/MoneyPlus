namespace MoneyPlus.Data.Repositories;

public class CategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
		{
        _context = context;

    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        var categories = await _context.Category.ToListAsync();

        categories.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));

        return categories;
    }
}