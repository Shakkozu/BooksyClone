using BooksyClone.Contract.Dictionaries;
using BooksyClone.Domain.Dictionaries.FetchingAvailableServicesVariants;

namespace BooksyClone.Domain.Dictionaries;
public class DictionariesFacade
{
	private readonly FetchAvailableServiceVariants _fetchAvailableServiceVariants;

	internal DictionariesFacade(FetchAvailableServiceVariants fetchAvailableServiceVariants)
    {
		_fetchAvailableServiceVariants = fetchAvailableServiceVariants;
	}
	public async Task<IEnumerable<ServiceVariantDto>> FindAvailableServiceVariants(CancellationToken ct) =>
		await _fetchAvailableServiceVariants.Handle(ct);
}
