﻿using BooksyClone.Domain.Auth;
using BooksyClone.Domain.Auth.GettingUserIdByEmail;
using BooksyClone.Domain.Auth.Login;
using BooksyClone.Domain.Auth.RegisterUser;
using Microsoft.Extensions.Configuration;

namespace BooksyClone.Tests.Auth;

public class AuthFixture(AuthFacade _authFacade,
	IConfiguration _configuration)
{
	internal async Task<string> GetAuthenticationTokenForSuperUserAsync()
	{
		var email = TestUserCredentials.Administrator(_configuration).Email;
		var password = TestUserCredentials.Administrator(_configuration).Password;

		var authenticationResult = await _authFacade.LoginUserAsync(new LoginUserDto
		{
			Email = email,
			Password = password
		}, CancellationToken.None);
		if (!authenticationResult.Success)
		{
			throw new Exception("Failed to authenticate user");
		}

		return authenticationResult.Token!;
	}

	internal async Task<string> GetAuthenticationTokenForUserAsync()
	{
		var email = TestUserCredentials.User.Email;
		var password = TestUserCredentials.User.Password;
		await AssertThatUserIsRegistred(email, password);

		var authenticationResult = await _authFacade.LoginUserAsync(new LoginUserDto
		{
			Email = email,
			Password = password
		}, CancellationToken.None);
		if (!authenticationResult.Success)
		{
			throw new Exception("Failed to authenticate user");
		}

		return authenticationResult.Token!;
	}

	private async Task AssertThatUserIsRegistred(string email, string password)
	{
		var userAlreadyExists = false;
		try
		{
			userAlreadyExists = !string.IsNullOrEmpty(await _authFacade.GetUserIdByEmail(new GetUserIdByEmailQuery(email)));
		}
		catch { }

		if (!userAlreadyExists)
		{
			var dto = new UserForRegistrationDto { Email = email, Password = password, ConfirmPassword = password };
			await _authFacade.RegisterUserAsync(dto, CancellationToken.None);
		}
	}

	internal async Task<Guid> GetSuperuserIdAsync()
	{
		var email = TestUserCredentials.Administrator(_configuration).Email;
		var password = TestUserCredentials.Administrator(_configuration).Password;
		await AssertThatUserIsRegistred(email, password);
		var result = await _authFacade.GetUserIdByEmail(new GetUserIdByEmailQuery(TestUserCredentials.Administrator(_configuration).Email));
		return Guid.Parse(result);
	}

	internal async Task<Guid> GetUserIdAsync()
	{
		await AssertThatUserIsRegistred(TestUserCredentials.User.Email, TestUserCredentials.User.Password);
		var result = await _authFacade.GetUserIdByEmail(new GetUserIdByEmailQuery(TestUserCredentials.User.Email));
		return Guid.Parse(result);
	}
}