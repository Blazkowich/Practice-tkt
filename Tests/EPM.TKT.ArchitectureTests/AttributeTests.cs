using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class AttributeTests
{
    // ── Domain purity ─────────────────────────────────────────────────────────

    [Test]
    public void Domain_ShouldNot_Use_DataAnnotations()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn("System.ComponentModel.DataAnnotations")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on DataAnnotations.");
    }

    [Test]
    public void Domain_ShouldNot_Use_EntityFramework()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on Entity Framework Core.");
    }

    // ── Application layer purity ──────────────────────────────────────────────

    [Test]
    public void Application_ShouldNot_Use_EntityFramework()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on Entity Framework Core.");
    }

    [Test]
    public void Application_ShouldNot_Use_DataAnnotations()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn("System.ComponentModel.DataAnnotations")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on DataAnnotations.");
    }
}
