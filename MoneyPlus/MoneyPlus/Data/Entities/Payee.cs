namespace MoneyPlus.Data.Entities;

public class Payee
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public int? TaxNumber { get; set; }

    public bool IsActive { get; set; }

    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}