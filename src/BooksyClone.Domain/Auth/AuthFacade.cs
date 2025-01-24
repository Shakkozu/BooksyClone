using BooksyClone.Domain.Auth.FetchingMailById;
using BooksyClone.Domain.Auth.FetchingUserFromHttpContext;
using BooksyClone.Domain.Auth.GettingUserIdByEmail;
using BooksyClone.Domain.Auth.Login;
using BooksyClone.Domain.Auth.RegisterUser;

namespace BooksyClone.Domain.Auth;


public class AuthFacade
{
	private readonly RegisterUserCommandHandler _registerUserCommandHandler;
	private readonly LoginUserCommandHandler _loginUserCommandHandler;
	private readonly HttpContextUserIdProvider _fetchUserIdentifierFromContext;
	private readonly GetUserIdByEmailHandlerQueryHandler _getUserIdByEmailHandlerQueryHandler;
	private readonly GetUserEmailByUserIdQueryHandler _getUserEmailByUserIdQueryHandler;

	internal AuthFacade(
		RegisterUserCommandHandler registerUserCommandHandler,
		LoginUserCommandHandler loginUserCommandHandler,
		HttpContextUserIdProvider fetchUserIdentifierFromContext,
		GetUserIdByEmailHandlerQueryHandler getUserIdByEmailHandlerQueryHandler,
		GetUserEmailByUserIdQueryHandler getUserEmailByUserIdQueryHandler
		)
	{
		_registerUserCommandHandler = registerUserCommandHandler;
		_loginUserCommandHandler = loginUserCommandHandler;
		_fetchUserIdentifierFromContext = fetchUserIdentifierFromContext;
		_getUserIdByEmailHandlerQueryHandler = getUserIdByEmailHandlerQueryHandler;
		_getUserEmailByUserIdQueryHandler = getUserEmailByUserIdQueryHandler;
	}

	public async Task<RegistrationResponseDto> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto, CancellationToken ct)
	{
		return await _registerUserCommandHandler.HandleAsync(userForRegistrationDto, ct);
	}

	public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto userForLoginDto, CancellationToken ct)
	{
		return await _loginUserCommandHandler.HandleAsync(userForLoginDto, ct);
	}

	public Guid GetLoggedUserId()
	{
		return _fetchUserIdentifierFromContext.GetUserId();
	}

	public async Task<string> GetUserIdByEmail(GetUserIdByEmailQuery query)
	{
		return await _getUserIdByEmailHandlerQueryHandler.HandleAsync(query);
	}

	internal async Task<string> GetEmailByUserId(Guid userId)
	{
		return await _getUserEmailByUserIdQueryHandler.HandleAsync(userId.ToString());
	}
}