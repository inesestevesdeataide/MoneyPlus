namespace MoneyPlus.Pages.Categories;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Category == null)
        {
            return NotFound();
        }

        var category = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }
        else 
        {
            Category = category;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Category == null)
        {
            return NotFound();
        }

        var category = await _context.Category.FindAsync(id);

        if (category != null)
        {
            category.IsActive = false;

            _context.Attach(category).State = EntityState.Modified;

            var subcategoriesInCategory = await _context.Subcategory.Where(s => s.CategoryId == category.Id).ToListAsync();

            foreach (var sub in subcategoriesInCategory)
            {
                sub.IsActive = false;
                _context.Attach(sub).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}