namespace MoneyPlus.Pages.Subcategories;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Subcategory Subcategory { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Subcategory == null)
        {
            return NotFound();
        }

        var subcategory =  await _context.Subcategory.FirstOrDefaultAsync(m => m.Id == id);

        if (subcategory == null)
        {
            return NotFound();
        }

        Subcategory = subcategory;
        subcategory.Category = await _context.Category.FirstOrDefaultAsync(c => c.Id == subcategory.CategoryId);

        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var sameName = await _context.Subcategory
            .Where(s => s.Name.ToLower() == Subcategory.Name.ToLower() && s.Id != Subcategory.Id && s.CategoryId == Subcategory.CategoryId)
            .ToListAsync();

        if (!ModelState.IsValid || sameName.Count() > 0)
        {
            return Page();
        }

        _context.Attach(Subcategory).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SubcategoryExists(Subcategory.Id))
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

    private bool SubcategoryExists(int id)
    {
      return _context.Subcategory.Any(e => e.Id == id);
    }
}