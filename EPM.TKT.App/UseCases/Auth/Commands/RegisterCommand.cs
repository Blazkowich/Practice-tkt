using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Commands;

public sealed record RegisterCommand(
    string  Email,
    string  Password,
    string? FirstName,
    string? LastName
) : IRequest<Result<AuthResult>>;
