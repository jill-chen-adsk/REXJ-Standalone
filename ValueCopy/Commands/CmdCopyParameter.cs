using ADSK.JExtRAC.ValueCopy.Entities;
using ADSK.JExtRAC.ValueCopy.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.ValueCopy;

namespace ADSK.JExtRAC.ValueCopy.Commands
{
    /// ================================================================================
    /// <summary>Command parameter filter</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdCopyParameter : Revit.UI.IExternalCommand
    {
        #region Member Variables

        /// <summary>Error message</summary>
        public static StringBuilder errorMess = null;

        #endregion Member Variables

        #region Member Functions

        /// ================================================================================
        /// <summary>Command execution processing</summary>
        ///
        /// <param name="commandData" >Revit Command data</param>
        /// <param name="message"     >Error message</param>
        /// <param name="elements"    >element</param>
        ///
        /// <returns>Execution result</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            Document rvtDbDoc = rvtUIApp.ActiveUIDocument.Document;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);

            // Get current unit display
            Units units = rvtDbDoc.GetUnits();

            // Start transaction
            TransactionGroup transGroup = null;
            try
            {
                errorMess = new StringBuilder();

                List<Element> elementNeedTrafer = cmpElements.GetElementSelected(rvtUIDoc);
                if (elementNeedTrafer.Count == 0)
                {
                    MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOSELECTELEMENT"));
                    return Revit.UI.Result.Cancelled;
                }

                var elementPicked = cmpElements.PickElement(rvtUIDoc, cmpAttribute.ResourceText("IDS_TXT_PICKSOURCE"));
                if (elementPicked == null)
                    return Revit.UI.Result.Cancelled;

                transGroup = new TransactionGroup(rvtDbDoc, "Copy parameter");
                transGroup.Start();

                // Set all unit display to none prefix
                var currentUnit = cmpElements.SetProjectUnitDisplayToNone(rvtDbDoc);

                // Get data element
                ObjectElement objElement = new ObjectElement(elementPicked);
                objElement.GetParameterAndGroupParamter();

                // Show form parameter
                FormParameterWPF frm = new FormParameterWPF(cmpAttribute, objElement);
                if (frm.ShowDialog() != true)
                    return Revit.UI.Result.Cancelled;

                // Convert to object report
                List<ObjectReportCopy> ObjectReports = new List<ObjectReportCopy>();
                foreach (var ele in elementNeedTrafer)
                {
                    ObjectReportCopy objReport = new ObjectReportCopy(ele);
                    objReport.GetParameterName();
                    objReport.GetFamilyAndTypeNameElement(rvtDbDoc);
                    ObjectReports.Add(objReport);
                }

                // Start copy parameter
                cmpElements.SetParmeterToElement(rvtDbDoc, objElement, ObjectReports);

                // Set unit to Previous
                cmpElements.SetPreviousProjectUnitDisplay(rvtDbDoc, units);

                FormReportWPF frmReport = new FormReportWPF(cmpElements, cmpAttribute, ObjectReports, errorMess);
                frmReport.ShowDialog();
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;

                MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"), cmpAttribute.ResourceText("IDS_ERR_ERROR"));

                return Revit.UI.Result.Failed;
            }
            finally
            {
                // Commit transaction
                if (transGroup != null)
                    transGroup.Assimilate();
            }
            return Revit.UI.Result.Succeeded;
        }

        #endregion Member Functions
    }
}
