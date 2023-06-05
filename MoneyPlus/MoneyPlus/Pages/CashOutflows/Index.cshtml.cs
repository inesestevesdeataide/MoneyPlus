namespace MoneyPlus.Pages.CashOutflows;

[Authorize]
public class IndexModel : PageModel
{
    private readonly CashOutflowRepository _repository;

    public IndexModel(CashOutflowRepository repository)
    {
        _repository = repository;
    }

    public IList<CashOutflow> CashOutflow { get;set; } = default!;

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (_repository != null)
        {
            CashOutflow = await _repository.GetCashOutflowsByUserAsync(user);
        }
    }
}