namespace MoneyPlus.Data.Models;

public class YearlyBalance
{
    private Dictionary<int, Dictionary<string, double>> _report { get; set; }
    private Dictionary<int, string> _totalByYear { get; set; }

    public List<int> Years { get; set; }
    public List<string> Categories { get; set; }

    public YearlyBalance(Dictionary<int, Dictionary<string, double>> report)
	{
        _report = report;

        var years = new List<int>();
        var categories = new List<string>();

        var totals = new Dictionary<int, string>();

        foreach (KeyValuePair<int, Dictionary<string, double>> kvp in _report)
        {
            years.Add(kvp.Key);

            double total = 0;

            foreach (KeyValuePair<string, double> innerKvp in kvp.Value)
            {
                if (!categories.Contains(innerKvp.Key))
                {
                    categories.Add(innerKvp.Key);
                }

                total += innerKvp.Value;
            }
            totals.Add(kvp.Key, String.Format("{0:0.00}", total));
        }
        categories.Sort();

        Categories = categories;
        Years = years;
        _totalByYear = totals;
    }

    public string GetCellValue(int year, string cat)
    {
        if (!_report[year].ContainsKey(cat))
        {
            return "0,00";
        }
        else
        {
            return String.Format("{0:0.00}", _report[year][cat]);
        }
    }

    public string GetTotalByYear(int year)
    {
        return _totalByYear[year];
    }
}