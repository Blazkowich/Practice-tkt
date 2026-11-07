using EPM.TKT.App.UseCases.Auth.Commands;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPM.TKT.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName), ct);

        return result.Match(
            onSuccess: auth  => Ok(Map(auth)),
            onFailure: error => Problem(error));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password), ct);

        return result.Match(
            onSuccess: auth  => Ok(Map(auth)),
            onFailure: error => Problem(error));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        return result.Match(
            onSuccess: auth  => Ok(Map(auth)),
            onFailure: error => Problem(error));
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordHttpRequest request, CancellationToken ct)
    {
        var resetToken = await sender.Send(new ForgotPasswordCommand(request.Email), ct);
        // In production: send via email and return 200 OK with no body.
        return Ok(new { ResetToken = resetToken });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ResetPasswordCommand(request.ResetToken, request.NewPassword), ct);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: error => Problem(error));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenHttpRequest request, CancellationToken ct)
    {
        await sender.Send(new LogoutCommand(request.RefreshToken), ct);
        return NoContent();
    }

    private static AuthHttpResponse Map(AuthResult r)
        => new(r.AccessToken, r.RefreshToken, r.AccessTokenExpiry);

    private IActionResult Problem(Error error) => error.Type switch
    {
        ErrorType.Unauthorized => Unauthorized(new { error.Message }),
        ErrorType.NotFound     => NotFound(new { error.Message }),
        ErrorType.Conflict     => Conflict(new { error.Message }),
        _                      => BadRequest(new { error.Message })
    };
}
