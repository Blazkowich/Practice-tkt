namespace EPM.TKT.WebApi.Contracts.Requests.Auth;

public sealed record ResetPasswordHttpRequest(string ResetToken, string NewPassword);
