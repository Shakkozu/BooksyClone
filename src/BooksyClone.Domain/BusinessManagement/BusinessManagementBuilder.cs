using BooksyClone.Domain.Auth;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.BusinessManagement.EmployeesManagement;
using BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;
using BooksyClone.Domain.BusinessManagement.FetchingEmployeeBusinesses;
using BooksyClone.Domain.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementBuilder(IConfiguration configuration)
{
	public BusinessManagementFacade Build(IServiceCollection services)
	{
		var serviceProvider = services.BuildServiceProvider();

		var connectionString = configuration.GetPostgresDatabaseConnectionString();
		var dbConnectionFactory = new DbConnectionFactory(connectionString);
		return new BusinessManagementFacade(
			new ConfigureServiceVariantsOfferedByBusiness(dbConnectionFactory),
			new GetBusinessConfiguration(dbConnectionFactory),
			new RegisterNewEmployee(dbConnectionFactory),
			serviceProvider.GetRequiredService<AuthFacade>(),
			new AcceptEmployeeInvitationToJoiningBusiness(dbConnectionFactory),
			new FetchEmployeeBusinesses(dbConnectionFactory)

		);
	}
}