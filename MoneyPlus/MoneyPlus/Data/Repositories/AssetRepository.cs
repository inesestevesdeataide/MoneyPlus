namespace MoneyPlus.Data.Repositories;

public class AssetRepository
{
    private readonly ApplicationDbContext _context;

    public AssetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Asset> CreateAsync(Asset asset)
    {
        asset.IsActive = true;

        _context.Asset.Add(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset> GetAssetByIdAsync(int id)
    {
        return await _context.Asset.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Asset>> GetAssetsByUserAsync(string user)
    {
        var assets = await _context.Asset
            .Include(a => a.User).Where(a => a.IsActive == true && a.UserId == user).ToListAsync();

        assets.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));

        return assets;
    }

    public async Task<Asset> UpdateAssetAsync(Asset asset)
    {
        _context.Attach(asset).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return asset;
    }

    public bool AssetExists(int id)
    {
        return _context.Asset.Any(e => e.Id == id);
    }

    public bool AssetIsDuplicated(Asset asset, string user)
    {
        var duplicate = _context.Asset
            .Where(a => a.Name.ToLower() == asset.Name.ToLower() && a.Id != asset.Id
            && a.IsActive == true && a.UserId == asset.UserId)
            .ToList();

        return duplicate.Count() > 0;
    }

    // Soft Delete
    public async Task<Asset> DeleteAsset(Asset asset)
    {
        asset.IsActive = false;
        asset.Value = 0;

        return await UpdateAssetAsync(asset);
    }
}