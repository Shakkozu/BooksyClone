using BooksyClone.Domain.Storage;
using Microsoft.Extensions.Configuration;

namespace BooksyClone.Domain.BusinessOnboarding;

internal class OnboardingBuilder(IConfiguration configuration, IOnboardingEventsPublisher onboardingEventsPublisher)
{
    internal OnboardingFacade Build()
    {
        var dbContext = new PostgresDbContext(configuration.GetPostgresDatabaseConnectionString());
        return new OnboardingFacade(dbContext, onboardingEventsPublisher);
    }
    
}