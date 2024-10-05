
using Serilog;
using BooksyClone.Domain.BusinessOnboarding;
using BooksyClone.Domain.Storage;
using SQLitePCL;

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
        builder.Services.AddSerilog(Log.Logger);
        //builder.Logging.AddSerilog();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.InstallSqliteEntityFramework();
        builder.Services.InstallOnboardingModule();
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
            var context = serviceScope.ServiceProvider.GetRequiredService<SqliteDbContext>();
            context.MigrateAsync().GetAwaiter().GetResult();
        }

        app.InstallOnbardingModuleEndpoints();

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseAntiforgery();


        app.MapControllers();

        app.Run();
    }
}
