namespace MoneyPlus.Data.Models;

public class MonthlyBalance
{
    private Dictionary<string, Dictionary<string, Dictionary<string, double>>> _report { get; set; }
    private Dictionary<string, List<string>> _catSubcategories { get; set; }
    private Dictionary<string, double> _catMonthly { get; set; }
    private Dictionary<string, string> _totalByMonth { get; set; }
    private Dictionary<string, double> _totalByCat { get; set; }
    private Dictionary<string, double> _totalBySub { get; set; }

    public List<string> Months { get; set; }
    public List<string> Categories { get; set; }
    public string Total { get; set; }


    public MonthlyBalance(Dictionary<string, Dictionary<string, Dictionary<string, double>>> report)
	{
        _report = report;

        var months = new List<string>();
        var categories = new List<string>();

        var catMonth = new Dictionary<string, double>();
        var catSubcategories = new Dictionary<string, List<string>>();

        var totalsMonth = new Dictionary<string, string>();
        var totalsCat = new Dictionary<string, double>();
        var totalsSub = new Dictionary<string, double>();
        double totals = 0;

        foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, double>>> kvpMonth in _report)
        {
            months.Add(kvpMonth.Key);

            double totalMonth = 0;

            foreach (KeyValuePair<string, Dictionary<string, double>> kvpCat in kvpMonth.Value)
            {
                if (!categories.Contains(kvpCat.Key))
                {
                    categories.Add(kvpCat.Key);
                    totalsCat.Add(kvpCat.Key, 0);
                    catSubcategories.Add(kvpCat.Key, new List<String>());
                }

                var catMonthKey = kvpMonth.Key + kvpCat.Key;

                if (!catMonth.ContainsKey(catMonthKey))
                {
                    catMonth.Add(catMonthKey, 0);
                }

                foreach (KeyValuePair<string, double> kvpSub in kvpCat.Value)
                {
                    if (!catSubcategories[kvpCat.Key].Contains(kvpSub.Key))
                    {
                        catSubcategories[kvpCat.Key].Add(kvpSub.Key);
                        totalsSub.Add(kvpSub.Key, 0);
                    }

                    totalMonth += kvpSub.Value;
                    totalsCat[kvpCat.Key] += kvpSub.Value;
                    totalsSub[kvpSub.Key] += kvpSub.Value;
                    catMonth[catMonthKey] += kvpSub.Value;
                }
            }
            totalsMonth.Add(kvpMonth.Key, String.Format("{0:0.00}", totalMonth));
            totals += totalMonth;
        }
        categories.Sort();

        _catSubcategories = catSubcategories;
        Categories = categories;
        Months = months;
        _totalByMonth = totalsMonth;
        _totalByCat = totalsCat;
        _totalBySub = totalsSub;
        _catMonthly = catMonth;
        Total = String.Format("{0:0.00}", totals);
    }

    public string GetCellValue(string month, string cat, string sub)
    {
        if (!_report[month].ContainsKey(cat))
        {
            return "0,00";
        }
        else if (!_report[month][cat].ContainsKey(sub))
        {
            return "0,00";
        }
        else
        {
            return String.Format("{0:0.00}", _report[month][cat][sub]);
        }
    }

    public string GetCatCellValue(string month, string cat)
    {
        var key = month + cat;

        if (!_catMonthly.ContainsKey(key))
        {
            return "0,00";
        }
        else
        {
            return String.Format("{0:0.00}", _catMonthly[key]);
        }
    }

    public List<string> GetSubcategoriesByCat(string cat)
    {
        return _catSubcategories[cat];
    }

    public string GetTotalByMonth(string month)
    {
        return _totalByMonth[month];
    }

    public string GetTotalInCat(string cat)
    {
        if (!_totalByCat.ContainsKey(cat))
        {
            return "0,00";
        }
        else
        {
            return String.Format("{0:0.00}", _totalByCat[cat]);
        }
    }

    public string GetTotalInSub(string sub)
    {
        if (!_totalBySub.ContainsKey(sub))
        {
            return "0,00";
        }
        else
        {
            return String.Format("{0:0.00}", _totalBySub[sub]);
        }
    }
}