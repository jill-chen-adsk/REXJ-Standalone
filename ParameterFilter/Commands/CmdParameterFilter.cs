using ADSK.JExtRAC.ParameterFilter.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.ParameterFilter;

namespace ADSK.JExtRAC.ParameterFilter.Commands
{
    /// ================================================================================
    /// <summary>Command parameter filter</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdParameterFilter : Revit.UI.IExternalCommand
    {
        #region Member Variables

        /// <summary>Revit UIアプリケーション</summary>
        public static Revit.UI.UIApplication _RvtUIApp;

        #endregion Member Variables

        // Member Functions

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

            try
            {
                Revit.UI.Result result = Revit.UI.Result.Cancelled;
                List<Element> selElems = cmpElements.SelElems.ToList();

                // Get element in group
                if (selElems.Count == 1 && selElems[0] is Group)
                {
                    Group group = selElems[0] as Group;
                    selElems.Clear();
                    foreach (ElementId memberId in (IEnumerable<ElementId>)group.GetMemberIds())
                    {
                        Element element = rvtUIDoc.Document.GetElement(memberId);
                        selElems.Add(element);
                    }
                }

                // Show mess when user didn't select element
                if (selElems.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOELEMENTSELECT"), cmpAttribute.ResourceText("IDS_ERR_ERROR"), System.Windows.Forms.MessageBoxButtons.OK);
                    return result;
                }

                // Get value
                var objectElements = cmpElements.GetDataElement(selElems);

                // Show form
                FormParameterFilter formFilter = new FormParameterFilter(rvtUIDoc, cmpAttribute, cmpElements, objectElements);
                formFilter.ShowDialog();
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"), cmpAttribute.ResourceText("IDS_ERR_ERROR"));

                return Revit.UI.Result.Failed;
            }
            return Revit.UI.Result.Succeeded;
        }

        #endregion Member Functions
    }
}