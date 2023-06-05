namespace MoneyPlus.Pages.Reports;

public class MonthlyCashOutflowListingModel : PageModel
{
    private ApplicationDbContext _context { get; set; }
    private CashOutflowRepository _cashOutflowRepository { get; set; }

    private List<string> _filledMonths { get; set; }
    private int _index { get; set; }

    public string MonthYear { get; set; }

    public IList<MonthlyCashOutflow> CashOutflow { get; set; } = default!;

    public MonthlyCashOutflowListingModel(CashOutflowRepository cashOutflowRepository,
        ApplicationDbContext context)
    {
        _cashOutflowRepository = cashOutflowRepository;
        _context = context;
    }

    [BindProperty]
    public int AssetId { get; set; }
    [BindProperty]
    public int CategoryId { get; set; }
    [BindProperty]
    public int PayeeId { get; set; }


    public async Task OnGetAsync(string? monthYear)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var filledMonths = _cashOutflowRepository.GetDistinctMonthYear(user);

        MonthYear = monthYear == null ? DateTime.Now.Year + " " + DateTime.Now.Month : monthYear;

        if (!filledMonths.Contains(MonthYear))
        {
            filledMonths.Add(MonthYear);
        }

        filledMonths.Sort((d1, d2) => CompareDates(d1, d2));

        _filledMonths = filledMonths;
        _index = filledMonths.IndexOf(MonthYear);

        var cats = from cat in _context.Category
                   where cat.RecordType == RecordType.CashOutflow
                   select new SelectListItem()
                   {
                       Value = cat.Id.ToString(),
                       Text = cat.Name
                   };

        var categories = cats.ToList();

        ViewData["AssetId"] = new SelectList(_context.Asset.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        ViewData["CategoryId"] = categories;
        ViewData["PayeeId"] = new SelectList(_context.Payee.Where(p => p.UserId == user && p.IsActive == true), "Id", "Name");
        
        if (_context.CashOutflow != null)
        {
            var passparAssetId = Request.Query["AssetId"];
            var passparCategoryId = Request.Query["CategoryId"];
            var passparPayeeId = Request.Query["PayeeId"];

            int parsedAssetId = 0;
            int parsedCategoryId = 0;
            int parsedPayeeId = 0;

            if (passparAssetId.Count != 0 && int.Parse(passparAssetId) != 0)
            {
                parsedAssetId = int.Parse(passparAssetId);
                AssetId = parsedAssetId;
            }

            if (passparCategoryId.Count != 0 && int.Parse(passparCategoryId) != 0)
            {
                parsedCategoryId = int.Parse(passparCategoryId);
                CategoryId = parsedCategoryId;
            }

            if (passparPayeeId.Count != 0 && int.Parse(passparPayeeId) != 0)
            {
                parsedPayeeId = int.Parse(passparPayeeId);
                PayeeId = parsedPayeeId;
            }

            CashOutflow = await _cashOutflowRepository.FilterCashOutflowListingAsync(user, MonthYear, parsedAssetId, parsedCategoryId, parsedPayeeId);
        }
    }

    private int CompareDates(string d1, string d2)
    {
        int.TryParse(d1.Split(" ")[0], out int y1);
        int.TryParse(d2.Split(" ")[0], out int y2);

        int.TryParse(d1.Split(" ")[1], out int m1);
        int.TryParse(d2.Split(" ")[1], out int m2);

        if (y1 == y2)
        {
            return m1 < m2 ? -1 : 1;
        }
        else
        {
            return y1 < y2 ? -1 : 1;
        }
    }

    public string PreviousMonth()
    {
        if (HasPrevious())
        {
            return _filledMonths[_index - 1];
        }
        return MonthYear;
    }

    public string NextMonth()
    {
        if (HasNext())
        {
            return _filledMonths[_index + 1];
        }
        return MonthYear;
    }

    public bool HasPrevious()
    {
        return _index > 0;
    }

    public bool HasNext()
    {
        return _index < _filledMonths.Count - 1;
    }
}