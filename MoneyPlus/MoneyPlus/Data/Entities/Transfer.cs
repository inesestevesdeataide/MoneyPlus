namespace MoneyPlus.Data.Entities;

public class Transfer : Record
{
    [Required]
    public int OriginWalletId { get; set; }
    public Wallet OriginWallet { get; set; }

    [Required]
    public int DestinationWalletId { get; set; }
    public Wallet DestinationWallet { get; set; }
}