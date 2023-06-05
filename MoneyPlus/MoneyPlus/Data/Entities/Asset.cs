namespace MoneyPlus.Data.Entities;

public class Asset
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public double Value { get; set; }
    public bool IsActive { get; set; }

    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}