namespace MoneyPlus.Data.Entities;

public class Subcategory
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}