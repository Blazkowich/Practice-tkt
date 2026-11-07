namespace EPM.TKT.WebApi.Contracts.Auth;

public sealed record ResetPasswordHttpRequest(string ResetToken, string NewPassword);
