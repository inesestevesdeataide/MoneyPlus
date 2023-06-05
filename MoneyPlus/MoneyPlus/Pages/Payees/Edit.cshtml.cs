namespace MoneyPlus.Pages.Payees;

[Authorize]
public class EditModel : PageModel
{
    private readonly PayeeRepository _repository;
    private readonly ILogger<EditModel> _logger;

    public EditModel(PayeeRepository repository, ILogger<EditModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [BindProperty]
    public Payee Payee { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _repository == null)
        {
            return NotFound();
        }

        var payee = await _repository.GetPayeeByIdAsync((int)id);

        if (payee == null)
        {
            return NotFound();
        }

        Payee = payee;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var hasDuplicates = _repository.PayeeIsDuplicated(Payee, Payee.UserId);

        if (!ModelState.IsValid || hasDuplicates)
        {
            _logger.LogError($"User {Payee.UserId} inserted invalid fields when editing Payee.");
            return Page();
        }

        try
        {
            await _repository.UpdatePayeeAsync(Payee);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_repository.PayeeExists(Payee.Id))
            {
                _logger.LogCritical($"Not found by user {Payee.UserId}");
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }
}