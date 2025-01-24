using Microsoft.AspNetCore.Http;

namespace BooksyClone.Domain.Auth.FetchingUserFromHttpContext;

internal class HttpContextUserIdProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public HttpContextUserIdProvider(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public Guid GetUserId()
	{
		var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
		if (userId == null)
		{
			throw new UnauthorizedAccessException();
		}
		return Guid.Parse(userId);
	}
}