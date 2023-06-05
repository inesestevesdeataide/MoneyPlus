namespace MoneyPlus.Pages.Payees;

[Authorize]
public class CreateModel : PageModel
{
    private readonly PayeeRepository _repository;
    private readonly ILogger<CreateModel> _logger;


    public CreateModel(PayeeRepository repository, ILogger<CreateModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IActionResult OnGet(string? previousPage)
    {
        return Page();
    }

    [BindProperty]
    public Payee Payee { get; set; }
    [BindProperty]
    public string? PreviousPage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        Payee.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var hasDuplicates = _repository.PayeeIsDuplicated(Payee, Payee.UserId);

        if (!ModelState.IsValid || hasDuplicates)
        {
            _logger.LogError($"User {Payee.UserId} inserted invalid fields when creating Payee.");
            return Page();
        }

        await _repository.CreateAsync(Payee);

        if (PreviousPage == null)
        {
            return RedirectToPage("./Index");
        }
        else
        {
            return RedirectToPage(".././" + PreviousPage);
        }
    }
}