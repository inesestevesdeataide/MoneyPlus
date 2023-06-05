namespace MoneyPlus.BackgroundServices;

public class FakeEmailSenderBackgroundService : BackgroundService
{
    static int timer = 24;
    TimeSpan IntervalBetweenJobs = TimeSpan.FromHours(timer);
    public IServiceProvider _serviceProvider { get; }

    public FakeEmailSenderBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync();

            await Task.Delay(IntervalBetweenJobs);
        }
    }

    private async Task DoWorkAsync()
    {
        try
        {
            await SendEmailAsync();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Booom at: {DateTime.UtcNow}");
        }
    }

    private async Task SendEmailAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var allUsers = ctx.Users.ToList();

        double walletsUser = 0;
        double assetsUser = 0;
        double wealth = 0;
        string strWealth = "";

        foreach (var user in allUsers)
        {
            walletsUser = ctx.Wallet.Where(w => w.UserId == user.Id).Sum(w => w.Balance);
            assetsUser = ctx.Asset.Where(a => a.UserId == user.Id).Sum(a => a.Value);

            wealth = walletsUser + assetsUser;
            strWealth = String.Format("{0:0.00}", wealth);

            var dailyEmail = new DailyEmail();

            dailyEmail.EmailSender = "moneyplus.app@gmail.com";
            dailyEmail.Password = "m0neyPlu3.";
            dailyEmail.EmailServerAddress = "";
            dailyEmail.EmailServerPort = "";
            dailyEmail.UseSsl = true;
            dailyEmail.EmailRecipient = user.Email;
            dailyEmail.Wealth = wealth;
            dailyEmail.EmailSubject = "Your Money+ Daily";
            dailyEmail.EmailBody = $@"
Hi!

We at Money+ hope you're doing great!

Just passing by to let you know your current wealth to this day is € {strWealth}.

You be good and don't you miss us...
Money+";
            dailyEmail.Date = DateTime.Now;
            dailyEmail.UserId = user.Id;

            ctx.DailyEmail.Add(dailyEmail);
            await ctx.SaveChangesAsync();
        }
    }
}