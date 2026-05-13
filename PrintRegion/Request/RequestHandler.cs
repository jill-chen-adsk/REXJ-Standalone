using Autodesk.Revit.DB;
using ADSK.JExtRAC.PrintRegion.Commands;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.PrintRegion.Request
{
    public class RequestHandler
    {
        /// ================================================================================
        /// <summary> User press OK button, the command will call in here </summary>
        /// <param name="uiapp" >Revit UI app</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        public static void Execute(RequestId requestId)
        {
            switch (requestId)
            {
                case RequestId.NONE:
                    {
                        return;  // no request at this time -> we can leave immediately
                    }

                case RequestId.OK:
                    {
                        if (CmdPrint._entData == null)
                            return;

                        try
                        {
                            UIDocument uiDoc = CmdPrint._entData._rvtUIApp.ActiveUIDocument;
                            Document doc = uiDoc.Document;

                            // Set active view
                            if (uiDoc.ActiveView != null && uiDoc.ActiveView != CmdPrint._entData._viewDuplicate)
                                uiDoc.ActiveView = CmdPrint._entData._viewDuplicate;

                            // Start transaction
                            Transaction tr = new Transaction(doc);
                            tr.Start("Duplicate view");

                            // Get current view
                            View currentView = doc.ActiveView;

                            // Set view information
                            Components.Elements cmpElements = new Components.Elements(CmdPrint._entData._rvtUIApp.ActiveUIDocument);
                            cmpElements.SetInfomationView(uiDoc, currentView, CmdPrint._entData._pointPickMin, CmdPrint._entData._pointPickMax, CmdPrint._entData._viewScale);
                            //cmpElements.TrimGrid(uiDoc, currentView, CmdPrint._entData._pointPickMin, CmdPrint._entData._pointPickMax);
                            //cmpElements.HideElements(uiDoc, currentView);

                            // Print region
                            CmdPrint._entData._pMgr.SubmitPrint(currentView);

                            // Commit transaction
                            tr.Commit();

                            // Close dialog
                            if (CmdPrint._entData._printFrm != null)
                                CmdPrint._entData._printFrm.Close();

                            // Delete created view
                            cmpElements.DeleteViewCreatedAndSetActiveView(CmdPrint._entData);
                        }
                        catch (System.Exception ex)
                        {
                            var message = ex.Message;
                        }
                        break;
                    }

                case RequestId.PREVIEW:
                    {
                        if (CmdPrint._entData == null)
                            return;

                        try
                        {
                            UIDocument uiDoc = CmdPrint._entData._rvtUIApp.ActiveUIDocument;
                            Document doc = uiDoc.Document;

                            // Set active view
                            if (uiDoc.ActiveView != null && uiDoc.ActiveView != CmdPrint._entData._viewDuplicate)
                                uiDoc.ActiveView = CmdPrint._entData._viewDuplicate;

                            // Get current view
                            View currentView = doc.ActiveView;
                            if (currentView == null)
                                return;

                            // Start transaction
                            Transaction tr = new Transaction(doc);
                            tr.Start("Set view scale");

                            // Change scale
                            currentView.Scale = CmdPrint._entData._viewScale;

                            // Commit transaction
                            tr.Commit();
                        }
                        catch (System.Exception ex)
                        {
                            var message = ex.Message;
                        }

                        break;
                    }

                case RequestId.CHANGESETUP:
                    {
                        if (CmdPrint._entData == null)
                            return;

                        try
                        {
                            UIDocument uiDoc = CmdPrint._entData._rvtUIApp.ActiveUIDocument;
                            Document doc = uiDoc.Document;

                            // Set active view
                            if (uiDoc.ActiveView != null && uiDoc.ActiveView != CmdPrint._entData._viewDuplicate)
                                uiDoc.ActiveView = CmdPrint._entData._viewDuplicate;

                            // Start transaction
                            Transaction tr = new Transaction(doc);
                            tr.Start("Change Print Setup");

                            // Change setup
                            CmdPrint._entData._pMgr.ChangePrintSetup();

                            // Commit transaction
                            tr.Commit();
                        }
                        catch (System.Exception ex)
                        {
                            var message = ex.Message;
                        }

                        break;
                    }

                case RequestId.CANCEL:
                    {
                        UIDocument uiDoc = CmdPrint._entData._rvtUIApp.ActiveUIDocument;

                        Components.Elements cmpElements = new Components.Elements(CmdPrint._entData._rvtUIApp.ActiveUIDocument);

                        // Set active view
                        uiDoc.ActiveView = CmdPrint._entData._viewCurrent;

                        // Delete created view
                        cmpElements.DeleteViewCreatedAndSetActiveView(CmdPrint._entData);

                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return;
        }
    }
}
