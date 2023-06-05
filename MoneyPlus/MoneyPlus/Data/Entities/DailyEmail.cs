namespace MoneyPlus.Data.Entities;

public class DailyEmail
{
    public int Id { get; set; }

    public string EmailSender { get; set; }
    public string Password { get; set; }
    public string EmailServerAddress { get; set; }
    public string EmailServerPort { get; set; }
    public bool UseSsl { get; set; }

    public string EmailRecipient { get; set; }
    public double Wealth { get; set; }
    public string EmailSubject { get; set; }
    public string EmailBody { get; set; }
    public DateTime Date { get; set; }

    public string UserId { get; set; }
    public IdentityUser User { get; set; }
}