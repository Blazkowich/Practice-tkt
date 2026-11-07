using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EPM.TKT.App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EPM.TKT.Infrastructure.Services;

internal sealed class TokenService(IConfiguration config) : ITokenService
{
    private readonly string _secret   = config["Jwt:Secret"]   ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
    private readonly string _issuer   = config["Jwt:Issuer"]   ?? "EPM.TKT";
    private readonly string _audience = config["Jwt:Audience"] ?? "EPM.TKT.Users";
    private readonly int _accessMinutes = int.Parse(config["Jwt:AccessTokenExpiryMinutes"] ?? "15");
    private readonly int _refreshDays   = int.Parse(config["Jwt:RefreshTokenExpiryDays"]   ?? "7");
    private readonly int _resetMinutes  = int.Parse(config["Jwt:ResetTokenExpiryMinutes"]  ?? "30");

    private SymmetricSecurityKey Key => new(Encoding.UTF8.GetBytes(_secret));

    public string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject            = new ClaimsIdentity(claims),
            Expires            = GetAccessTokenExpiry(),
            Issuer             = _issuer,
            Audience           = _audience,
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public string GeneratePasswordResetToken(Guid userId)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("purpose", "password-reset")
            }),
            Expires            = DateTime.UtcNow.AddMinutes(_resetMinutes),
            Issuer             = _issuer,
            Audience           = _audience,
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    public Guid? ValidatePasswordResetToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey        = Key,
                ValidIssuer             = _issuer,
                ValidAudience           = _audience,
                ValidateLifetime        = true,
                ClockSkew               = TimeSpan.Zero
            }, out _);

            if (principal.FindFirst("purpose")?.Value != "password-reset") return null;

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
        catch { return null; }
    }

    public DateTime GetAccessTokenExpiry()  => DateTime.UtcNow.AddMinutes(_accessMinutes);
    public DateTime GetRefreshTokenExpiry() => DateTime.UtcNow.AddDays(_refreshDays);
}
