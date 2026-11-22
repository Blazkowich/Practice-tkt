using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class InheritanceTests
{
    // ── Controllers ───────────────────────────────────────────────────────────

    /// <summary>
    /// Checks that all controller classes use Microsoft.AspNetCore.Mvc via dependency
    /// (i.e. inherit ControllerBase) without directly referencing the framework type here,
    /// which would pull in a vulnerable assembly reference.
    /// </summary>
    [Test]
    public void Controllers_Should_Depend_On_AspNetCoreMvc()
    {
        var result = Types.InAssembly(TestAssemblies.WebApi)
            .That().ResideInNamespace("EPM.TKT.WebApi.Controllers")
            .And().HaveNameEndingWith("Controller")
            .Should().HaveDependencyOn("Microsoft.AspNetCore.Mvc")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Every controller must inherit from ControllerBase (checked via MVC dependency).");
    }

    [Test]
    public void Controllers_Should_Be_Sealed()
    {
        var result = Types.InAssembly(TestAssemblies.WebApi)
            .That().ResideInNamespace("EPM.TKT.WebApi.Controllers")
            .And().HaveNameEndingWith("Controller")
            .Should().BeSealed()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Controllers should be sealed — they are not designed for inheritance.");
    }

    // ── Domain ────────────────────────────────────────────────────────────────

    [Test]
    public void Domain_Types_ShouldNot_Inherit_ValidationAttribute()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .That().AreClasses()
            .ShouldNot().Inherit(typeof(System.ComponentModel.DataAnnotations.ValidationAttribute))
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain types must not inherit from ValidationAttribute.");
    }

    [Test]
    public void Domain_Types_ShouldNot_Inherit_Attribute()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .That().AreClasses()
            .ShouldNot().Inherit(typeof(Attribute))
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain types must not define custom attributes.");
    }

    // ── Infrastructure ────────────────────────────────────────────────────────

    [Test]
    public void Repository_Implementations_Should_Be_Sealed()
    {
        var result = Types.InAssembly(TestAssemblies.Infrastructure)
            .That().ResideInNamespace("EPM.TKT.Infrastructure.Repositories")
            .And().AreClasses()
            .Should().BeSealed()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Repository implementations should be sealed.");
    }
}
