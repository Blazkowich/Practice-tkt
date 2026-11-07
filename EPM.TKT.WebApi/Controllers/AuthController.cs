using EPM.TKT.App.UseCases.Auth.Commands;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPM.TKT.WebApi.Controllers;

/// <summary>Handles user authentication: registration, login, token management, and password reset.</summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Register a new user account.</summary>
    /// <param name="request">Email, password, and optional name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Access token and refresh token pair.</returns>
    /// <response code="200">Registration succeeded — tokens returned.</response>
    /// <response code="409">Email is already registered.</response>
    [HttpPost("register")]
    [EndpointSummary("Register a new user account")]
    [EndpointDescription("Creates a new user, assigns the User role, and returns a token pair. Returns 409 if the email is already registered.")]
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

    /// <summary>Authenticate with email and password.</summary>
    /// <param name="request">User credentials.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Access token and refresh token pair.</returns>
    /// <response code="200">Login succeeded — tokens returned.</response>
    /// <response code="401">Invalid credentials or account inactive.</response>
    [HttpPost("login")]
    [EndpointSummary("Login with email and password")]
    [EndpointDescription("Validates credentials and returns a JWT access token (15 min) and an opaque refresh token (7 days).")]
    [ProducesResponseType(typeof(AuthHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(request.Email, request.Password), ct);

        return result.Match(
            onSuccess: auth  => Ok(Map(auth)),
            onFailure: error => Problem(error));
    }

    /// <summary>Exchange a refresh token for a new token pair.</summary>
    /// <param name="request">The current refresh token.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>New access token and refresh token pair.</returns>
    /// <response code="200">Token refreshed. The old refresh token is revoked — store the new pair immediately.</response>
    /// <response code="401">Refresh token is missing, revoked, or expired.</response>
    [HttpPost("refresh")]
    [EndpointSummary("Refresh the access token")]
    [EndpointDescription("Rotates the refresh token: the supplied token is revoked and a fresh token pair is returned.")]
    [ProducesResponseType(typeof(AuthHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);

        return result.Match(
            onSuccess: auth  => Ok(Map(auth)),
            onFailure: error => Problem(error));
    }

    /// <summary>Request a password reset token.</summary>
    /// <param name="request">The email address of the account to reset.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Request accepted. Always 200 — the response never reveals whether the email exists.</response>
    [HttpPost("forgot-password")]
    [EndpointSummary("Request a password reset token")]
    [EndpointDescription("Generates a signed reset token (30 min). In production the token is delivered by email; it is returned directly here for development convenience.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordHttpRequest request, CancellationToken ct)
    {
        var resetToken = await sender.Send(new ForgotPasswordCommand(request.Email), ct);
        return Ok(new { ResetToken = resetToken });
    }

    /// <summary>Set a new password using a reset token.</summary>
    /// <param name="request">The reset token (from forgot-password) and the new password.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="204">Password updated. All existing refresh tokens for this account are revoked.</response>
    /// <response code="401">Reset token is invalid or expired.</response>
    /// <response code="404">User referenced by the token no longer exists.</response>
    [HttpPost("reset-password")]
    [EndpointSummary("Reset password using a reset token")]
    [EndpointDescription("Validates the signed reset token, updates the password hash, and revokes all active refresh tokens for the account.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ResetPasswordCommand(request.ResetToken, request.NewPassword), ct);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: error => Problem(error));
    }

    /// <summary>Revoke the refresh token and end the session.</summary>
    /// <param name="request">The refresh token to revoke.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="204">Token revoked successfully.</response>
    /// <response code="401">Bearer access token is missing or invalid.</response>
    [Authorize]
    [HttpPost("logout")]
    [EndpointSummary("Logout and revoke the refresh token")]
    [EndpointDescription("Revokes the supplied refresh token. Requires a valid Bearer access token in the Authorization header.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
