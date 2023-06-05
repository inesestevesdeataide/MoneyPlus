namespace MoneyPlus.Data.Repositories;

public class PayeeRepository
{
    private readonly ApplicationDbContext _context;

    public PayeeRepository(ApplicationDbContext context)
	{
        _context = context;
    }

    public async Task<Payee> CreateAsync(Payee payee)
    {
        payee.IsActive = true;
        _context.Payee.Add(payee);
        await _context.SaveChangesAsync();
        return payee;
    }

    public async Task<Payee> GetPayeeByIdAsync(int id)
    {
        return await _context.Payee.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Payee>> GetPayeesByUserAsync(string user)
    {
        var payees  = await _context.Payee
            .Include(p => p.User).Where(p => p.IsActive== true && p.UserId == user).ToListAsync();

        payees.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));

        return payees;
    }

    public async Task<Payee> UpdatePayeeAsync(Payee payee)
    {
        _context.Attach(payee).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return payee;
    }

    public bool PayeeExists(int id)
    {
        return _context.Payee.Any(e => e.Id == id);
    }

    public bool PayeeIsDuplicated(Payee payee, string user)
    {
        var duplicate = _context.Payee
            .Where(p => p.Name.ToLower() == payee.Name.ToLower() && p.Id != payee.Id
            && p.IsActive == true && p.UserId == payee.UserId && p.TaxNumber == payee.TaxNumber)
            .ToList();

        return duplicate.Count() > 0;
    }

    // Soft Delete
    public async Task<Payee> DeletePayee(Payee payee)
    {
        payee.IsActive = false;

        return await UpdatePayeeAsync(payee);
    }
}