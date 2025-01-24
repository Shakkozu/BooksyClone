using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Infrastructure.EmailSending;

public static class EmailSenderInstaller
{
	public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration config)
	{
		var emailConfig = GetEmailConfiguration(config);
		if (emailConfig.UseFakeEmailSender)
			services.AddSingleton<IEmailSender, FakeEmailSender>();
		else
			services.AddSingleton<IEmailSender>(new EmailSender(GetEmailSmtpConfig(config)));

		return services;
	}


	public static EmailConfiguration GetEmailConfiguration(this IConfiguration configuration)
	{
		return new EmailConfiguration
		{
			Smtp = GetEmailSmtpConfig(configuration),
			UseFakeEmailSender = bool.Parse(configuration.GetRequiredSection("EmailSender:UseFakeEmailSender").Value!)
		};
	}
	public static EmailSenderSmtpConfiguration GetEmailSmtpConfig(this IConfiguration config)
	{
		return new EmailSenderSmtpConfiguration
		{
			SmtpServer = config.GetSection("EmailSender:Smtp:Host").Value!,
			SmtpPort = int.Parse(config.GetSection("EmailSender:Smtp:Port").Value!),
			SmtpUser = config.GetSection("EmailSender:Smtp:Username").Value!,
			SmtpPass = config.GetSection("EmailSender:Smtp:Password").Value!,
			Ssl = bool.Parse(config.GetSection("EmailSender:Smtp:Ssl").Value!)
		};
	}
}