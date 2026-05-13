using ADSK.ViewExtension.ViewDuplicate.UI;
using TextRes = ADSK.ViewExtension.ViewDuplicate.Resources.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace ADSK.ViewExtension.ViewDuplicate.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdViewDuplicate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document dbDoc = uiDoc.Document;

            Result uiRet = Result.Cancelled;

            using (Transaction tr1 = new Transaction(dbDoc, TextRes.CMD_VIEWDUPLICATE))
            {
                if (tr1.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        using (var dlg1 = new DlgViewDuplicate(commandData))
                        {
                            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                tr1.Commit();
                                uiRet = Result.Succeeded;
                            }
                            else
                            {
                                tr1.RollBack();
                                uiRet = Result.Cancelled;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tr1.RollBack();
                        TaskDialog.Show(TextRes.TXT_ERROR, ex.Message);
                        uiRet = Result.Failed;
                    }
                }
            }

            return uiRet;
        }
    }
}
