namespace MoneyPlus.Data.Models;

public abstract class Record
{
    [Required]
    public int Id { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    [Required]
    public double Amount { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    [Required]
    public int SubcategoryId { get; set; }
    public Subcategory Subcategory { get; set; }
}