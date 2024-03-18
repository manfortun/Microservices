using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Create JWT Token from <paramref name="username"/>
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public string GenerateJwtToken(string username)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
        };

        return GenerateJwtToken(claims);
    }

    /// <summary>
    /// Create JWT token from <paramref name="claims"/>
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        string jwtKey = _config["Jwt:SecretKey"] ?? throw new InvalidOperationException("No secret key in application settings.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return GenerateJwtToken(credentials, claims);
    }

    /// <summary>
    /// Create JWT token from <paramref name="credentials"/> and <paramref name="claims"/>
    /// </summary>
    /// <param name="credentials"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GenerateJwtToken(SigningCredentials credentials, IEnumerable<Claim> claims)
    {
        ArgumentNullException.ThrowIfNull(credentials);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? throw new InvalidOperationException("No issuer in application settings."),
            audience: _config["Jwt:Audience"] ?? throw new InvalidOperationException("No audience in application settings."),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
