using EPM.TKT.Domain.Enums;
using EPM.TKT.Infrastructure.Context.Interfaces;
using EPM.TKT.Infrastructure.Entities.Sec;
using EPM.TKT.Infrastructure.Repositories;
using EPM.TKT.InfrastructureTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EPM.TKT.InfrastructureTests.Repositories;

[TestFixture]
public sealed class RoleRepositoryTests
{
    private Mock<ITKTDbContext>   _context   = null!;
    private Mock<DbSet<UserRole>> _mockUserRoles = null!;
    private Mock<DbSet<Role>>     _mockRoles = null!;
    private RoleRepository        _repo      = null!;

    [SetUp]
    public void SetUp()
    {
        _context       = new Mock<ITKTDbContext>();
        _mockUserRoles = MockDbSet.Create<UserRole>();
        _mockRoles     = MockDbSet.Create<Role>();

        _context.Setup(c => c.UserRoles).Returns(_mockUserRoles.Object);
        _context.Setup(c => c.Roles).Returns(_mockRoles.Object);
        _context.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Invoke the lambda so inner operations are exercised
        _context.Setup(c => c.ExecuteInTransactionAsync(
                    It.IsAny<Func<CancellationToken, Task>>(),
                    It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>(
                    (action, ct) => action(ct));

        _repo = new RoleRepository(_context.Object);
    }

    // ── AssignRoleAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task AssignRoleAsync_AddsUserRole_WhenRoleDoesNotExist()
    {
        var userId = Guid.NewGuid();
        // Empty UserRoles — role not yet assigned
        _context.Setup(c => c.UserRoles).Returns(MockDbSet.Create<UserRole>().Object);

        UserRole? captured = null;
        _mockUserRoles.Setup(d => d.Add(It.IsAny<UserRole>()))
                      .Callback<UserRole>(r => captured = r);

        // Re-create repo with fresh context setup
        _context.Setup(c => c.UserRoles).Returns(_mockUserRoles.Object);
        var repo = new RoleRepository(_context.Object);

        await repo.AssignRoleAsync(userId, RoleType.Admin);

        Assert.That(captured,          Is.Not.Null);
        Assert.That(captured!.UserId,  Is.EqualTo(userId));
        Assert.That(captured.RoleId,   Is.EqualTo((byte)RoleType.Admin));
    }

    [Test]
    public async Task AssignRoleAsync_CallsSaveChanges_WhenRoleDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _context.Setup(c => c.UserRoles).Returns(MockDbSet.Create<UserRole>().Object);
        var repo = new RoleRepository(_context.Object);

        await repo.AssignRoleAsync(userId, RoleType.Admin);

        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task AssignRoleAsync_DoesNotAddOrSave_WhenRoleAlreadyExists()
    {
        var userId = Guid.NewGuid();
        var existing = new List<UserRole>
        {
            new() { UserId = userId, RoleId = (byte)RoleType.Admin, AssignedAt = DateTime.UtcNow }
        };
        _context.Setup(c => c.UserRoles).Returns(MockDbSet.Create(existing).Object);
        var repo = new RoleRepository(_context.Object);

        await repo.AssignRoleAsync(userId, RoleType.Admin);

        _mockUserRoles.Verify(d => d.Add(It.IsAny<UserRole>()), Times.Never);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── GetUserRolesAsync ─────────────────────────────────────────────────────

    [Test]
    public async Task GetUserRolesAsync_ReturnsRoles_ForUser()
    {
        var userId = Guid.NewGuid();
        var userRoles = new List<UserRole>
        {
            new() { UserId = userId,        RoleId = 1, AssignedAt = DateTime.UtcNow },
            new() { UserId = Guid.NewGuid(), RoleId = 2, AssignedAt = DateTime.UtcNow } // different user
        };
        var roles = new List<Role>
        {
            new() { RoleId = 1, Name = "User" },
            new() { RoleId = 2, Name = "Manager" }
        };
        _context.Setup(c => c.UserRoles).Returns(MockDbSet.Create(userRoles).Object);
        _context.Setup(c => c.Roles).Returns(MockDbSet.Create(roles).Object);
        var repo = new RoleRepository(_context.Object);

        var result = await repo.GetUserRolesAsync(userId);

        Assert.That(result.Count,         Is.EqualTo(1));
        Assert.That(result[0].Name,       Is.EqualTo("User"));
    }

    [Test]
    public async Task GetUserRolesAsync_ReturnsEmpty_WhenNoRolesAssigned()
    {
        var userId = Guid.NewGuid();
        _context.Setup(c => c.UserRoles).Returns(MockDbSet.Create<UserRole>().Object);
        _context.Setup(c => c.Roles).Returns(MockDbSet.Create<Role>().Object);
        var repo = new RoleRepository(_context.Object);

        var result = await repo.GetUserRolesAsync(userId);

        Assert.That(result, Is.Empty);
    }

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task GetAllAsync_ReturnsAllRoles()
    {
        var roles = new List<Role>
        {
            new() { RoleId = 1, Name = "User" },
            new() { RoleId = 2, Name = "Manager" }
        };
        _context.Setup(c => c.Roles).Returns(MockDbSet.Create(roles).Object);
        var repo = new RoleRepository(_context.Object);

        var result = await repo.GetAllAsync();

        Assert.That(result.Count, Is.EqualTo(2));
    }
}
