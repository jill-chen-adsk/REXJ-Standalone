using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PipeSizing.Components;

namespace PipeSizing
{
  [Transaction(TransactionMode.Manual)]
  [Regeneration(RegenerationOption.Manual)]
  public class CmdPipeSizing : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      UIApplication rvtUiApp = commandData.Application;
      UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument;
      Document rvtDbDoc = rvtUiDoc.Document;

      var cmpAttribute = new PipeSizing.Components.Attribute();
      var cmpElements = new Elements(rvtUiDoc);
      var cmpGeometry = new Geometry(rvtUiDoc);
      var cmpParameters = new Parameters(cmpAttribute, rvtUiDoc);
      var cmpSettings = new PipeSizing.Components.Settings(rvtUiDoc);
      var cmpService = new Service(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);

      Result ret = Result.Cancelled;

      IList<Element> targetPipes = cmpElements.SelectPipeAry;
      IList<FamilyInstance> targetPipeFitValve = cmpElements.SelectPipeFittingValveAry;

      if (targetPipes.Count < 1 && targetPipeFitValve.Count < 1)
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SELECTPIPE"), cmpAttribute.ResourceText("IDS_TXT_PIPESIZING"));
        return ret;
      }

      if (!cmpParameters.GetSystemTypeTable())
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETDEFTABLE"), cmpAttribute.ResourceText("IDS_TXT_PIPESIZING"));
        return ret;
      }

      if (!cmpParameters.GetSizeTable())
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETTABLE"), cmpAttribute.ResourceText("IDS_TXT_PIPESIZING"));
        return ret;
      }

      TransactionGroup txGrp = new TransactionGroup(rvtDbDoc);
      txGrp.Start("Pipe Size Correction");

      Transaction tx = new Transaction(rvtDbDoc);
      tx.Start("Size correction");

      foreach (Element elem in targetPipes)
      {
        cmpService.PipeSizing(elem);
      }

      var fittingAccessory = new List<FamilyInstance>();
      foreach (FamilyInstance famIns in cmpElements.SelectPipeFittingValveAry)
      {
        try
        {
          if (famIns != null)
          {
            fittingAccessory.Add(famIns);
          }
        }
        catch
        {
          // continue
        }
      }

      foreach (FamilyInstance famIns in fittingAccessory)
      {
        cmpService.FittingAccessorySizing(famIns);
      }

      tx.Commit();
      txGrp.Assimilate();

      rvtUiDoc.RefreshActiveView();

      rvtUiDoc.Selection.SetElementIds(new List<ElementId>());

      System.Windows.Forms.MessageBox.Show(
        cmpAttribute.ResourceText("IDS_TXT_CMDFINISH"),
        cmpAttribute.ResourceText("IDS_TXT_PIPESIZING"),
        System.Windows.Forms.MessageBoxButtons.OK,
        System.Windows.Forms.MessageBoxIcon.None,
        System.Windows.Forms.MessageBoxDefaultButton.Button1,
        System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);

      return Result.Succeeded;
    }
  }
}
