namespace MoneyPlus.Data.Repositories;

public partial class CashOutflowRepository
{
    private readonly ApplicationDbContext _context;

    public CashOutflowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CashOutflow>> GetCashOutflowsByUserAsync(string user)
    {
        var cashOutflows = await _context.CashOutflow
            .Include(c => c.Asset)
            .Include(c => c.OriginWallet)
            .Include(c => c.Payee)
            .Include(c => c.Subcategory)
            .Include(c => c.Subcategory.Category)
            .Where(c => c.OriginWallet.UserId == user).ToListAsync();

        cashOutflows.Sort((c1, c2) => c2.Date.CompareTo(c1.Date));

        return cashOutflows;
    }

    public async Task<List<CashOutflow>> GetCashOutflowsByUserAndMonthAsync(string user, string? monthYear)
    {
        if (monthYear == null)
        {
            monthYear = DateTime.Now.Year.ToString() + " " + DateTime.Now.Month.ToString();
        }

        var splittedMonthYear = monthYear.Split(" ");

        int year = int.Parse(splittedMonthYear[0]);
        var month = int.Parse(splittedMonthYear[1]);

        var cashOutflows = await _context.CashOutflow
            .Include(c => c.Asset)
            .Include(c => c.OriginWallet)
            .Include(c => c.Payee)
            .Include(c => c.Subcategory)
            .Include(c => c.Subcategory.Category)
            .Where(c => c.OriginWallet.UserId == user && c.Date.Year == year && c.Date.Month == month).ToListAsync();

        cashOutflows.Sort((c1, c2) => c2.Date.CompareTo(c1.Date));

        return cashOutflows;
    }

    public async Task<List<MonthlyCashOutflow>> FilterCashOutflowListingAsync(string user, string? monthYear
        , int assetId, int categoryId, int payeeId)
    {
        if (monthYear == null)
        {
            monthYear = DateTime.Now.Year.ToString() + " " + DateTime.Now.Month.ToString();
        }

        var splittedMonthYear = monthYear.Split(" ");

        int year = int.Parse(splittedMonthYear[0]);
        var month = int.Parse(splittedMonthYear[1]);

        var cashOutflows = from co in _context.CashOutflow
                           join p in _context.Payee on co.PayeeId equals p.Id
                           join a in _context.Asset on co.AssetId equals a.Id into asset
                           from asset1 in asset.DefaultIfEmpty()
                           join s in _context.Subcategory on co.SubcategoryId equals s.Id
                           join c in _context.Category on co.Subcategory.CategoryId equals c.Id
                           join w in _context.Wallet on co.OriginWalletId equals w.Id
                           where w.UserId == user && co.Type == RecordType.CashOutflow && co.Date.Year == year && co.Date.Month == month
                           orderby co.Date
                           select new MonthlyCashOutflow
                           {
                               Date = co.Date,
                               PayeeId = p.Id,
                               Payee = p.Name,
                               Amount = co.Amount,
                               Description = co.Description,
                               AssetId = asset1.Id,
                               Asset = asset1.Name,
                               SubcategoryId = s.Id,
                               Subcategory = s.Name,
                               CategoryId = c.Id,
                               Category = c.Name,
                               OriginWalletId = w.Id,
                               OriginWallet = w.Name
                           };

        if (assetId != 0)
        {
            cashOutflows = cashOutflows.Where(c => c.AssetId == assetId);
        }
        if (categoryId != 0)
        {
            cashOutflows = cashOutflows.Where(c => c.CategoryId == categoryId);
        }
        if (payeeId != 0)
        {
            cashOutflows = cashOutflows.Where(c => c.PayeeId == payeeId);
        }

        return await cashOutflows.ToListAsync();
    }

    public List<int> GetDistinctYears(string user)
    {
        var result = from co in _context.CashOutflow
                     join ow in _context.Wallet on co.OriginWalletId equals ow.Id
                     where ow.UserId == user
                     select co.Date.Year;

        List<int> filledYears = result.Distinct().ToList();

        return filledYears;
    }

    public List<string> GetDistinctMonthYear(string user)
    {
        var result = from co in _context.CashOutflow
                     join ow in _context.Wallet on co.OriginWalletId equals ow.Id
                     where ow.UserId == user
                     select co.Date.Year.ToString() + " " + co.Date.Month.ToString();

        List<string> filledMonths = result.Distinct().ToList();

        return filledMonths;
    }
}