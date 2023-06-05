namespace MoneyPlus.Pages.Reports;

[Authorize]
public class MonthlyBalanceByCategoryAndSubModel : PageModel
{
    private CashOutflowRepository _cashOutflowRepository { get; set; }
    private ReportsService _reportsService { get; set; }

    private List<int> _filledYears { get; set; }
    private int _index { get; set; }

    public MonthlyBalance Report { get; set; }
    public int Year { get; set; }


    public MonthlyBalanceByCategoryAndSubModel(CashOutflowRepository cashOutflowRepository,
        ReportsService reportsService)
    {
        _cashOutflowRepository = cashOutflowRepository;
        _reportsService = reportsService;
    }

    public async Task OnGetAsync(int? year)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var filledYears = _cashOutflowRepository.GetDistinctYears(user);

        Year = year == null ? DateTime.Now.Year : (int)year;

        if (!filledYears.Contains(Year))
        {
            filledYears.Add(Year);
        }

        filledYears.Sort();

        _filledYears = filledYears;
        _index = filledYears.IndexOf(Year);

        Report = await _reportsService.GetMonthlyBalanceByCategoryByUser(Year, user);
    }

    public int PreviousYear()
    {
        if (HasPrevious())
        {
            return _filledYears[_index - 1];
        }
        return Year;
    }

    public int NextYear()
    {
        if(HasNext())
        {
            return _filledYears[_index + 1];
        }
        return Year;
    }

    public bool HasPrevious()
    {
        return _index > 0; 
    }

    public bool HasNext()
    {
        return _index < _filledYears.Count - 1;
    }
}