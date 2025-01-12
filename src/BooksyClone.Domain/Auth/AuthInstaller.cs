using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BooksyClone.Domain.Auth.RegisterUser;
using BooksyClone.Domain.Auth.Login;
using BooksyClone.Domain.Auth.GettingUserIdByEmail;
using BooksyClone.Domain.Auth.FetchingUserFromHttpContext;
using Microsoft.AspNetCore.Routing;
using BooksyClone.Domain.Auth.LoggingOut;
using BooksyClone.Domain.Auth.RestrictedResource;

namespace BooksyClone.Domain.Auth;

internal record JwtSettings
{
	public string Secret { get; init; }
	public string Issuer { get; init; }
	public string Audience { get; init; }
	public int TokenExpirationInMinutes { get; init; }
}
public static class AuthInstaller
{
	public static IServiceCollection InstallAuthModule(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthorization();
		services.AddTransient<RegisterUserCommandHandler>();
		services.AddTransient<LoginUserCommandHandler>();
		services.AddTransient<GetUserIdByEmailHandlerQueryHandler>();
		services.AddDbContext<AuthorizationDbContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString("Postgres"));
		});
		services.AddIdentityApiEndpoints<IdentityUser>()
		   .AddRoles<IdentityRole>()
		   .AddEntityFrameworkStores<AuthorizationDbContext>();
		var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings.Issuer,
				ValidAudience = jwtSettings.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret))
			};
		});
		services.AddScoped<HttpContextAccessor>();
		services.AddScoped<IFetchUserIdentifierFromContext>(provider =>
		{
			var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
			return new HttpContextUserIdProvider(httpContextAccessor);
		});

		return services;
	}

	public static IEndpointRouteBuilder MapAuthenticationModuleEndpoints(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapLoginEndpoint();
		endpoints.MapLogoutEndpoint();
		endpoints.MapRegisterUserEndpoint();
		endpoints.MapGetRestrictedResourceEndpoint();

		return endpoints;
	}
}
