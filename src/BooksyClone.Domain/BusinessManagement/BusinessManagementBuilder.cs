using BooksyClone.Domain.Auth;
using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.BusinessManagement.EmployeesManagement;
using BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;
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
		return new BusinessManagementFacade(
			new ConfigureServiceVariantsOfferedByBusiness(new DbConnectionFactory(connectionString)),
			new GetBusinessConfiguration(new DbConnectionFactory(connectionString)),
			new RegisterNewEmployee(new DbConnectionFactory(connectionString))
		//serviceProvider.GetRequiredService<AuthFacade>()

		);
	}
}