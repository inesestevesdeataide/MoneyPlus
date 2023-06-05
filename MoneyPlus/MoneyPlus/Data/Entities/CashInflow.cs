namespace MoneyPlus.Data.Entities;

public class CashInflow : Record
{
    public int? AssetId { get; set; }
    public Asset Asset { get; set; }

    [Required]
    public int DestinationWalletId { get; set; }
    public Wallet DestinationWallet { get; set; }
}