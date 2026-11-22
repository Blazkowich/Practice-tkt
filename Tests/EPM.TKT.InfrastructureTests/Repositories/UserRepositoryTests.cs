using EPM.TKT.Domain.Enums;
using EPM.TKT.Infrastructure.Context.Interfaces;
using EPM.TKT.Infrastructure.Entities.Sec;
using EPM.TKT.Infrastructure.Repositories;
using EPM.TKT.InfrastructureTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EPM.TKT.InfrastructureTests.Repositories;

[TestFixture]
public sealed class UserRepositoryTests
{
    private Mock<ITKTDbContext> _context = null!;
    private Mock<DbSet<User>> _mockUsers = null!;
    private Mock<DbSet<UserRole>> _mockRoles = null!;
    private UserRepository _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _context = new Mock<ITKTDbContext>();
        _mockUsers = MockDbSet.Create<User>();
        _mockRoles = MockDbSet.Create<UserRole>();

        _context.Setup(c => c.Users).Returns(_mockUsers.Object);
        _context.Setup(c => c.UserRoles).Returns(_mockRoles.Object);
        _context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Invoke the lambda so inner Add / SaveChanges calls are exercised
        _context.Setup(c => c.ExecuteInTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task<Guid>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task<Guid>>, CancellationToken>(
                    (action, ct) => action(ct));

        _repo = new UserRepository(_context.Object);
    }

    // ── CreateUserAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task CreateUserAsync_AddsUser_WithCorrectFields()
    {
        User? captured = null;
        _mockUsers.Setup(d => d.Add(It.IsAny<User>()))
                  .Callback<User>(u => captured = u);

        await _repo.CreateUserAsync("alice@example.com", "hash123", "Alice", "Smith");

        Assert.That(captured, Is.Not.Null);
        Assert.That(captured!.Email, Is.EqualTo("alice@example.com"));
        Assert.That(captured.PasswordHash, Is.EqualTo("hash123"));
        Assert.That(captured.FirstName, Is.EqualTo("Alice"));
        Assert.That(captured.LastName, Is.EqualTo("Smith"));
        Assert.That(captured.IsActive, Is.True);
    }

    [Test]
    public async Task CreateUserAsync_AddsUserRole_WithUserRoleType()
    {
        User? capturedUser = null;
        _mockUsers.Setup(d => d.Add(It.IsAny<User>()))
                  .Callback<User>(u => capturedUser = u);

        UserRole? capturedRole = null;
        _mockRoles.Setup(d => d.Add(It.IsAny<UserRole>()))
                  .Callback<UserRole>(r => capturedRole = r);

        await _repo.CreateUserAsync("bob@example.com", "hash", null, null);

        Assert.That(capturedRole, Is.Not.Null);
        Assert.That(capturedRole!.RoleId, Is.EqualTo((byte)RoleType.User));
        Assert.That(capturedRole.UserId, Is.EqualTo(capturedUser!.UserId));
    }

    [Test]
    public async Task CreateUserAsync_CallsSaveChanges_Once()
    {
        _mockUsers.Setup(d => d.Add(It.IsAny<User>()));
        _mockRoles.Setup(d => d.Add(It.IsAny<UserRole>()));

        await _repo.CreateUserAsync("carol@example.com", "hash", null, null);

        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateUserAsync_ReturnsNewUserId()
    {
        User? capturedUser = null;
        // Simulate EF Core's client-side Guid generation that happens inside Add()
        _mockUsers.Setup(d => d.Add(It.IsAny<User>()))
                  .Callback<User>(u => { u.UserId = Guid.NewGuid(); capturedUser = u; });
        _mockRoles.Setup(d => d.Add(It.IsAny<UserRole>()));

        var userId = await _repo.CreateUserAsync("dan@example.com", "hash", null, null);

        Assert.That(userId, Is.EqualTo(capturedUser!.UserId));
        Assert.That(userId, Is.Not.EqualTo(Guid.Empty));
    }

    // ── EmailExistsAsync ──────────────────────────────────────────────────────

    [Test]
    public async Task EmailExistsAsync_ReturnsTrue_WhenUserExists()
    {
        var users = new List<User> { new() { UserId = Guid.NewGuid(), Email = "exists@example.com" } };
        _context.Setup(c => c.Users).Returns(MockDbSet.Create(users).Object);
        var repo = new UserRepository(_context.Object);

        var result = await repo.EmailExistsAsync("exists@example.com");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task EmailExistsAsync_ReturnsFalse_WhenUserNotFound()
    {
        _context.Setup(c => c.Users).Returns(MockDbSet.Create<User>().Object);
        var repo = new UserRepository(_context.Object);

        var result = await repo.EmailExistsAsync("nobody@example.com");

        Assert.That(result, Is.False);
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Test]
    public async Task GetByIdAsync_ReturnsUser_WhenFound()
    {
        var userId = Guid.NewGuid();
        var users = new List<User>
        {
            new() { UserId = userId, Email = "eve@example.com", IsActive = true }
        };
        var roles = new List<UserRole>();
        var rolesList = new List<Role>();

        _context.Setup(c => c.Users).Returns(MockDbSet.Create(users).Object);
        _context.Setup(c => c.UserRoles).Returns(MockDbSet.Create(roles).Object);
        _context.Setup(c => c.Roles).Returns(MockDbSet.Create(rolesList).Object);
        var repo = new UserRepository(_context.Object);

        var result = await repo.GetByIdAsync(userId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.UserId, Is.EqualTo(userId));
        Assert.That(result.Email, Is.EqualTo("eve@example.com"));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _context.Setup(c => c.Users).Returns(MockDbSet.Create<User>().Object);
        _context.Setup(c => c.UserRoles).Returns(MockDbSet.Create<UserRole>().Object);
        _context.Setup(c => c.Roles).Returns(MockDbSet.Create<Role>().Object);
        var repo = new UserRepository(_context.Object);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    // NOTE: DeactivateAsync and UpdatePasswordAsync use ExecuteUpdateAsync — an EF Core
    // bulk-update operation that bypasses the change tracker entirely. It requires a real
    // database provider (SQL Server / InMemory) and cannot be exercised with Moq alone.
    // Those methods are covered by integration tests.
}
