namespace EPM.TKT.WebApi.Contracts.Requests.Auth;

public sealed record LoginHttpRequest(string Email, string Password);
