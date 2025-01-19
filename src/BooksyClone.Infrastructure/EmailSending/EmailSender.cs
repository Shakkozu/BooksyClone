using MailKit.Net.Imap;
using System.Net;
using System.Net.Mail;

namespace BooksyClone.Infrastructure.EmailSending;

public record EmailSenderConfiguration
{
	public string SmtpServer { get; init; }
	public int SmtpPort { get; init; }
	public string SmtpUser { get; init; }
	public string SmtpPass { get; init; }
	public bool Ssl { get; init; }
}

public interface IEmailSender
{
	void SendEmail(string recipient, string subject, string message);
}
public class EmailSender(EmailSenderConfiguration config) : IEmailSender
{
	private readonly string _smtpServer = config.SmtpServer;
	private readonly int _smtpPort = config.SmtpPort;
	private readonly string _smtpUser = config.SmtpUser;
	private readonly string _smtpPass = config.SmtpPass;

	public void SendEmail(string recipient, string subject, string message)
	{
		var mailMessage = new MailMessage
		{
			From = new MailAddress(_smtpUser),
			Subject = subject,
			Body = message,
			IsBodyHtml = true,
		};
		mailMessage.To.Add(recipient);

		using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
		{
			Credentials = new NetworkCredential(_smtpUser, _smtpPass),
			EnableSsl = config.Ssl,
		};

		smtpClient.Send(mailMessage);
	}
}

public class FakeEmailSender : IEmailSender
{
	public void SendEmail(string recipient, string subject, string message)
	{
		Console.WriteLine($"Sending email to {recipient} with subject {subject} and message {message}");
	}
}

public record EmailMessage(string Subject, string From, DateTime Date, string Body);


