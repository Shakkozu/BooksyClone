using BooksyClone.Domain.Auth.Login;
using BooksyClone.Domain.Auth.RegisterUser;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace BooksyClone.Tests.Auth;

[TestFixture]
public class AuthorizationTests
{
	private BooksyCloneApp _app;

	[OneTimeSetUp]
	public void Setup()
	{
		_app = BooksyCloneApp.CreateInstance();
	}

	[OneTimeTearDown]
	public void TearDown()
	{
		_app.Dispose();
	}


	[Test]
	public async Task ShouldRegisterUser()
	{
		var client = _app.CreateClient();
		var user = new UserForRegistrationDto
		{
			Email = $"{Guid.NewGuid()}@example.com",
			Password = "zaq1@WSX",
			ConfirmPassword = "zaq1@WSX"
		};
		var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

		var response = await client.PostAsync("/api/accounts/register", content);

		var responseContent = await response.Content.ReadAsStringAsync();
		var registrationResponse = JsonConvert.DeserializeObject<RegistrationResponseDto>(responseContent);
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		registrationResponse.Errors.Should().BeNull();
		registrationResponse.Success.Should().BeTrue();
	}

	[Test]
	public async Task ShouldCreateRole()
	{
		var role = Guid.NewGuid().ToString();
		var rolesManager = _app.Services.GetRequiredService<RoleManager<IdentityRole>>();
		var roleExists = await rolesManager.RoleExistsAsync(role);

		var result = await rolesManager.CreateAsync(new IdentityRole(role));

		Assert.AreEqual(false, roleExists);
		Assert.IsTrue(result.Succeeded);
	}

	[Test]
	public async Task ShouldNotRegisterUserWithNotProperlyFilledRegistrationDto()
	{
		var client = _app.CreateClient();
		var user = new UserForRegistrationDto
		{
			Email = "notValidEmail",
			Password = "zaq1@WSX",
			ConfirmPassword = "XSW@1qaz"
		};
		var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

		var response = await client.PostAsync("/api/accounts/register", content);

		var responseContent = await response.Content.ReadAsStringAsync();
		var registrationResponse = JsonConvert.DeserializeObject<RegistrationResponseDto>(responseContent);
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		registrationResponse.Success.Should().BeFalse();
		registrationResponse.Errors.Should().NotBeNullOrEmpty();
		registrationResponse.Errors.Should().Contain("The password and confirmation password do not match.");
	}

	[Test]
	public async Task ShouldRegisterAndAuthorizeUser()
	{
		var client = _app.CreateClient();
		var user = new UserForRegistrationDto
		{
			Email = $"{Guid.NewGuid()}@example.com",
			Password = "zaq1@WSX",
			ConfirmPassword = "zaq1@WSX"
		};
		var loginDto = new LoginUserDto
		{
			Email = user.Email,
			Password = user.Password
		};
		await client.PostAsync("/api/accounts/register", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

		var loginResponse = await client.PostAsync("/api/accounts/login", new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json"));

		var responseContent = await loginResponse.Content.ReadAsStringAsync();
		var loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(responseContent);
		loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
		loginResponseDto.Errors.Should().BeNull();
		loginResponseDto.Success.Should().BeTrue();
		loginResponseDto.Token.Should().NotBeNullOrEmpty();

		var request = new HttpRequestMessage(HttpMethod.Get, "/api/accounts/restricted-resource");
		request.Headers.Add("Authorization", $"Bearer {loginResponseDto.Token}");
		var response = await client.SendAsync(request);
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Test]
	public async Task UnauthorizedUserCannotReachProtectedEndpoint()
	{
		var client = _app.CreateClient();
		var request = new HttpRequestMessage(HttpMethod.Get, "api/accounts/restricted-resource");

		var response = await client.SendAsync(request);

		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}
}