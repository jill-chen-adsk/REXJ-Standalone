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
  public class CmdEditDefSystem : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      var cmpAttribute = new PipeSizing.Components.Attribute();
      var cmpElements = new Elements(commandData.Application.ActiveUIDocument);
      var cmpGeometry = new Geometry(commandData.Application.ActiveUIDocument);
      var cmpParameters = new Parameters(cmpAttribute, commandData.Application.ActiveUIDocument);
      var cmpSettings = new PipeSizing.Components.Settings(commandData.Application.ActiveUIDocument);
      var cmpService = new Service(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);

      Result ret = Result.Cancelled;

      if (!cmpService.IsExcelInComputer())
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOEXCEL"));
        return ret;
      }

      string filePath = Path.Combine(
        Path.GetDirectoryName(cmpAttribute.ExecuteFile) ?? ".",
        "Data",
        "PipeSizing",
        cmpAttribute.ResourceText("IDS_TXT_DEFSYSTEMFILE"));

      if (System.Windows.Forms.MessageBox.Show(
            cmpAttribute.ResourceText("IDS_TXT_EDITDEFSYSTEM_CONFIRM"),
            cmpAttribute.ResourceText("IDS_TXT_PIPESIZING"),
            System.Windows.Forms.MessageBoxButtons.YesNo,
            System.Windows.Forms.MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
      {
        return ret;
      }

      if (File.Exists(filePath))
      {
        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        ret = Result.Succeeded;
      }
      else
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_DEFSYSTEMFILE"));
        ret = Result.Failed;
      }

      return ret;
    }
  }
}
