#region Namespaces

using ADSK.JExtRAC.AutoCreateDimension.Common;
using ADSK.JExtRAC.AutoCreateDimension.Screen;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ResText = ADSK.JExtRAC.AutoCreateDimension.Resources.Text;
using System.Collections.Generic;

#endregion Namespaces

namespace ADSK.JExtRAC.AutoCreateDimension.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdAutoCreateDimension : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication UiApp = commandData.Application;
            UIDocument UiDoc = UiApp.ActiveUIDocument;
            Application App = UiApp.Application;
            Document Doc = UiDoc.Document;

            ICollection<ElementId> elementIdList = UiDoc.Selection.GetElementIds();
            if (elementIdList.Count == 0) {
                ComDialog.ShowDialog(ResText.IDS_MSG_WARN_TITLE, TaskDialogIcon.TaskDialogIconWarning, ResText.IDS_MSG_SELECT_OBJECTS, false);
                return Result.Succeeded;
            }
            using (TransactionGroup transGroup = new TransactionGroup(Doc, ResText.IDS_TRN_PLACEMENT)) {
                transGroup.Start(ResText.IDS_TRN_BATCH_DIMENSION);

                while (true) {
                    AutoDimension form = new AutoDimension(commandData, elementIdList);
                    System.Windows.Forms.NativeWindow nativeWindow = System.Windows.Forms.NativeWindow.FromHandle(UiApp.MainWindowHandle);
                    form.ShowDialog(nativeWindow);

                    while (form.DialogResult == System.Windows.Forms.DialogResult.OK && form._isSelectPoint) {
                        if (form._isPoint) {
                            SketchPlane activeSp = Doc.ActiveView.SketchPlane;
                            try {
                                XYZ point;
                                using (Transaction tran = new Transaction(Doc, ResText.IDS_TRN_WORKPLANE)) {
                                    tran.Start();
                                    Plane plane = Plane.CreateByNormalAndOrigin(Doc.ActiveView.ViewDirection, Doc.ActiveView.Origin);
                                    SketchPlane sp = SketchPlane.Create(Doc, plane);
                                    Doc.ActiveView.SketchPlane = sp;
                                    point = UiDoc.Selection.PickPoint(ResText.IDS_MSG_PICK_DIM_BASE_POINT);
                                    tran.Commit();
                                }
                                if (point != null) {
                                    form.dimensionPoint = point;
                                    form.CreateDimension();
                                    using (Transaction tran = new Transaction(Doc, ResText.IDS_TRN_WORKPLANE)) {
                                        tran.Start();
                                        if (activeSp != null) {
                                            Doc.ActiveView.SketchPlane = activeSp;
                                        }
                                        else
                                        {
                                            Doc.ActiveView.HideActiveWorkPlane();
                                        }
                                        tran.Commit();
                                    }
                                }
                            }
                            catch (System.Exception) {
                                return Result.Succeeded;
                            }
                        }
                    }

                    if (form.DialogResult != System.Windows.Forms.DialogResult.OK) {
                        transGroup.Assimilate();
                        return Result.Succeeded;
                    }
                }
            }
        }
    }
}
