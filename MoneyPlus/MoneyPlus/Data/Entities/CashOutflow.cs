namespace MoneyPlus.Data.Entities;

public class CashOutflow : Record
{
    public int? AssetId { get; set; }
    public Asset Asset { get; set; }

    [Required]
    public int PayeeId { get; set; }
    public Payee Payee { get; set; }

    [Required]
    public int OriginWalletId { get; set; }
    public Wallet OriginWallet { get; set; }
}