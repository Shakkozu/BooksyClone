
using Serilog;
using BooksyClone.Domain.BusinessOnboarding;
using BooksyClone.Domain.Storage;
using BooksyClone.Domain.Schedules;
using BooksyClone.Infrastructure.Migrations;
using BooksyClone.Domain.Availability;
using BooksyClone.Domain.Auth;

namespace BooksyClone;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();
        var config = builder.Configuration;
        var connectionString = config.GetValue<string>("Availability:Postgres:ConnectionString");
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException("Could not read the connectionstring under key `Availability:Postgres:ConnectionString` ");

        builder.Services.AddSerilog(Log.Logger);
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.InstallPostgresEntityFramework(connectionString);
        builder.Services.InstallOnboardingModule(config);
        builder.Services.InstallSchedulesModule(config);
        builder.Services.InstallAvailabilityModule(config);
        builder.Services.InstallAuthModule(config);
        builder.Services.AddAntiforgery();
		builder.Host.UseSerilog(Log.Logger);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var serviceScope = app.Services.CreateScope())
        {
			MigrationsRunner.RunMigrations(connectionString);
        }

        app.InstallOnbardingModuleEndpoints();
        app.InstallSchedulesModuleEndpoints();
        app.InstallAvailabilityModuleEndpoints();
		app.MapAuthenticationModuleEndpoints();

		app.UseHttpsRedirection();

		app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();


        app.MapControllers();

        app.Run();
    }
}
