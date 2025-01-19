using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Infrastructure.EmailSending;

public static class EmailSenderInstaller
{
	public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration config)
	{
		var useFakeEmailSender = bool.Parse(config.GetRequiredSection("EmailSender:UseFakeEmailSender").Value!);
		if (useFakeEmailSender)
			services.AddSingleton<IEmailSender, FakeEmailSender>();
		else
			services.AddSingleton<IEmailSender>(new EmailSender(GetEmailConfig(config)));

		return services;
	}

	public static EmailSenderConfiguration GetEmailConfig(IConfiguration config)
	{
		return new EmailSenderConfiguration
		{
			SmtpServer = config.GetSection("EmailSender:Smtp:Host").Value!,
			SmtpPort = int.Parse(config.GetSection("EmailSender:Smtp:Port").Value!),
			SmtpUser = config.GetSection("EmailSender:Smtp:Username").Value!,
			SmtpPass = config.GetSection("EmailSender:Smtp:Password").Value!,
			Ssl = bool.Parse(config.GetSection("EmailSender:Smtp:Ssl").Value!)
		};
	}
}