using Revit = Autodesk.Revit;
using ADSK.JExtRAC.PrintRegion.Components;
using ADSK.JExtRAC.PrintRegion.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.PrintRegion.Request;
using ADSK.JExtRAC.PrintRegion.Entities;
using ADSK.JExtRAC.PrintRegion.Utils;
using System.Linq;

namespace ADSK.JExtRAC.PrintRegion.Commands
{
    /// ================================================================================
    /// <summary>Command Print Region</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdPrint : IExternalCommand
    {
        #region Member Variables

        /// <summary>Class data</summary>
        public static EntitiesData _entData = null;

        #endregion Member Variables

        /// ================================================================================
        /// <summary>Command execution processing</summary>
        ///
        /// <param name="commandData" >Revit Command data</param>
        /// <param name="message"     >Error message</param>
        /// <param name="elements"    >Error element</param>
        ///
        /// <returns>Execution result</returns>
        ///
        /// <history>2022/01/17 Created Applied Technology</history>
        /// ================================================================================

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            CultureHelper.InitializeCulture();

            if (_entData == null)
                _entData = new EntitiesData();

            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            Attribute cmpAttribute = new Attribute();
            Elements cmpElements = new Elements(rvtUIDoc);
            _entData._rvtUIApp = rvtUIApp;

            using (TransactionGroup transGroup = new TransactionGroup(rvtUIDoc.Document)) {
                transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_TRANSACTION_PRINT_REGION"));

                if (cmpElements.PickPoints(cmpAttribute, rvtUIDoc, out _entData._pointPickMin, out _entData._pointPickMax) == false)
                    return Result.Cancelled;
                // 現ビューのスケールの利用
                _entData._viewScale = rvtUIDoc.ActiveView.Scale;

                // Start transaction
                using (Transaction tr = new Transaction(cmpElements.RvtDBDoc)) {
                    try {
                        tr.Start("Set information view");

                        var viewDuplicate = cmpElements.DuplicateView(rvtUIDoc);
                        if (viewDuplicate == null) {
                            tr.RollBack();
                            return Result.Cancelled;
                        }

                        // Set crop box and scale default to view
                        cmpElements.SetInfomationView(rvtUIDoc, viewDuplicate, _entData._pointPickMin, _entData._pointPickMax, _entData._viewScale);
                        // 通芯のトリミング
                        cmpElements.TrimGrid(rvtUIDoc, viewDuplicate, _entData._pointPickMin, _entData._pointPickMax);
                        // 断面ビューなどを非表示
                        cmpElements.HideElements(rvtUIDoc, viewDuplicate);

                        // Commit transaction
                        tr.Commit();

                        // Set active view preview
                        _entData._viewCurrent = rvtUIApp.ActiveUIDocument.ActiveView;
                        _entData._viewDuplicate = viewDuplicate;
                        rvtUIApp.ActiveUIDocument.ActiveView = viewDuplicate;
                        // Zoom and center the view to a specified rectangle.
                        UIView uiView = rvtUIDoc.GetOpenUIViews().FirstOrDefault(x => x.ViewId.ToString() == viewDuplicate.Id.ToString());
                        if (uiView != null)
                            uiView.ZoomToFit();
                        rvtUIApp.ActiveUIDocument.RefreshActiveView();

                        _entData._pMgr = new PrintMgr(commandData, cmpAttribute);
                        if (_entData._pMgr.InstalledPrinterNames.Count == 0) {
                            PrintMgr.MyMessageBox(cmpAttribute.ResourceText("IDS_ERR_NO_PRINTER"));
                            return Result.Cancelled;
                        }

                        // Get current process
                        System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
                        System.IntPtr h = process.MainWindowHandle;

                        _entData._printFrm = new PrintFrmWPF(cmpAttribute, cmpElements, _entData._pMgr);

                        // Off dialogbox showing
                        _entData._rvtUIApp.DialogBoxShowing += _rvtUIApp_DialogBoxShowing;

                        // Set handle
                        _entData._revitHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                        // Show form
                        WeaveDialogHost.ShowDialog(_entData._printFrm, h);

                    }
                    catch (System.Exception) {
                        if (tr.HasStarted())
                            tr.RollBack();
                    }
                }
                transGroup.RollBack();

                return Result.Succeeded;
            }
        }

        /// ================================================================================
        /// <summary>Dialog show warning</summary>
        ///
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ///
        /// <history>2022/01/17 Created Applied Technology</history>
        /// ================================================================================
        private void _rvtUIApp_DialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
        {
            Attribute cmpAttribute = new Attribute();
            TaskDialogShowingEventArgs taskdialog = e as TaskDialogShowingEventArgs;
            if (taskdialog != null && taskdialog.Message.Contains(cmpAttribute.ResourceText("IDS_TXT_SHOW_LOG_JPN"))
                || taskdialog != null && taskdialog.Message.Contains(cmpAttribute.ResourceText("IDS_TXT_SHOW_LOG_ENG")))
            {
                e.OverrideResult((int)TaskDialogResult.Close);
            }
        }
    }
}
