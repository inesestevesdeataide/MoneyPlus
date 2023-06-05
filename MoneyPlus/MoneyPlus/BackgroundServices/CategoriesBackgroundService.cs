namespace MoneyPlus.BackgroundServices;

public class CategoriesBackgroundService : BackgroundService
{
    static int timer = 5;
    TimeSpan IntervalBetweenJobs = TimeSpan.FromMinutes(timer);
    public IServiceProvider _serviceProvider { get; }

    public CategoriesBackgroundService(IServiceProvider serviceProvider)
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
            //Path to read file
            var filePath = "/Users/inesestevesdeataide/catSubcatList.yaml";
            // Windows
            //var file = "c:\\temp\\catSubcatList.yaml";

            var fileExists = File.Exists(filePath);

            if (fileExists)
            {
                var categorySubcategoryList = DeserializeCategoryListFromYaml(filePath);
                UpdateCategoryListFromYamlAsync(categorySubcategoryList);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    private async void UpdateCategoryListFromYamlAsync(List<CatAndSubcat> yamlCategoryList)
    {
        using var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        foreach (var yamlCat in yamlCategoryList)
        {
            var cat = new Category();
            cat.Name = yamlCat.Name;
            cat.RecordType = yamlCat.RecordType;
            cat.IsActive = yamlCat.IsActive;

            if (ctx != null)
            {
                var sameCat = ctx.Category.Where(c => c.Name.ToLower() == cat.Name.ToLower() && c.RecordType == cat.RecordType).FirstOrDefault();

                if (sameCat != null)
                {
                    cat = sameCat;
                }
                else
                {
                    ctx.Category.Add(cat);
                }
            }

            if (yamlCat.Subcategories != null)
            {
                foreach (var yamlSub in yamlCat.Subcategories)
                {
                    var sub = new Subcategory();
                    sub.Name = yamlSub.Name;
                    sub.IsActive = yamlSub.IsActive;
                    sub.Category = cat;
                    sub.CategoryId = cat.Id;

                    var sameSub = ctx.Subcategory
                        .Where(s => s.Name.ToLower() == sub.Name.ToLower() && s.CategoryId == sub.CategoryId)
                        .FirstOrDefault();

                    if (sameSub != null)
                    {
                        sub = sameSub;
                    }
                    else
                    {
                        ctx.Subcategory.Add(sub);
                    }
                }
            }
        }
        await ctx.SaveChangesAsync();

        Debug.WriteLine($"Checking categories yaml file at: {DateTime.UtcNow}");
    }

    private List<CatAndSubcat> DeserializeCategoryListFromYaml(string path)
    {
        var catSubcatsFromYaml = File.ReadAllText(path);
        var deserializer = new DeserializerBuilder().Build();
        var yamlCategoryList = deserializer.Deserialize<List<CatAndSubcat>>(catSubcatsFromYaml);

        return yamlCategoryList;
    }
}