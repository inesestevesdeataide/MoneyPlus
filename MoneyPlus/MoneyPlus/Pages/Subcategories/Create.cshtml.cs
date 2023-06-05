namespace MoneyPlus.Pages.Subcategories;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
        return Page();
    }

    [BindProperty]
    public Subcategory Subcategory { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        var sameName = await _context.Subcategory
            .Where(s => s.Name.ToLower() == Subcategory.Name.ToLower() && s.Id != Subcategory.Id && s.CategoryId == Subcategory.CategoryId)
            .ToListAsync();

        if (!ModelState.IsValid || sameName.Count() > 0)
        {
            return RedirectToPage("./Create");
        }

        Subcategory.IsActive = true;
        _context.Subcategory.Add(Subcategory);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}