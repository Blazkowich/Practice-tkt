namespace EPM.TKT.Domain.Options;

public sealed class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "EPM.TKT";
    public string Audience { get; set; } = "EPM.TKT.Users";
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 7;
    public int ResetTokenExpiryMinutes { get; set; } = 30;
}
