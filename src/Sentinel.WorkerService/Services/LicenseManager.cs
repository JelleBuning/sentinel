using System.Xml;
using Sentinel.WorkerService.Common.Module.Interfaces;

namespace Sentinel.WorkerService.Services;

public static class LicenseManager
{
    public static bool IsLicensed<T>() where T : IModule
    {
        var moduleName = typeof(T).Assembly.GetName().Name;
        return GetLicensedModules().Any(licensedModule =>  moduleName?.StartsWith(licensedModule) ?? false);
    }

    private static List<string> GetLicensedModules() // NOTE: Can be replaced by API call for organisation licenses (sync the file with api call, so it will work offline)
    {
        var executableDirectory = Directory.GetParent(AppContext.BaseDirectory);
        var fileName = $"{executableDirectory?.FullName}\\Modules.config";

        var xmlDocument = new XmlDocument();
        xmlDocument.Load(fileName);

        var licenseNodes = xmlDocument.GetElementsByTagName("modules").Item(0)!.ChildNodes.Cast<XmlNode>().Where(x => x.Name == "module");
        return licenseNodes.Select(licenseNode => licenseNode.Attributes!["applicationKey"]!.Value).ToList();
    }
}