namespace EPM.TKT.WebApi.Contracts.Auth;

public sealed record AuthHttpResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry);
