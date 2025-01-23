using BooksyClone.Contract.BusinessManagement;
using BooksyClone.Contract.Shared;
using BooksyClone.Domain.Auth;
using BooksyClone.Domain.Auth.GettingUserIdByEmail;
using BooksyClone.Domain.Auth.RegisterUser;
using BooksyClone.Domain.BusinessManagement.ConfiguringServiceVariantsOfferedByBusiness;
using BooksyClone.Domain.BusinessManagement.EmployeesManagement;
using BooksyClone.Domain.BusinessManagement.FetchingBusinessConfiguration;
using BooksyClone.Domain.BusinessManagement.FetchingEmployeeBusinesses;

namespace BooksyClone.Domain.BusinessManagement;

public class BusinessManagementFacade
{
	private readonly ConfigureServiceVariantsOfferedByBusiness _configureServiceVariantsOfferedByBusiness;
	private readonly GetBusinessConfiguration _getBusinessConfiguration;
	private readonly RegisterNewEmployee _registerNewEmployee;
	private readonly AuthFacade _authFacade;
	private readonly AcceptEmployeeInvitationToJoiningBusiness _acceptEmployeeInvitationToJoiningBusiness;
	private readonly FetchEmployeeBusinesses _fetchEmployeeBusinesses;

	internal BusinessManagementFacade(
		ConfigureServiceVariantsOfferedByBusiness configureServiceVariantsOfferedByBusiness,
		GetBusinessConfiguration getBusinessConfiguration,
		RegisterNewEmployee registerNewEmployee,
		AuthFacade authFacade,
		AcceptEmployeeInvitationToJoiningBusiness acceptEmployeeInvitationToJoiningBusiness,
		FetchEmployeeBusinesses fetchEmployeeBusinesses
		)
	{
		_configureServiceVariantsOfferedByBusiness = configureServiceVariantsOfferedByBusiness;
		_getBusinessConfiguration = getBusinessConfiguration;
		_registerNewEmployee = registerNewEmployee;
		_authFacade = authFacade;
		_acceptEmployeeInvitationToJoiningBusiness = acceptEmployeeInvitationToJoiningBusiness;
		_fetchEmployeeBusinesses = fetchEmployeeBusinesses;
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

	public async Task<Result> RegisterEmployeeAccountUsingNewEmployeeTokenAsync(RegisterEmployeeAccountUsingNewEmployeeTokenRequest registerEmployeeAccountUsingTokenRequest)
	{
		var registerDto = new UserForRegistrationDto
		{
			ConfirmPassword = registerEmployeeAccountUsingTokenRequest.Password,
			Email = registerEmployeeAccountUsingTokenRequest.Email,
			Password = registerEmployeeAccountUsingTokenRequest.Password
		};
		var registration = await _authFacade.RegisterUserAsync(registerDto, CancellationToken.None);
		if (!registration.Success)
			return Result.ErrorResult(registration.Errors?.ToList() ?? []);

		var userId = await _authFacade.GetUserIdByEmail(new GetUserIdByEmailQuery(registerEmployeeAccountUsingTokenRequest.Email));
		return _acceptEmployeeInvitationToJoiningBusiness.AcceptInvitation(Guid.Parse(userId), registerEmployeeAccountUsingTokenRequest.Email, registerEmployeeAccountUsingTokenRequest.Token);
	}

	public async Task<RegistrationToken> RegisterNewEmployeeToBusinessAsync(RegisterNewEmployeeRequest registerNewEmployeeRequest, CancellationToken ct)
	{
		return await _registerNewEmployee.HandleAsync(registerNewEmployeeRequest, ct);
	}

	internal async Task<IEnumerable<Guid>> FetchEmployeeBusinesses(Guid userId)
	{
		return await _fetchEmployeeBusinesses.HandleAsync(userId, CancellationToken.None);
	}
}