namespace MoneyPlus.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MoneyPlus.Data.Entities.Category> Category { get; set; }
    public DbSet<MoneyPlus.Data.Entities.Wallet> Wallet { get; set; }
    public DbSet<MoneyPlus.Data.Entities.Subcategory> Subcategory { get; set; }
    public DbSet<MoneyPlus.Data.Entities.CashInflow> CashInflow { get; set; }
    public DbSet<MoneyPlus.Data.Entities.Asset> Asset { get; set; }
    public DbSet<MoneyPlus.Data.Entities.CashOutflow> CashOutflow { get; set; }
    public DbSet<MoneyPlus.Data.Entities.Payee> Payee { get; set; }
    public DbSet<MoneyPlus.Data.Entities.Transfer> Transfer { get; set; }
    public DbSet<MoneyPlus.Data.Entities.Investment> Investment { get; set; }
    public DbSet<MoneyPlus.Data.Entities.DailyEmail> DailyEmail { get; set; }
}