

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

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
            //configurationBuilder.AddInMemoryCollection();
        });
        builder.ConfigureServices(collection =>
        {

        });

        builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Automated_Tests");
        builder.UseSetting("Environment", "Automated_Tests");
        builder.ConfigureServices(_customization);
    }

    public HttpClient CreateHttpClient()
    {
        return CreateClient();
    }

    //public async Task<HttpClient> CreateHttpClientWithAuthorizationAsync(UserType userType = UserType.User)
    //{
    //    var client = CreateClient();
    //    if (!string.IsNullOrEmpty(_token))
    //    {
    //        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
    //        return client;
    //    }

    //    string? authToken;
    //    switch (userType)
    //    {
    //        case UserType.Admin:
    //            authToken = await RequestScope().ServiceProvider.GetRequiredService<AuthFixture>().GetAuthenticationTokenForSuperUserAsync();
    //            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
    //            break;
    //        case UserType.User:
    //            authToken = await RequestScope().ServiceProvider.GetRequiredService<AuthFixture>().GetAuthenticationTokenForUserAsync();
    //            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
    //            break;
    //    }
    //    return client;
    //}

    internal CreateProductCommandHandler CreateProductCommandHandler => RequestScope().ServiceProvider.GetRequiredService<CreateProductCommandHandler>();
    internal SearchProductsQueryHandler SearchProductsQueryHandler => RequestScope().ServiceProvider.GetRequiredService<SearchProductsQueryHandler>();
    internal ImportProductsToCatalogue ProductCatalogueImportHandler => RequestScope().ServiceProvider.GetRequiredService<ImportProductsToCatalogue>();
    internal ProductsTestsFixture ProductsTestsFixture => RequestScope().ServiceProvider.GetRequiredService<ProductsTestsFixture>();
    internal MealsTestsFixture MealsTestsFixture => RequestScope().ServiceProvider.GetRequiredService<MealsTestsFixture>();
    internal AuthFixture AuthFixture => RequestScope().ServiceProvider.GetRequiredService<AuthFixture>();

    internal ICommandHandler<SaveMealNutritionEntriesCommand> AddNutritionEntriesCommandHandler => RequestScope().ServiceProvider.GetRequiredService<ICommandHandler<SaveMealNutritionEntriesCommand>>();
    internal GetNutritionDayQueryHandler GetDiaryQueryHandler => RequestScope().ServiceProvider.GetRequiredService<GetNutritionDayQueryHandler>();
    internal GetShortSummaryForDaysQueryHandler GetShortSummaryForDaysQueryHandler => RequestScope().ServiceProvider.GetRequiredService<GetShortSummaryForDaysQueryHandler>();
    internal ICommandHandler<CleanupNutritionistModuleCommand> CleanupNutritionistModuleCommandHandler => RequestScope().ServiceProvider.GetRequiredService<ICommandHandler<CleanupNutritionistModuleCommand>>();
    internal ICommandHandler<RegisterNutritionistCommand> RegisterNutritionistCommandHandler => RequestScope().ServiceProvider.GetRequiredService<ICommandHandler<RegisterNutritionistCommand>>();

    public enum UserType
    {
        User,
        Admin,
    }
}