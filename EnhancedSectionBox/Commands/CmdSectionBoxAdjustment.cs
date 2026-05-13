using ADSK.JExtRAC.EnhancedSectionBox.Common;
using ADSK.JExtRAC.EnhancedSectionBox.Screen;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.EnhancedSectionBox.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdSectionBoxAdjustment : IExternalCommand
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

            var res = new Components.Attribute();

            if (UiDoc.ActiveView.GetType().Name != "View3D") {
                ComDialog.ShowDialog(res.ResourceText("IDS_WARN_TITLE"), TaskDialogIcon.TaskDialogIconWarning, res.ResourceText("IDS_WARN_NOT3DVIEW"), false);
                return Result.Succeeded;
            }

            View3D view3d = (View3D)UiDoc.ActiveView;
            if (!view3d.IsSectionBoxActive) {
                ComDialog.ShowDialog(res.ResourceText("IDS_WARN_TITLE"), TaskDialogIcon.TaskDialogIconWarning, res.ResourceText("IDS_WARN_NOSECTIONBOX"), false);
                return Result.Succeeded;
            }

            using (TransactionGroup transGroup = new TransactionGroup(Doc, res.ResourceText("IDS_TRAN_SECTIONBOX_ADJUST"))) {
                transGroup.Start(res.ResourceText("IDS_TRAN_SECTIONBOX_ADJUST"));

                while (true) {
                    FormSectionBoxAdjustment form = new FormSectionBoxAdjustment(commandData);
                    form.ShowDialog();

                    while (form.DialogResult != System.Windows.Forms.DialogResult.OK) {
                        if (form.DialogResult == System.Windows.Forms.DialogResult.Yes) {
                            transGroup.Assimilate();
                            return Result.Succeeded;
                        }
                        if (form.DialogResult == System.Windows.Forms.DialogResult.No) {
                            transGroup.RollBack();
                            return Result.Succeeded;
                        }
                    }
                }
            }
        }
    }
}
