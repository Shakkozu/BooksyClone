using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using BooksyClone.Domain.BusinessOnboarding.FetchingBusinessCreationApplication;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using BooksyClone.Infrastructure.Storage;
using BooksyClone.Domain.BusinessOnboarding.Model;
using BooksyClone.Domain.Storage;
using BooksyClone.Infrastructure.RabbitMQStreams.Producing;
using Microsoft.Extensions.Configuration;

namespace BooksyClone.Domain.BusinessOnboarding;
public static class OnboardingModule
{
    public static void InstallOnboardingModule(this IServiceCollection serviceProvider, IConfiguration configuration)
    {
        serviceProvider.AddTransient<OnboardingFacade>();
        serviceProvider.AddQueryable<BusinessDraft, PostgresDbContext>();
        serviceProvider.AddSingleton<IOnboardingEventsPublisher, OnboardingRabbitStreamsEventsPublisher>(_ => new OnboardingRabbitStreamsEventsPublisher(GetProducerConfiguration(configuration)));
    }

    public static IEndpointRouteBuilder InstallOnbardingModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRegisterNewBusinessEndpoint();
        endpoints.MapFetchBusinessDraftEndpoint();
        return endpoints;

    }

    private static RabbitMQStreamProducerConfiguration GetProducerConfiguration(IConfiguration configuration)
    {
        var streamName = configuration.GetValue<string>("Onboarding:RabbitMQ:Producer:StreamName");
        var username = configuration.GetValue<string>("Onboarding:RabbitMQ:Producer:Username");
        var password = configuration.GetValue<string>("Onboarding:RabbitMQ:Producer:Password");
        var produerRef = configuration.GetValue<string>("Onboarding:RabbitMQ:Producer:ProducerReferece");
        var rabbitmqAddress = configuration.GetValue<string>("Onboarding:RabbitMQ:Producer:RabbitMQAddress");
        var rabbitmqPort = int.Parse(configuration.GetValue<string>("Onboarding:RabbitMQ:Producer:RabbitMQPort") ?? throw new ArgumentException());
        var businessDraftConsumer = new RabbitMQStreamProducerConfiguration
        (
            streamName ?? throw new ArgumentNullException(streamName),
            username ?? throw new ArgumentNullException(username),
            password ?? throw new ArgumentNullException(password),
            produerRef ?? throw new ArgumentNullException(produerRef),
            rabbitmqAddress ?? throw new ArgumentNullException(rabbitmqAddress),
            rabbitmqPort
            );
        return businessDraftConsumer;
    }
}
