namespace MoneyPlus.Pages.Reports;

[Authorize]
public class YearlyBalanceByCategoryModel : PageModel
{
    private CashOutflowRepository _cashOutflowRepository { get; set; }
    private ReportsService _reportsService { get; set; }

    public YearlyBalance Report { get; set; }

    public YearlyBalanceByCategoryModel(CashOutflowRepository cashOutflowRepository, ReportsService reportsService)
    {
        _cashOutflowRepository = cashOutflowRepository;
        _reportsService = reportsService;
    }

    public async Task OnGetAsync()
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        Report = await _reportsService.GetYearlyBalanceByCategoryByUser(user);
    }
}