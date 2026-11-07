using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest;
