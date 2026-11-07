namespace EPM.TKT.App.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    string GeneratePasswordResetToken(Guid userId);
    Guid? ValidatePasswordResetToken(string token);
    DateTime GetAccessTokenExpiry();
    DateTime GetRefreshTokenExpiry();
}
