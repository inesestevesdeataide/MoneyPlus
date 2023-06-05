namespace MoneyPlus.Pages.Payees;

[Authorize]
public class IndexModel : PageModel
{
    private readonly PayeeRepository _repository;

    public IndexModel(PayeeRepository repository)
    {
        _repository = repository;
    }

    public IList<Payee> Payee { get;set; } = default!;

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (_repository != null)
        {
            Payee = await _repository.GetPayeesByUserAsync(user);
        }
    }
}