namespace MoneyPlus.Data.Models;

public class MonthlyCashOutflow
{
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    
    public int PayeeId { get; set; }
    public string Payee { get; set; }
    
    public double Amount { get; set; }
    public string Description { get; set; }
    
    public int? AssetId { get; set; }
    public string Asset { get; set; }
    
    public int? SubcategoryId { get; set; }
    public string Subcategory { get; set; }
    
    public int? CategoryId { get; set; }
    public string Category { get; set; }
    
    public int OriginWalletId { get; set; }
    public string OriginWallet { get; set; }
}
