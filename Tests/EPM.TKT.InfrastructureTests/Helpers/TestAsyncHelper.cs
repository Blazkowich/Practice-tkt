using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EPM.TKT.InfrastructureTests.Helpers;

/// <summary>
/// Implements IAsyncQueryProvider so that EF Core async extension methods
/// (AnyAsync, FirstOrDefaultAsync, ToListAsync, SumAsync …) work over
/// plain in-memory lists without a real database provider.
/// </summary>
internal sealed class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
{
    public IQueryable CreateQuery(Expression expression) =>
        new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
        new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(Expression expression) => inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        // TResult is always Task<X>; get the inner type X
        var innerType = typeof(TResult).GetGenericArguments()[0];

        // Execute synchronously with the in-memory LINQ provider
        var syncResult = typeof(IQueryProvider)
            .GetMethods()
            .First(m => m.Name == nameof(IQueryProvider.Execute) && m.IsGenericMethod)
            .MakeGenericMethod(innerType)
            .Invoke(inner, [expression]);

        // Wrap in Task.FromResult<X>(syncResult)
        return (TResult)typeof(Task)
            .GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(innerType)
            .Invoke(null, [syncResult])!;
    }
}

/// <summary>
/// Wraps an in-memory list as an IQueryable that also supports IAsyncEnumerable,
/// satisfying EF Core's async LINQ requirements.
/// </summary>
internal sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression)   : base(expression)   { }

    // Override Provider so the async query provider is used instead of LINQ-to-Objects
    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default) =>
        new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}

internal sealed class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
{
    public T Current => inner.Current;
    public ValueTask<bool> MoveNextAsync() => new(inner.MoveNext());
    public ValueTask DisposeAsync() { inner.Dispose(); return ValueTask.CompletedTask; }
}

/// <summary>
/// Creates a Mock&lt;DbSet&lt;T&gt;&gt; backed by an in-memory list, wired up for
/// both LINQ queries and EF Core async extension methods.
/// </summary>
internal static class MockDbSet
{
    public static Mock<DbSet<T>> Create<T>(IList<T>? seed = null) where T : class
    {
        var list  = seed ?? [];
        var query = new TestAsyncEnumerable<T>(list);

        var mock = new Mock<DbSet<T>>();

        mock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => new TestAsyncEnumerator<T>(list.GetEnumerator()));

        // EnumerableQuery<T> implements IQueryable<T> explicitly — cast required
        var q = (IQueryable<T>)query;
        mock.As<IQueryable<T>>().Setup(m => m.Provider)    .Returns(q.Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression)  .Returns(q.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType) .Returns(q.ElementType);
        mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => list.GetEnumerator());

        return mock;
    }
}
