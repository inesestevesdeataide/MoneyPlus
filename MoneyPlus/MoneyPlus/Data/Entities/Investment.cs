namespace MoneyPlus.Data.Entities;

public class Investment : Record
{
    public int PayeeId { get; set; }
    public Payee Payee { get; set; }

    [Required]
    public int OriginWalletId { get; set; }
    public Wallet OriginWallet { get; set; }

    [Required]
    public int DestinationWalletId { get; set; }
    public Wallet DestinationWallet { get; set; }
}