using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.BusinessManagement.EmployeesManagement;
using BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementFacade
{
    private readonly ConfigureServiceVariantsOfferedByBusiness _configureServiceVariantsOfferedByBusiness;
    private readonly GetBusinessConfiguration _getBusinessConfiguration;
    private readonly RegisterNewEmployee _registerNewEmployee;

    internal BusinessManagementFacade(
        ConfigureServiceVariantsOfferedByBusiness configureServiceVariantsOfferedByBusiness,
        GetBusinessConfiguration getBusinessConfiguration,
        RegisterNewEmployee registerNewEmployee)
    {
        _configureServiceVariantsOfferedByBusiness = configureServiceVariantsOfferedByBusiness;
        _getBusinessConfiguration = getBusinessConfiguration;
        _registerNewEmployee = registerNewEmployee;
    }

    public async Task<Result> ConfigureServicesOfferedByBusiness(
        BusinessServiceConfigurationDto businessServiceConfigurationDto,
        CancellationToken ct)
    {
        return await _configureServiceVariantsOfferedByBusiness.HandleAsync(businessServiceConfigurationDto, ct);
    }

    public async Task<BusinessServiceConfigurationDto?> GetBusinessConfigurationAsync(Guid businessUnitId, CancellationToken ct)
    {
        return await _getBusinessConfiguration.HandleAsync(businessUnitId, ct);
    }

    public Task<Result> RegisterEmployeeAccountUsingNewEmployeeTokenAsync(RegisterEmployeeAccountUsingNewEmployeeTokenRequest registerEmployeeAccountUsingTokenRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<RegistrationToken> RegisterNewEmployeeToBusinessAsync(RegisterNewEmployeeRequest registerNewEmployeeRequest, CancellationToken ct)
    {
        return await _registerNewEmployee.HandleAsync(registerNewEmployeeRequest, ct);
    }
}