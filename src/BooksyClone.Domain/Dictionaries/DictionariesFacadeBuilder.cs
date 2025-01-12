using BooksyClone.Domain.Storage;
using BooksyClone.Domain.Dictionaries.FetchingAvailableServicesVariants;
using BooksyClone.Domain.Availability.Storage;
using Microsoft.Extensions.Configuration;

namespace BooksyClone.Domain.Dictionaries;

internal class DictionariesFacadeBuilder
{
	private DbConnectionFactory _dbConnectionFactory;

	public DictionariesFacadeBuilder(IConfiguration configuration)
    {
		_dbConnectionFactory = new DbConnectionFactory(configuration.GetPostgresDatabaseConnectionString());
	}

	public DictionariesFacade Build()
	{
		return new DictionariesFacade(
			   new FetchAvailableServiceVariants(_dbConnectionFactory)
			);
	}
}
