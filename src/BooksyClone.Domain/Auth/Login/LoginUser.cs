﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BooksyClone.Domain.Auth.Login;

public record LoginUserDto
{
	public string? Email { get; set; }
	public string? Password { get; set; }
}

public record LoginResponseDto(bool Success, string? Token, string UserId, IEnumerable<string>? Errors);

internal class LoginUserCommandHandler
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IConfiguration _configuration;

	internal LoginUserCommandHandler(UserManager<IdentityUser> userManager,
		SignInManager<IdentityUser> signInManager,
		RoleManager<IdentityRole> roleManager,
		IConfiguration configuration)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_roleManager = roleManager;
		_configuration = configuration;
	}

	public async Task<LoginResponseDto> HandleAsync(LoginUserDto userForLoginDto, CancellationToken ct)
	{
		var user = await _userManager.FindByEmailAsync(userForLoginDto.Email);

		if (user == null)
		{
			return new LoginResponseDto(false, null, null, new[] { "User not found." });
		}

		var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

		if (result.Succeeded)
		{
			var token = await GenerateJwtTokenAsync(user);
			return new LoginResponseDto(true, token, user.Id, null);
		}

		return new LoginResponseDto(false, null, null, new[] { "Invalid password." });
	}

	private async Task<string> GenerateJwtTokenAsync(IdentityUser user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
		var hasAdminRole = await _userManager.IsInRoleAsync(user, AuthorizationRoles.Administrator);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, user.Id)
			}),
			Expires = DateTime.UtcNow.AddHours(1),
			Issuer = _configuration["Jwt:Issuer"],
			Audience = _configuration["Jwt:Audience"],
			Claims = new Dictionary<string, object>
			{
				{ "email", user.Email }
			},
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		if (hasAdminRole)
		{
			tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, "Administrator"));
		}
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}