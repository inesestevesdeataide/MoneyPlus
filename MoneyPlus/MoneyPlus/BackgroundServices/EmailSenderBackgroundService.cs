//Não estava a conseguir finalizar esta opção (R10).
//Daí a existência deste código e da pasta Interfaces.
//Apaguei, recomecei mas segui a sugestão de fazer os
//registos numa tabela => FakeEmailSenderBackgroundService.cs

//using System;
//using MailKit.Net.Smtp;
//using Microsoft.Extensions.Options;
//using MimeKit;
//using MoneyPlus.Interfaces;

//namespace MoneyPlus.BackgroundServices;

//public class EmailSenderBackgroundService : IEmailService
//{
//    private readonly EmailConfiguration _emailConfiguration;

//    public EmailSenderBackgroundService(IOptions<EmailConfiguration> emailConfiguration)
//    {
//        _emailConfiguration = emailConfiguration.Value;
//    }

//    public async Task SendEmailAsync(string email, string subject, string emailBody, string emailBodyHtml, DateTime date)
//    {
//        var message = new MimeMessage();
//        message.From.Add(new MailboxAddress(_emailConfiguration.EmailSender, _emailConfiguration.EmailRecipient));
//        message.To.Add(MailboxAddress.Parse(email));
//        message.Subject = subject;

//        var builder = new BodyBuilder { TextBody = emailBody, HtmlBody = emailBodyHtml };
//        message.Body = builder.ToMessageBody();

//        try
//        {
//            var smtpClient = new SmtpClient();
//            smtpClient.ServerCertificateValidationCallback = (s, c, f, e) => true;
//            await smtpClient.ConnectAsync(_emailConfiguration.EmailServerAddress).ConfigureAwait(false);

//            await smtpClient.AuthenticateAsync(_emailConfiguration.EmailSender, _emailConfiguration.Password).ConfigureAwait(false);
//            await smtpClient.SendAsync(message).ConfigureAwait(false);
//            await smtpClient.DisconnectAsync(true).ConfigureAwait(false);
//        }
//        catch (Exception e)
//        {
//            throw new InvalidOperationException(e.Message);
//        }
//    }
//}