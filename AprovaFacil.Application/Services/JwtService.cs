using AprovaFacil.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AprovaFacil.Application.Services;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public String GenerateJwtToken(IApplicationUser user, IList<String> roles)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("FullName", user.FullName ?? ""),
            new Claim("Role", user.Role ?? ""),
            new Claim("Department", user.Department ?? ""),
            new Claim("PictureUrl", user.PictureUrl ?? ""),
            new Claim("Enabled", user.Enabled.ToString().ToLower())
        };

        // Adiciona as roles
        foreach (String role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}