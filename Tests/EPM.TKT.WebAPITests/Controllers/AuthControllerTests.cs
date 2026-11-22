using EPM.TKT.App.UseCases.Auth;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Requests.Auth;
using EPM.TKT.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPM.TKT.WebAPITests.Controllers;

[TestFixture]
public sealed class AuthControllerTests
{
    private Mock<ISender> _sender = null!;
    private AuthController _controller = null!;

    private static AuthResult FakeAuth() => new()
    {
        AccessToken = "access-token",
        RefreshToken = "refresh-token",
        AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15)
    };

    [SetUp]
    public void SetUp()
    {
        _sender = new Mock<ISender>();
        _controller = new AuthController(_sender.Object);
    }

    // ── Register ────────────────────────────────────────────────────────────

    [Test]
    public async Task Register_ReturnsOk_WhenSuccessful()
    {
        _sender.Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<AuthResult>.Ok(FakeAuth()));

        var result = await _controller.Register(
            new RegisterHttpRequest("user@example.com", "P@ssword1", "John", "Doe"), default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Register_ReturnsConflict_WhenEmailAlreadyRegistered()
    {
        _sender.Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<AuthResult>.Fail("Email already registered.", ErrorType.Conflict));

        var result = await _controller.Register(
            new RegisterHttpRequest("dup@example.com", "P@ssword1", null, null), default);

        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
    }

    // ── Login ────────────────────────────────────────────────────────────────

    [Test]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        _sender.Setup(s => s.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<AuthResult>.Ok(FakeAuth()));

        var result = await _controller.Login(
            new LoginHttpRequest("user@example.com", "P@ssword1"), default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        _sender.Setup(s => s.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<AuthResult>.Fail("Invalid credentials.", ErrorType.Unauthorized));

        var result = await _controller.Login(
            new LoginHttpRequest("user@example.com", "wrong-pass"), default);

        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    // ── Refresh ──────────────────────────────────────────────────────────────

    [Test]
    public async Task Refresh_ReturnsOk_WhenTokenIsValid()
    {
        _sender.Setup(s => s.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<AuthResult>.Ok(FakeAuth()));

        var result = await _controller.Refresh(new RefreshTokenHttpRequest("valid-rt"), default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Refresh_ReturnsUnauthorized_WhenTokenIsExpired()
    {
        _sender.Setup(s => s.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<AuthResult>.Fail("Token expired.", ErrorType.Unauthorized));

        var result = await _controller.Refresh(new RefreshTokenHttpRequest("expired-rt"), default);

        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    // ── ForgotPassword ───────────────────────────────────────────────────────

    [Test]
    public async Task ForgotPassword_ReturnsOk_Always()
    {
        _sender.Setup(s => s.Send(It.IsAny<ForgotPasswordCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync("reset-token-xyz");

        var result = await _controller.ForgotPassword(
            new ForgotPasswordHttpRequest("user@example.com"), default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    // ── ResetPassword ────────────────────────────────────────────────────────

    [Test]
    public async Task ResetPassword_ReturnsNoContent_WhenSuccessful()
    {
        _sender.Setup(s => s.Send(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Ok());

        var result = await _controller.ResetPassword(
            new ResetPasswordHttpRequest("valid-token", "NewP@ss1"), default);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task ResetPassword_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        _sender.Setup(s => s.Send(It.IsAny<ResetPasswordCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Fail("Invalid or expired reset token.", ErrorType.Unauthorized));

        var result = await _controller.ResetPassword(
            new ResetPasswordHttpRequest("bad-token", "NewP@ss1"), default);

        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    // ── Logout ───────────────────────────────────────────────────────────────

    [Test]
    public async Task Logout_ReturnsNoContent()
    {
        _sender.Setup(s => s.Send(It.IsAny<LogoutCommand>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

        var result = await _controller.Logout(new RefreshTokenHttpRequest("rt"), default);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
}
