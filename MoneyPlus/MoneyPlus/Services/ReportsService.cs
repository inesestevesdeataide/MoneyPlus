namespace MoneyPlus.Services;

public class ReportsService
{
    private CashOutflowRepository _cashOutflowRepository { get; set; } 

    public ReportsService(CashOutflowRepository cashOutflowRepository)
	{
        _cashOutflowRepository = cashOutflowRepository;
    }

    public async Task<YearlyBalance> GetYearlyBalanceByCategoryByUser (string user)
    {
        var cashOutflows = await _cashOutflowRepository.GetCashOutflowsByUserAsync(user);

        var yearlyBalance = new Dictionary<int, Dictionary<string, double>>();

        foreach (var cashOutflow in cashOutflows)
        {
            var year = cashOutflow.Date.Year;
            var cat = cashOutflow.Subcategory.Category.Name;

            if (!yearlyBalance.ContainsKey(year))
            {
                yearlyBalance.Add(year, new Dictionary<string, double>());
            }
            if (!yearlyBalance[year].ContainsKey(cat))
            {
                yearlyBalance[year].Add(cat, 0);
            }

            yearlyBalance[year][cat] += cashOutflow.Amount;
        }
        return new YearlyBalance(yearlyBalance);
    }

    public async Task<MonthlyBalance> GetMonthlyBalanceByCategoryByUser(int year, string user)
    {
        var allCashOutflows = await _cashOutflowRepository.GetCashOutflowsByUserAsync(user);
        var start = new DateTime(year, 1, 1, 0, 0, 0);
        var end = new DateTime(year + 1, 1, 1, 0, 0, 0);
        var cashOutflows = allCashOutflows.Where(c => c.Date >= start && c.Date < end);

        //TODO: Passar para método no repository

        var monthlyBalance = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

        foreach (var cashOutflow in cashOutflows)
        {
            var intMonth = cashOutflow.Date.Month;
            var month = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(intMonth);

            var cat = cashOutflow.Subcategory.Category.Name;
            var sub = cashOutflow.Subcategory.Name;

            if (!monthlyBalance.ContainsKey(month))
            {
                monthlyBalance.Add(month, new Dictionary<string, Dictionary<string, double>>());
            }
            if (!monthlyBalance[month].ContainsKey(cat))
            {
                monthlyBalance[month].Add(cat, new Dictionary<string, double>());
            }
            if (!monthlyBalance[month][cat].ContainsKey(sub))
            {
                monthlyBalance[month][cat].Add(sub, 0);
            }

            monthlyBalance[month][cat][sub] += cashOutflow.Amount;
        }
        return new MonthlyBalance(monthlyBalance);
    }
}