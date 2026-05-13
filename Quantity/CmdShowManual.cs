using System.Diagnostics;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Quantity.Components;

namespace Quantity
{
  [Transaction(TransactionMode.Manual)]
  [Regeneration(RegenerationOption.Manual)]
  public sealed class CmdShowManual : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      var cmpAttribute = new Attribute();
      string filePath = Path.Combine(
        Path.GetDirectoryName(typeof(CmdShowManual).Assembly.Location) ?? ".",
        cmpAttribute.ResourceText("IDS_TXT_JEXTRME_MANUAL").TrimStart('\\', '/').Replace('/', Path.DirectorySeparatorChar));

      if (File.Exists(filePath))
      {
        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        return Result.Succeeded;
      }

      System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_MANUALFILE"));
      return Result.Failed;
    }
  }
}
