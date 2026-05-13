using ADSK.ViewExtension.SheetLayout.Resources;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
namespace ADSK.ViewExtension.SheetLayout.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdSheetLayout : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document dbDoc = commandData.Application.ActiveUIDocument.Document;
            Result retUi = Result.Cancelled;

            using (Transaction tr1 = new Transaction(dbDoc, Text.CMD_SHEETLAYOUT))
            {
                if (tr1.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        using var dlg1 = new DlgSheetLayout(commandData);
                        if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            tr1.Commit();
                            retUi = Result.Succeeded;
                        }
                        else
                        {
                            tr1.RollBack();
                            retUi = Result.Cancelled;
                        }
                    }
                    catch (Exception ex)
                    {
                        tr1.RollBack();
                        TaskDialog.Show(Text.TXT_ERROR, ex.Message);
                        retUi = Result.Failed;
                    }
                }
            }

            return retUi;
        }
    }
}
