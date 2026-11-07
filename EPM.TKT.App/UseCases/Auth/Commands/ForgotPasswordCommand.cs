using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Commands;

// Returns the reset token (dev convenience). In production send via email instead.
public sealed record ForgotPasswordCommand(string Email) : IRequest<string?>;
