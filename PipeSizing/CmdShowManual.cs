using System.Diagnostics;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PipeSizing.Components;

namespace PipeSizing
{
  [Transaction(TransactionMode.Manual)]
  [Regeneration(RegenerationOption.Manual)]
  public class CmdShowManual : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      var cmpAttribute = new PipeSizing.Components.Attribute();

      Result ret = Result.Cancelled;

      string rel = cmpAttribute.ResourceText("IDS_TXT_JEXTRME_MANUAL").TrimStart('\\', '/');
      string filePath = Path.Combine(Path.GetDirectoryName(cmpAttribute.ExecuteFile) ?? ".", rel.Replace('/', Path.DirectorySeparatorChar));

      if (File.Exists(filePath))
      {
        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        ret = Result.Succeeded;
      }
      else
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_MANUALFILE"));
        ret = Result.Failed;
      }

      return ret;
    }
  }
}
