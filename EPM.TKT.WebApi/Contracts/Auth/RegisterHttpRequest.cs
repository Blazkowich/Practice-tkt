namespace EPM.TKT.WebApi.Contracts.Auth;

public sealed record RegisterHttpRequest(
    string Email,
    string Password,
    string? FirstName,
    string? LastName);
