namespace MoneyPlus.Data.Repositories;

public class SubcategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubcategoryRepository (ApplicationDbContext context)
    {
        _context = context;

    }

    public async Task<List<Subcategory>> GetAllSubcategoriesAsync()
    {
        var subcategories = await _context.Subcategory
            .Include(s => s.Category).ToListAsync();

        subcategories.Sort((s1, s2) => s1.Category.Name.CompareTo(s2.Category.Name)
            == 0 ? s1.Name.CompareTo(s2.Name) : s1.Category.Name.CompareTo(s2.Category.Name));

        return subcategories;
    }
}