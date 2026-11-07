using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResult>>;
