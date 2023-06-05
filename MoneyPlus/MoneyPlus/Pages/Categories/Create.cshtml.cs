namespace MoneyPlus.Pages.Categories;

[Authorize (Roles = "Admin")]
public class CreateModel : PageModel
{
    public static IEnumerable Types = new List<IEnumerable>() { RecordType.CashInflow.ToString(),
        RecordType.CashOutflow.ToString(), RecordType.Investment.ToString(), RecordType.Transfer.ToString() };

    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Category Category { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        var sameName = await _context.Category
            .Where(c => c.Name.ToLower() == Category.Name.ToLower() && c.Id != Category.Id && c.RecordType == Category.RecordType)
            .ToListAsync();

        if (!ModelState.IsValid || sameName.Count() > 0)
        {
            return Page();
        }

        Category.IsActive = true;
        _context.Category.Add(Category);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}