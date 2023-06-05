namespace MoneyPlus.Interfaces;

public interface IEmailService
{
	Task SendEmailAsync(string email, string subject
		, string emailBody, string emailBodyHtml, DateTime date);
}