namespace MoneyPlus.Pages.Payees;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly PayeeRepository _repository;

    public DetailsModel(PayeeRepository repository)
    {
        _repository = repository;
    }

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
}