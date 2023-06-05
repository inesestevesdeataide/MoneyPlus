namespace MoneyPlus.Data.Models;

public class CatAndSubcat
{
    public string Name { get; set; }
    public string RecordType { get; set; }
    public bool IsActive { get; set; }

    public List<Subcat> Subcategories { get; set; }
}