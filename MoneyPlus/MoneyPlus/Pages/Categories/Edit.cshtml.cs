namespace MoneyPlus.Pages.Categories;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; } = default!;
    [BindProperty]
    public bool InitialStatus { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Category == null)
        {
            return NotFound();
        }

        var category =  await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        Category = category;
        InitialStatus = Category.IsActive;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var sameName = await _context.Category.Where(c => c.Name.ToLower() == Category.Name.ToLower() && c.Id != Category.Id && c.RecordType == Category.RecordType).ToListAsync();

        if (!ModelState.IsValid || sameName.Count() > 0)
        {
            return Page();
        }

        if (InitialStatus != Category.IsActive)
        {
            var subcategoriesInCategory = await _context.Subcategory.Where(s => s.CategoryId == Category.Id).ToListAsync();

            foreach (var sub in subcategoriesInCategory)
            {
                sub.IsActive = Category.IsActive;
                _context.Attach(sub).State = EntityState.Modified;
            }
        }
        _context.Attach(Category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(Category.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool CategoryExists(int id)
    {
      return _context.Category.Any(e => e.Id == id);
    }
}