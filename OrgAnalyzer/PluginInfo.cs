using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Rappen.XTB.OrgAnalyzer
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Organization Analyzer"),
        ExportMetadata("Description", "Analyze various metrics in Microsoft Dynamics 365"),
        ExportMetadata("SmallImageBase64", null),
        ExportMetadata("BigImageBase64", null),
        ExportMetadata("BackgroundColor", "#f8f8ff"),
        ExportMetadata("PrimaryFontColor", "#00319C"),
        ExportMetadata("SecondaryFontColor", "#9439B5")]
    public class PluginInfo : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new OrgAnalyzer();
        }
    }
}