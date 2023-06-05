namespace MoneyPlus.Pages.Subcategories;

[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

  public Subcategory Subcategory { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Subcategory == null)
        {
            return NotFound();
        }

        var subcategory = await _context.Subcategory.FirstOrDefaultAsync(s => s.Id == id);
        subcategory.Category = await _context.Category.FirstOrDefaultAsync(c => c.Id == subcategory.CategoryId);

        if (subcategory == null)
        {
            return NotFound();
        }
        else 
        {
            Subcategory = subcategory;
        }
        return Page();
    }
}