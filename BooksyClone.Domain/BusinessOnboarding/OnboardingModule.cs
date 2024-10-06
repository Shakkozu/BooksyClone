using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using BooksyClone.Infrastructure.Storage;
using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.Storage;

namespace BooksyClone.Domain.BusinessOnboarding;
public static class OnboardingModule
{
    public static void InstallOnboardingModule(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddTransient<OnboardingFacade>();
        serviceProvider.AddQueryable<BusinessDraft, SqliteDbContext>();
        serviceProvider.AddSingleton<OnboardingEventsStore>();
        serviceProvider.AddSingleton<CurrentBusinessDraftFormProjection>();

    }

    public static IEndpointRouteBuilder InstallOnbardingModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRegisterNewBusinessEndpoint();
        endpoints.MapFetchBusinessDraftEndpoint();
        return endpoints;

    }
}
