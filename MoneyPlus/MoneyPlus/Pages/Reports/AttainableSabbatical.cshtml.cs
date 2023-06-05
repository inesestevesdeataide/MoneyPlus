namespace MoneyPlus.Pages.Reports;

public class AttainableSabbaticalModel : PageModel
{
    private ApplicationDbContext _context { get; set; }
    private WalletRepository _walletRepository { get; set; }
    private AssetRepository _assetRepository { get; set; }
    private CashOutflowRepository _cashOutflowRepository { get; set; }

    public double EveryWallet { get; set; }
    public double EveryAsset { get; set; }
    public double AverageCostLivingLast2Years { get; set; }
    public double YearsWithoutWorking { get; set; }


    public AttainableSabbaticalModel(ApplicationDbContext context,
        WalletRepository walletRepository, AssetRepository assetRepository,
        CashOutflowRepository cashOutflowRepository)
    {
        _context = context;
        _walletRepository = walletRepository;
        _assetRepository = assetRepository;
        _cashOutflowRepository = cashOutflowRepository;
    }

    [BindProperty]
    public double InflationRate { get; set; }
    [BindProperty]
    public double ReturnOnInvestment { get; set; }

    public async Task OnGetAsync(double? inflationRate, double? returnOnInvestment)
    {
        double realInflationRate = 1;

        if (inflationRate != null)
        {
            realInflationRate = (double)inflationRate / 100 + 1;
        }
        else
        {
            inflationRate = realInflationRate;
        }

        double realReturnOnInvestment = 1;

        if (returnOnInvestment != null)
        {
            realReturnOnInvestment = (double)returnOnInvestment / 100 + 1;
        }
        else
        {
            returnOnInvestment = realReturnOnInvestment;
        }

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var wallets = await _walletRepository.GetWalletsByUserAsync(user);
        double everyWallet = 0;

        foreach (var wallet in wallets)
        {
            everyWallet += wallet.Balance;
        }

        var roundedEveryWallet = Math.Round(everyWallet, 2, MidpointRounding.AwayFromZero);
        EveryWallet = roundedEveryWallet;

        var assets = await _assetRepository.GetAssetsByUserAsync(user);
        double everyAsset = 0;

        foreach (var asset in assets)
        {
            everyAsset += asset.Value;
        }

        var roundedEveryAsset = Math.Round(everyAsset, 2, MidpointRounding.AwayFromZero);
        EveryAsset = roundedEveryAsset;

        var startDate = DateTime.Now.AddYears(-2);
        var endDate = DateTime.Now;

        var allCashOutflows = await _cashOutflowRepository.GetCashOutflowsByUserAsync(user);
        var last2yearscashOutflows = new List<CashOutflow>();

        foreach (var cashOutflow in allCashOutflows)
        {
            if (cashOutflow.Date >= startDate && cashOutflow.Date <= endDate)
            {
                last2yearscashOutflows.Add(cashOutflow);
            }
        }

        double last2YearsSpendings = 0;

        foreach (var cashOutflow in last2yearscashOutflows)
        {
            last2YearsSpendings += cashOutflow.Amount;
        }

        double avgCostLivingLast2Years = last2YearsSpendings / 2;
        var roundedavgCostLivingLast2Years = Math.Round(avgCostLivingLast2Years, 2, MidpointRounding.AwayFromZero);

        AverageCostLivingLast2Years = roundedavgCostLivingLast2Years;

        double yearsWithoutWorking = 0;

        var wealth = everyAsset + everyWallet;
        var moreThanLifeExpectancy = 2500;

        while (wealth > 0 && yearsWithoutWorking <= moreThanLifeExpectancy)
        {
            avgCostLivingLast2Years *= realInflationRate;
            wealth -= avgCostLivingLast2Years;

            if (wealth > 0)
            {
                wealth *= realReturnOnInvestment;
                yearsWithoutWorking += 1;
            }
        }
        var roundedYearsWithoutWorking = Math.Round(yearsWithoutWorking, 2, MidpointRounding.AwayFromZero);
        YearsWithoutWorking = roundedYearsWithoutWorking;
    }
}
