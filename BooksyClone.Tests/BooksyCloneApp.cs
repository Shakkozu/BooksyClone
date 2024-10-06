



using BooksyClone.Domain.Schedules;
using BooksyClone.Infrastructure.RabbitMQStreams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BooksyClone.Tests;

public class BooksyCloneApp : WebApplicationFactory<Program>
{
    private IServiceScope _scope;
    private bool _reuseScope;
    private string _token;
    private readonly Action<IServiceCollection> _customization;
    private BooksyCloneApp(Action<IServiceCollection> customization, bool reuseScope = false)
    {
        _customization = customization;
        _reuseScope = reuseScope;
        _scope = base.Services.CreateAsyncScope();
    }

    public static BooksyCloneApp CreateInstance(bool reuseScope = false)
    {
        var omniomApp = new BooksyCloneApp(_ => { }, reuseScope);
        return omniomApp;
    }

    public static BooksyCloneApp CreateInstance(Action<IServiceCollection> customization, bool reuseScope = false)
    {
        var omniomApp = new BooksyCloneApp(customization, reuseScope);
        return omniomApp;
    }

    private IServiceScope RequestScope()
    {
        if (!_reuseScope)
        {
            _scope.Dispose();
            _scope = Services.CreateAsyncScope();
        }
        return _scope;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {

        });
        builder.ConfigureServices(collection =>
        {

        });

        builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Automated_Tests");
        builder.UseSetting("Environment", "Automated_Tests");
        builder.ConfigureServices(_customization);
    }

    internal IEventPublisher GetEventPublisher => RequestScope().ServiceProvider.GetRequiredService<IEventPublisher>();
    internal SchedulesFacade SchedulesFacade => RequestScope().ServiceProvider.GetRequiredService<SchedulesFacade>();

    public HttpClient CreateHttpClient()
    {
        return CreateClient();
    }

    public enum UserType
    {
        User,
        Admin,
    }
}