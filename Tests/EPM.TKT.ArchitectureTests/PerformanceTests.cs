using System.Reflection;

namespace EPM.TKT.ArchitectureTests;

/// <summary>
/// Enforces async naming conventions so callers always know when a method
/// involves I/O and should be awaited.
/// </summary>
[TestFixture]
public sealed class PerformanceTests
{
    // ── Infrastructure repositories ───────────────────────────────────────────

    [Test]
    public void Async_Methods_In_Repositories_Should_End_With_Async()
    {
        var repositoryTypes = TestAssemblies.Infrastructure
            .GetTypes()
            .Where(t => t.IsClass
                     && t.Namespace?.StartsWith("EPM.TKT.Infrastructure.Repositories") == true);

        AssertAsyncNaming(repositoryTypes, "Infrastructure repository");
    }

    // ── App use-case handlers ─────────────────────────────────────────────────

    [Test]
    public void Async_Methods_In_Handlers_Should_End_With_Async()
    {
        var handlerTypes = TestAssemblies.App
            .GetTypes()
            .Where(t => t.IsClass
                     && t.Namespace?.StartsWith("EPM.TKT.App.UseCases") == true
                     && t.Name.EndsWith("Handler"));

        AssertAsyncNaming(handlerTypes, "App handler");
    }

    // ── WebApi controllers ────────────────────────────────────────────────────

    [Test]
    public void Async_Methods_In_Controllers_Should_End_With_Async()
    {
        var controllerTypes = TestAssemblies.WebApi
            .GetTypes()
            .Where(t => t.IsClass
                     && t.Namespace?.StartsWith("EPM.TKT.WebApi.Controllers") == true
                     && t.Name.EndsWith("Controller"));

        AssertAsyncNaming(controllerTypes, "WebApi controller");
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static void AssertAsyncNaming(IEnumerable<Type> types, string context)
    {
        foreach (var type in types)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (typeof(Task).IsAssignableFrom(method.ReturnType))
                {
                    Assert.That(method.Name, Does.EndWith("Async"),
                        $"{context} method '{type.FullName}.{method.Name}' returns Task but does not end with 'Async'.");
                }
            }
        }
    }
}
