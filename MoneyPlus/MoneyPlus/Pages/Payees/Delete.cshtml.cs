namespace MoneyPlus.Pages.Payees;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly PayeeRepository _repository;

    public DeleteModel(PayeeRepository repository)
    {
        _repository = repository;
    }

    [BindProperty]
    public Payee Payee { get; set; }

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
        else 
        {
            Payee = payee;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null || _repository == null)
        {
            return NotFound();
        }
        var payee = await _repository.GetPayeeByIdAsync((int)id);

        if (payee != null)
        {
            await _repository.DeletePayee(payee);
        }

        return RedirectToPage("./Index");
    }
}