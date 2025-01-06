using Microsoft.Extensions.Configuration;

namespace BooksyClone.Tests.Auth;

internal record TestUserCredentials(string Email, string Password)
{
	internal static TestUserCredentials User => new TestUserCredentials("test_user@example.com", "Test123!");
	internal static TestUserCredentials Administrator(IConfiguration configuration) =>
		new TestUserCredentials(
			configuration["Administrator:Email"],
			configuration["Administrator:Password"]
			);
}

