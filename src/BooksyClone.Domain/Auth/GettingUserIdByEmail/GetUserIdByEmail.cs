using Microsoft.AspNetCore.Identity;

namespace BooksyClone.Domain.Auth.GettingUserIdByEmail;
public record GetUserIdByEmailQuery(string Email);

internal class GetUserIdByEmailHandlerQueryHandler(UserManager<IdentityUser> _userManager)
{
	public async Task<string> HandleAsync(GetUserIdByEmailQuery query)
	{
		var user = await _userManager.FindByEmailAsync(query.Email);
		if (user == null)
		{
			throw new InvalidOperationException($"User with email {query.Email} not found ");
		}

		return user.Id;
	}
}
