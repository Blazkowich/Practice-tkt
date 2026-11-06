Tests live in `EPM.TKT.Tests/`. Framework: **xunit** + **Moq**. Run with:

```
dotnet test EPM.TKT.slnx
dotnet test EPM.TKT.slnx --filter "FullyQualifiedName~YourHandlerTests"
```

## Handler test

```csharp
using Moq;
using Xunit;
using EPM.TKT.App.Repositories;
using EPM.TKT.App.UseCases.[Domain].Queries;
using EPM.TKT.Domain.Models.[Domain];

namespace EPM.TKT.Tests.UseCases.[Domain];

public class Get[Name]HandlerTests
{
    private readonly Mock<I[Domain]Repository> _repositoryMock;
    private readonly Get[Name]Handler _handler;

    public Get[Name]HandlerTests()
    {
        _repositoryMock = new Mock<I[Domain]Repository>(MockBehavior.Strict);
        _handler = new Get[Name]Handler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsModel_WhenFound()
    {
        var expected = new [Name]Model { Id = 1, Title = "Test" };
        _repositoryMock
            .Setup(r => r.Get[Name]Async(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new Get[Name]Request(1), CancellationToken.None);

        Assert.Equal(expected, result);
        _repositoryMock.VerifyAll();
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNotFound()
    {
        _repositoryMock
            .Setup(r => r.Get[Name]Async(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(([Name]Model?)null);

        var result = await _handler.Handle(new Get[Name]Request(99), CancellationToken.None);

        Assert.Null(result);
        _repositoryMock.VerifyAll();
    }
}
```

Rules:
- No class-level attribute (xunit discovers classes automatically).
- Constructor initializes all mocks and the class under test; `IDisposable.Dispose()` for teardown.
- `MockBehavior.Strict` on all mocks — unexpected calls fail the test immediately.
- Call `_mock.VerifyAll()` at the end of every test that sets up mock calls.
- Use `[Fact]` for single-scenario tests; `[Theory]` + `[InlineData(...)]` for parameterized scenarios.
- Assertions: `Assert.Equal(expected, actual)`, `Assert.Null(x)`, `Assert.NotNull(x)`, `Assert.IsType<T>(x)`, `Assert.True(cond)`.

## Repository test (SQLite in-memory)

```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using EPM.TKT.Infrastructure.Entities.TKT;
using EPM.TKT.Infrastructure.Repositories;

namespace EPM.TKT.Tests.Repositories;

public class [Domain]RepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly [Name]TestDbContext _dbContext;
    private readonly Mock<ITKTDbContext> _contextMock;

    public [Domain]RepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<[Name]TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new [Name]TestDbContext(options);
        _dbContext.Database.EnsureCreated();

        _contextMock = new Mock<ITKTDbContext>();
        _contextMock.Setup(c => c.[DbSetPluralName]).Returns(_dbContext.[DbSetPluralName]);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task Get[Name]Async_ReturnsCorrectRow()
    {
        _dbContext.[DbSetPluralName].Add(new [EntityName] { [EntityName]Id = 1, Name = "Foo" });
        await _dbContext.SaveChangesAsync();

        var repo = new [Domain]Repository(_contextMock.Object);
        var result = await repo.Get[Name]Async(1);

        Assert.NotNull(result);
        Assert.Equal("Foo", result!.Name);
    }

    // Minimal inline DbContext — only includes the entities this test exercises
    private sealed class [Name]TestDbContext(DbContextOptions<[Name]TestDbContext> opts) : DbContext(opts)
    {
        public DbSet<[EntityName]> [DbSetPluralName] { get; set; } = null!;
    }
}
```

Rules:
- Implement `IDisposable`; open the `SqliteConnection` in the constructor, dispose in `Dispose()`.
- Create a minimal inline `DbContext` containing only the entities the test exercises.
- Mock `ITKTDbContext` and wire its `DbSet` properties to the real in-memory DbSets.
- Seed data with `_dbContext.Add(...); await _dbContext.SaveChangesAsync()` before the repository call.
- Use `[Theory]` + `[InlineData(...)]` for parameterized filter/sort scenarios.

## Naming conventions

- Test class: `[HandlerOrRepositoryName]Tests`
- Test method: `[MethodUnderTest]_[ExpectedOutcome]_[Condition]`
- Test file mirrors source path under `EPM.TKT.Tests/`
