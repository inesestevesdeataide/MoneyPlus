namespace MoneyPlus.Data.Entities;

public class Wallet
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public double Balance { get; set; }

    public string UserId { get; set; }
    public IdentityUser User { get; set; }

    public bool IsActive { get; set; }
}