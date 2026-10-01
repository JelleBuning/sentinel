using Mycelium.WorkerService.Services;
using NUnit.Framework;

namespace Mycelium.WorkerService.UnitTests.Services;

public class LicenseManagerTests
{
    [Test]
    public void ParseLicensedModules_SingleModule_ReturnsApplicationKey()
    {
        const string xml = """<modules><module applicationKey="ModuleA" /></modules>""";

        var result = LicenseManager.ParseLicensedModules(xml);

        Assert.That(result, Is.EqualTo(new List<string> { "ModuleA" }));
    }

    [Test]
    public void ParseLicensedModules_MultipleModules_ReturnsAllApplicationKeys()
    {
        const string xml = """
            <modules>
                <module applicationKey="ModuleA" />
                <module applicationKey="ModuleB" />
                <module applicationKey="ModuleC" />
            </modules>
            """;

        var result = LicenseManager.ParseLicensedModules(xml);

        Assert.That(result, Is.EqualTo(new List<string> { "ModuleA", "ModuleB", "ModuleC" }));
    }

    [Test]
    public void ParseLicensedModules_NoModules_ReturnsEmptyList()
    {
        const string xml = """<modules></modules>""";

        var result = LicenseManager.ParseLicensedModules(xml);

        Assert.That(result, Is.Empty);
    }

    // IsLicensed<T>() is not covered here: it always goes through GetLicensedModules(), which reads
    // "Modules.config" from the executable's directory on disk with no seam to inject a fake path,
    // so it can't be isolated from the real file system in a unit test.
}
