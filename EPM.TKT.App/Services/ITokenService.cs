namespace EPM.TKT.App.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken(Guid userId);
    string GeneratePasswordResetToken(Guid userId);
    Guid? ValidateRefreshToken(string token);
    Guid? ValidatePasswordResetToken(string token);
    DateTime GetAccessTokenExpiry();
}
