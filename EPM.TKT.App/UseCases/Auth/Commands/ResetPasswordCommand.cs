using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Commands;

public sealed record ResetPasswordCommand(string ResetToken, string NewPassword) : IRequest<Result>;
