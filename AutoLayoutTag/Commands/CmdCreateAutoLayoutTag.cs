using Autodesk.Revit.Attributes;
using ADSK.JExtRAC.AutoLayoutTag.Utils;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;
using Collections = System.Collections;

namespace ADSK.JExtRAC.AutoLayoutTag.Commands
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [TransactionAttribute(TransactionMode.Manual)]
    public class CmdCreateAutoLayoutTag : Revit.UI.IExternalCommand
    {
        // Member function

        #region Member Functions

        /// ================================================================================
        /// <summary>Command execution process</summary>
        ///
        /// <param name="commandData" >Revit command data</param>
        /// <param name="message"     >Error message</param>
        /// <param name="elements"    >Element</param>
        ///
        /// <returns>return value</returns>
        ///
        /// <history>2021/12/22 Created Applied Technology</history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                           ref string message,
                           Revit.DB.ElementSet elements)
        {
            CultureHelper.InitializeCulture();
            Revit.UI.UIApplication rvtUIApp = commandData.Application;
            Revit.UI.UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            Revit.DB.Document rvtDoc = rvtUIDoc.Document;
            RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();
            RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);
            RvtExtApp.Components.Geometry cmpGeometry = new RvtExtApp.Components.Geometry(rvtUIDoc);
            RvtExtApp.Components.Parameters cmpParameters = new RvtExtApp.Components.Parameters(cmpAttribute, rvtUIDoc);
            RvtExtApp.Components.Settings cmpSettings = new RvtExtApp.Components.Settings(rvtUIDoc);
            RvtExtApp.Components.Service cmpService = new RvtExtApp.Components.Service(cmpAttribute,
                                                                                            cmpElements,
                                                                                            cmpGeometry,
                                                                                            cmpParameters,
                                                                                            cmpSettings);

            // Return value
            Revit.UI.Result retExtCom = Revit.UI.Result.Cancelled;

            // TransactionGroup
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(rvtDoc);
            // transGroup start
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_AUTOMATIC_TAG"));

            Revit.DB.Transaction trans = new Autodesk.Revit.DB.Transaction(rvtDoc);
            try
            {
                // Transaction
                trans.Start("SetCommand");
                // Check active view is plan view or celling plan view
                Revit.DB.View activeViewAreaPlan = cmpElements.ActiveView;
                if (activeViewAreaPlan == null)
                {
                    trans.RollBack();
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_OPENVIEWPLAN"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    cmpParameters.SetSharedParamDefault();

                    transGroup.Assimilate();
                    return retExtCom;
                }
                // DtTag
                RvtExtApp.Entities.DtTag entDtTag = new RvtExtApp.Entities.DtTag(cmpAttribute,
                                                                                 cmpElements,
                                                                                 cmpGeometry,
                                                                                 cmpParameters,
                                                                                 cmpSettings);

                RvtExtApp.UI.FormConfig form = null;
                // Screen display
                while (true)
                {
                    // Create form
                    form = new RvtExtApp.UI.FormConfig(cmpAttribute, cmpElements, entDtTag);
                    form.ShowDialog();

                    while (form.DialogResult == System.Windows.Forms.DialogResult.OK && form._isSelectObject ||
                            form.DialogResult == System.Windows.Forms.DialogResult.OK && form._isSetArea)
                    {
                        if (form._isObject)
                        {
                            try
                            {
                                // Select object
                                var listElement = cmpElements.PickElements(rvtUIDoc, new RvtExtApp.Utils.SelectionElementFilter(), cmpAttribute.ResourceText("IDS_TXT_PICK_OBJECTS"));
                                if (listElement != null && listElement.Count > 0)
                                    entDtTag.LstElement = listElement;
                            }
                            catch (System.Exception ex)
                            {
                                var mess = ex.Message;
                            }

                            form._isSelectObject = false;
                            //Set data lblSelectionNumber

                            if (entDtTag.LstElement != null && entDtTag.LstElement.Count > 0)
                                form.lblSelectionNumber.Text = "( " + cmpAttribute.ResourceText("IDS_TXT_SELECTION_NUMBER") + entDtTag.LstElement.Count.ToString() + " )";

                            // Add data to data grid view settings
                            form.AddDataDgvSettings();

                            form.ShowDialog();
                        }
                        else
                        {
                            try
                            {
                                // Select pick box
                                Revit.DB.Outline outline = RvtExtApp.Components.Elements.CreateOutline(rvtUIDoc, cmpAttribute.ResourceText("IDS_TXT_PICK_AREA"));

                                if (outline != null)
                                    entDtTag.OutLine = outline;
                            }
                            catch (System.Exception ex)
                            {
                                var mess = ex.Message;
                            }

                            form._isSetArea = false;
                            form.ShowDialog();
                        }
                    }

                    if (form.DialogResult != System.Windows.Forms.DialogResult.OK)
                    {
                        trans.Commit();
                        cmpParameters.SetSharedParamDefault();
                        // Consolidate transactions
                        transGroup.Assimilate();
                        return Revit.UI.Result.Succeeded;
                    }
                    // Check has error data input
                    if (form.IsError())
                        break;
                }
                trans.Commit();
                // Create Tag
                trans.Start("CreateTag");
                if (cmpService.CreateIndependentTag(rvtDoc, entDtTag.DicCategory, entDtTag.LstElement, entDtTag.OutLine, entDtTag.ChkLeftRight,
                     entDtTag.ChkTopBottom, entDtTag.TagLeaderOtp, entDtTag.GetObjectOpt, entDtTag.AreaPremisesOpt, entDtTag.HandlePresetTagOpt) == false)
                {
                    trans.RollBack();
                    transGroup.Assimilate();
                    cmpParameters.SetSharedParamDefault();
                    return retExtCom;
                }
                rvtUIDoc.RefreshActiveView();
                trans.Commit();
                retExtCom = Revit.UI.Result.Succeeded;

                // Check element out side Crop View
                if (entDtTag.AreaPremisesOpt == 1 && rvtDoc.ActiveView.CropBoxActive)
                {
                    cmpService.CheckHasElementOutsideCropView(rvtDoc, out System.Text.StringBuilder strId);
                    // show form information
                    if (strId.Length != 0)
                    {
                        RvtExtApp.UI.FormInfo frmLog = new RvtExtApp.UI.FormInfo(cmpAttribute, strId);
                        frmLog.ShowDialog();
                    }
                }
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_COMMAND"), cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                trans.RollBack();
                transGroup.Assimilate();
                cmpParameters.SetSharedParamDefault();
                return retExtCom;
            }
            // Consolidate transactions
            transGroup.Assimilate();
            cmpParameters.SetSharedParamDefault();
            return retExtCom;
        }

        #endregion Member Functions
    }
}