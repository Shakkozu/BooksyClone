using Microsoft.AspNetCore.Identity;

namespace BooksyClone.Domain.Auth.FetchingMailById;

internal class GetUserEmailByUserIdQueryHandler(UserManager<IdentityUser> _userManager)
{
    public async Task<string> HandleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with email {userId} not found ");
        }

        return user.Email!;
    }
}