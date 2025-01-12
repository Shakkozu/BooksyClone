using BooksyClone.Contract.Dictionaries;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace BooksyClone.Tests.Dictionaries;
internal class DictionariesModuleTests
{
	private BooksyCloneApp _app;

	[OneTimeSetUp]
	public void Setup()
	{
		_app = BooksyCloneApp.CreateInstance();
	}

	[OneTimeTearDown]
	public void TearDown()
	{
		_app.Dispose();
	}

	[Test]
	public async Task ShouldFetchAvailableServices()
	{
		var expectedCategory = "Fryzjer";
		var expectedHaircutCategoryServiceVariantsNames = new List<string> {
			"Strzyżenie męskie",
			"Strzyżenie damskie",
			"Broda",
			"Fryzjer dla dzieci",
			"Koloryzacja i farbowanie włosów",
		};
		var route = "/api/v1/dictionaries/service-variants";
		var response = await _app.CreateHttpClient().GetAsync(route);

		response.EnsureSuccessStatusCode();
		var serviceVariants = await response.Content.ReadFromJsonAsync<IEnumerable<ServiceVariantDto>>();

		serviceVariants!.Where(x => x.CategoryName == expectedCategory).Select(sv => sv.Name).Should().Contain(expectedHaircutCategoryServiceVariantsNames);
	}
}
