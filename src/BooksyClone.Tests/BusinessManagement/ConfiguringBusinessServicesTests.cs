using System.Net;
using System.Net.Http.Json;
using System.Text;
using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using FluentAssertions;
using Newtonsoft.Json;

namespace BooksyClone.Tests.BusinessManagement;

public class ConfiguringBusinessServicesTests
{
    private BooksyCloneApp _app;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _app = BooksyCloneApp.CreateInstance();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _app.Dispose();
    }
    
    [Test]
    public async Task ShouldReturnNotFoundForNonExistingBusiness()
    {
        var endpointRoute = $"/api/v1/companies/{Guid.NewGuid()}/services-configuration";

        var response = await _app.CreateHttpClient().GetAsync(endpointRoute);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldReturnEmptyConfigurationForExistingButNotYetConfiguredBusiness()
    {
        var businessUnitId = await _app.OnboardingFixture.ABusinessExists(Guid.NewGuid());
        var endpointRoute = $"/api/v1/companies/{businessUnitId}/services-configuration";

        var response = await _app.CreateHttpClient().GetAsync(endpointRoute);

        response.EnsureSuccessStatusCode();
        var businessConfiguration = await response.Content.ReadFromJsonAsync<BusinessServiceConfigurationDto>();
        businessConfiguration!.BusinessUnitId.Should().Be(businessUnitId);
        businessConfiguration!.OfferedServices.Should().BeEmpty();
    }

    [Test]
    public async Task ShouldConfigureServiceVariantsOfferedByBusiness()
    {
        var registeredUserId = Guid.NewGuid();
        var businessUnitId = await _app.OnboardingFixture.ABusinessExists(registeredUserId);
        var genericServiceVariants = (await _app.DictionariesFacade.FindAvailableServiceVariants(CancellationToken.None)).ToList();
        var manHaircutServiceVariantId = genericServiceVariants.Single(x => x.Name == "Strzyżenie męskie").Id;
        var beardServiceVariantId = genericServiceVariants.Single(x => x.Name == "Broda").Id;
        var hairdressingCategoryId = genericServiceVariants.Single(x => x.Name == "Strzyżenie męskie").CategoryId;
        var endpointRoute = $"/api/v1/companies/{businessUnitId}/services-configuration";
        var updateBusinessConfigurationRequest = new BusinessServiceConfigurationDto
        {
            BusinessUnitId = businessUnitId,
            OfferedServices = [
                new OfferedServiceDto
                {
                    Guid = Guid.NewGuid(),
                    Name = "Combo Standard",
                    MarkdownDescription = @"**Konsultacja specjalisty**_
*   _**Strzyżenie włosów (skin fade, klasyczne strzyżenie włosów do 10mm) oraz brody**_
*   _**Golenie brzytwą oraz golarka**_
*   _**Gorący ręcznik**_
*   _**Mycie głowy**_
*   _**Pielęgnacja kosmetykami premium**_",
                    Duration = TimeSpan.FromMinutes(90),
                    Price = Money.PLN(100.23m),
                    Order = 1,
                    EmployeeId = registeredUserId,
                    CategoryId = (long)hairdressingCategoryId,
                    GenericServiceVariantsIds = [(long)manHaircutServiceVariantId, (long)beardServiceVariantId],
                }
            ]
        };

        var content = new StringContent(JsonConvert.SerializeObject(updateBusinessConfigurationRequest), Encoding.UTF8, "application/json");
        var response = await _app.CreateHttpClient().PostAsync(endpointRoute, content);

        response.EnsureSuccessStatusCode();
        var businessConfiguration = await _app.CreateHttpClient().GetAsync($"api/v1/companies/{businessUnitId}/services-configuration");
        var businessConfigurationDto = await businessConfiguration.Content.ReadFromJsonAsync<BusinessServiceConfigurationDto>();
        businessConfigurationDto!.OfferedServices.Should().ContainEquivalentOf(updateBusinessConfigurationRequest.OfferedServices.First());
        businessConfigurationDto!.BusinessUnitId.Should().Be(businessUnitId);
        businessConfigurationDto!.OfferedServices.First().EmployeeId.Should().Be(registeredUserId);
    }
}