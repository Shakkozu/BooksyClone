using BooksyClone.Domain.Availability;
using BooksyClone.Domain.BusinessManagement;
using BooksyClone.Domain.BusinessOnboarding;
using BooksyClone.Domain.Dictionaries;
using BooksyClone.Domain.Schedules;
using BooksyClone.Infrastructure.RabbitMQStreams;
using BooksyClone.Infrastructure.TimeManagement;
using BooksyClone.Tests.Availability;
using BooksyClone.Tests.BusinessOnboarding;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BooksyClone.Tests;

public class BooksyCloneApp : WebApplicationFactory<Program>
{
    private IServiceScope _scope;
    private bool _reuseScope;
    private readonly Action<IServiceCollection> _customization;
    private ITimeService _timeService;

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
        _timeService = A.Fake<ITimeService>();
        UpdateCurrentAppTime(DateTime.Now);

        base.ConfigureWebHost(builder);
        builder.ConfigureAppConfiguration(configurationBuilder => { });
        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<ITimeService>(_timeService);
            collection.AddSingleton<AvailabilityFixture>();
            collection.AddSingleton<OnboardingFixture>();
        });

        builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Tests");
        builder.UseSetting("Environment", "Tests");
        builder.ConfigureServices(_customization);
    }

    internal IEventPublisher GetEventPublisher => RequestScope().ServiceProvider.GetRequiredService<IEventPublisher>();
    internal SchedulesFacade SchedulesFacade => RequestScope().ServiceProvider.GetRequiredService<SchedulesFacade>();

    internal DictionariesFacade DictionariesFacade =>
        RequestScope().ServiceProvider.GetRequiredService<DictionariesFacade>();

    internal AvailabilityFacade AvailabilityFacade =>
        RequestScope().ServiceProvider.GetRequiredService<AvailabilityFacade>();

    internal AvailabilityFixture AvailabilityFixture =>
        RequestScope().ServiceProvider.GetRequiredService<AvailabilityFixture>();

    internal ITimeService ITimeService => RequestScope().ServiceProvider.GetRequiredService<ITimeService>();

    public BusinessManagementFacade BusinessManagementFacade =>
        RequestScope().ServiceProvider.GetRequiredService<BusinessManagementFacade>();

    public OnboardingFacade OnboardingFacade => RequestScope().ServiceProvider.GetRequiredService<OnboardingFacade>();

    internal OnboardingFixture OnboardingFixture =>
        RequestScope().ServiceProvider.GetRequiredService<OnboardingFixture>();
    
    internal IConfiguration Configuration => RequestScope().ServiceProvider.GetRequiredService<IConfiguration>();

    public HttpClient CreateHttpClient()
    {
        return CreateClient();
    }

    internal void UpdateCurrentAppTime(DateTime currentTime)
    {
        A.CallTo(() => _timeService.UtcNow).Returns(currentTime.ToUniversalTime());
        A.CallTo(() => _timeService.Now).Returns(currentTime);
        A.CallTo(() => _timeService.Today).Returns(currentTime.Date);
    }

    public enum UserType
    {
        User,
        Admin,
    }
}