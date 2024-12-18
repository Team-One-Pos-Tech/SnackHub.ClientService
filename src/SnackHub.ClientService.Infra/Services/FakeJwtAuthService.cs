using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SnackHub.ClientService.Domain.Contracts;
using SnackHub.ClientService.Domain.Models.Gateways;

namespace SnackHub.ClientService.Infra.Services;

public class FakeJwtAuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public FakeJwtAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AuthResponseType> Execute(SignInRequest request)
    {
        var serviceKey = _configuration["Auth:Key"] ?? throw new ApplicationException("JWT key is not configured.");
        
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceKey));
        var issuer = _configuration["Auth:Issuer"];
        var audience = _configuration["Auth:Audience"];

        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokeOptions = new JwtSecurityToken(
            issuer,
            audience,
            new List<Claim>(),
            expires: DateTime.Now.AddMinutes(2),
            signingCredentials: signinCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

        return new AuthResponseType(tokenString, true);
    }
}