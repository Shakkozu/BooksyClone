using System.Net.Http.Json;
using System.Text;
using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.BusinessOnboarding.RegisteringANewBusiness;
using BooksyClone.Tests.BusinessOnboarding;
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
    public async Task ShouldConfigureServiceVariantsOfferedByBusiness()
    {
        var registeredUserId = Guid.NewGuid();
        var businessUnitId = await _app.OnboardingFixture.ABusinessExists(registeredUserId);
        var genericServiceVariants = (await _app.DictionariesFacade.FindAvailableServiceVariants(CancellationToken.None)).ToList();
        var manHaircutServiceVariantId = genericServiceVariants.Single(x => x.Name == "Strzyżenie męskie").Id;
        var beardServiceVariantId = genericServiceVariants.Single(x => x.Name == "Broda").Id;
        var hairdressingCategoryId = genericServiceVariants.Single(x => x.Name == "Strzyżenie męskie").CategoryId;
        var endpointRoute = $"/api/v1/companies/{businessUnitId}/business-services-configuration";
        var updateBusinessConfigurationRequest = new BusinessConfigurationDto
        {
            BusinessUnitId = businessUnitId,
            EmployeeId = registeredUserId,
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
                    Price = Money.PLN(140),
                    Order = 1,
                    EmployeesOfferingService = [registeredUserId],
                    CategoryId = (ulong)hairdressingCategoryId,
                    GenericServiceVariantsIds = [(ulong)manHaircutServiceVariantId, (ulong)beardServiceVariantId],
                }
            ]
        };
        
        var content = new StringContent(JsonConvert.SerializeObject(updateBusinessConfigurationRequest), Encoding.UTF8, "application/json");
        var response = await _app.CreateHttpClient().PostAsync(endpointRoute, content);
        
        response.EnsureSuccessStatusCode();
        var businessConfiguration = await _app.CreateHttpClient().GetAsync($"api/v1/companies/{businessUnitId}/business-services-configuration");
        var businessConfigurationDto = await businessConfiguration.Content.ReadFromJsonAsync<BusinessConfigurationDto>();
        businessConfigurationDto!.OfferedServices.Should().ContainEquivalentOf(updateBusinessConfigurationRequest.OfferedServices.First());
        businessConfigurationDto!.BusinessUnitId.Should().Be(businessUnitId);
        businessConfigurationDto!.EmployeeId.Should().Be(registeredUserId);
    }
}