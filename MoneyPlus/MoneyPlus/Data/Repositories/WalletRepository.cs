namespace MoneyPlus.Data.Repositories;

public class WalletRepository
{
    private readonly ApplicationDbContext _context;

    public WalletRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet> CreateAsync(Wallet wallet)
    {
        wallet.IsActive = true;

        _context.Wallet.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }

    public async Task<Wallet> GetWalletByIdAsync(int id)
    {
        return await _context.Wallet.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Wallet>> GetWalletsByUserAsync(string user)
    {
        var wallets = await _context.Wallet
            .Include(w => w.User).Where(w => w.UserId == user && w.IsActive == true).ToListAsync();

        wallets.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));

        return wallets;
    }

    public async Task<Wallet> UpdateWalletAsync(Wallet wallet)
    {
        _context.Attach(wallet).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return wallet;
    }

    public bool WalletExists(int id)
    {
        return _context.Wallet.Any(e => e.Id == id);
    }

    public bool WalletIsDuplicated(Wallet wallet, string user)
    {
        var duplicate = _context.Wallet
            .Where(w => w.Name.ToLower() == wallet.Name.ToLower() && w.Id != wallet.Id
            && w.IsActive == true && w.UserId == wallet.UserId)
            .ToList();

        return duplicate.Count() > 0;
    }

    // Soft Delete
    public async Task<Wallet> DeleteWallet(Wallet wallet)
    {
        wallet.IsActive = false;
        wallet.Balance = 0;

        return await UpdateWalletAsync(wallet);
    }
}