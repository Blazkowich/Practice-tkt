namespace EPM.TKT.WebApi.Contracts.Responses.Auth;

public sealed record AuthHttpResponse(
    string   AccessToken,
    string   RefreshToken,
    DateTime AccessTokenExpiry);
