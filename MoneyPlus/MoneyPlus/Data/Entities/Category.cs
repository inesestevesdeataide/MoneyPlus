namespace MoneyPlus.Data.Entities;

public class Category
{
    public int Id { get; set; }
    [Required]
    public string RecordType { get; set; }
    [Required]
    public string Name { get; set; }
    public bool IsActive { get; set; }
}