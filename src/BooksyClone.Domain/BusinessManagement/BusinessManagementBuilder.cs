using BooksyClone.Domain.Availability.Storage;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.BusinessManagement.EmployeesManagement;
using BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;
using BooksyClone.Domain.Storage;
using BooksyClone.Infrastructure.EmailSending;
using Microsoft.Extensions.Configuration;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementBuilder(IConfiguration configuration)
{
    public BusinessManagementFacade Build()
    {
        var connectionString = configuration.GetPostgresDatabaseConnectionString();
        return new BusinessManagementFacade(
            new ConfigureServiceVariantsOfferedByBusiness(new DbConnectionFactory(connectionString)),
            new GetBusinessConfiguration(new DbConnectionFactory(connectionString)),
            new RegisterNewEmployee(new DbConnectionFactory(connectionString))
        );
    }

    public BusinessManagementFacade Build(IConfiguration configuration1)
    {
        var connectionString = configuration.GetPostgresDatabaseConnectionString();
        return new BusinessManagementFacade(
            new ConfigureServiceVariantsOfferedByBusiness(new DbConnectionFactory(connectionString)),
            new GetBusinessConfiguration(new DbConnectionFactory(connectionString)),
            new RegisterNewEmployee(new DbConnectionFactory(connectionString))
            );
    }
}