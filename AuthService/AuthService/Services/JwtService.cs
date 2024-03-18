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

    public string GenerateJwtToken(string username)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
        };

        return GenerateJwtToken(claims);
    }

    public string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        string jwtKey = _config["Jwt:SecretKey"] ?? throw new InvalidOperationException("No secret key in application settings.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return GenerateJwtToken(credentials, claims);
    }

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

    public string GetIdFromJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null)
            throw new ArgumentException("Invalid JWT token");

        var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "sub" || claim.Type == "id");

        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            throw new InvalidOperationException("User ID claim not found in token");

        return userIdClaim.Value;
    }
}
