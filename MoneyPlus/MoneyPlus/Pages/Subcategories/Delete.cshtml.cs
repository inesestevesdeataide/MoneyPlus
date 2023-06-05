namespace MoneyPlus.Pages.Subcategories;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Subcategory Subcategory { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Subcategory == null)
        {
            return NotFound();
        }

        var subcategory = await _context.Subcategory.FirstOrDefaultAsync(m => m.Id == id);

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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _context.Subcategory == null)
        {
            return NotFound();
        }
        var subcategory = await _context.Subcategory.FindAsync(id);

        if (subcategory != null)
        {
            subcategory.IsActive = false;
            _context.Subcategory.Update(subcategory);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}