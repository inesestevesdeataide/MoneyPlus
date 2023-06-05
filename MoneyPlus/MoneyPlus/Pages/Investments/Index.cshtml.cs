namespace MoneyPlus.Pages.Investments;

[Authorize]
public class IndexModel : PageModel
{
    private readonly InvestmentRepository _repository;

    public IndexModel(InvestmentRepository repository)
    {
        _repository = repository;
    }

    public IList<Investment> Investment { get; set; } = default!;

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (_repository != null)
        {
            Investment = await _repository.GetInvestmentsByUserAsync(user);
        }
    }
}