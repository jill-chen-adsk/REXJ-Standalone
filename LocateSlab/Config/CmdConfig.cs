using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.LocateSlab.Config
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdConfig : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var rvtUIApp = commandData.Application;
            var rvtUIDoc = rvtUIApp.ActiveUIDocument;
            var cmpAttribute = new Components.Attribute();
            var cmpElements = new Components.Elements(rvtUIDoc);
            var cmpGeometry = new Components.Geometry(rvtUIDoc);
            var cmpParameters = new Components.Parameters(cmpAttribute, rvtUIDoc);
            var cmpSettings = new Components.Settings(rvtUIDoc);
            var cmpService = new Components.Service(cmpAttribute, cmpElements, cmpGeometry,
                cmpParameters, cmpSettings);

            var retExtCom = Result.Cancelled;
            var transGroup = new TransactionGroup(cmpElements.RvtDBDoc);
            transGroup.Start(cmpAttribute.ResourceText("IDS_TXT_LOCATESLAB"));
            var trans = new Transaction(cmpElements.RvtDBDoc);

            try
            {
                var activeView = rvtUIDoc.ActiveView;
                if (activeView.ViewType != ViewType.FloorPlan &&
                    activeView.ViewType != ViewType.CeilingPlan &&
                    activeView.ViewType != ViewType.AreaPlan &&
                    activeView.ViewType != ViewType.EngineeringPlan)
                {
                    MessageBox.Show(
                        cmpAttribute.ResourceText("IDS_INFO_ACTIVE_VIEW"),
                        cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                var elemBeams = cmpService.GetSelSetBeams();
                if (elemBeams.Count == 0)
                {
                    MessageBox.Show(
                        cmpAttribute.ResourceText("IDS_INFO_NO_EXIST_BEAMS"),
                        cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                var elemFloorTypes = cmpElements.FloorTypes;
                if (elemFloorTypes.Count == 0)
                {
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                trans.Start("SetCommand");

                var entDtSlabType = new Entities.DtSlabType(cmpAttribute, cmpElements,
                    cmpGeometry, cmpParameters, cmpSettings);
                if (entDtSlabType.ErrMsg != "")
                {
                    MessageBox.Show(entDtSlabType.ErrMsg, cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }
                entDtSlabType.GetData(elemFloorTypes);

                var elemProjInfo = cmpElements.ProjectInfo;
                var entDtCmd = new Entities.DtCmd(cmpAttribute, cmpElements, cmpGeometry,
                    cmpParameters, cmpSettings, elemProjInfo,
                    cmpAttribute.ResourceText("IDS_SHPARAM_DEF_CMD_LOCATESLAB"), 3);

                if (entDtCmd.ErrMsg != "")
                {
                    MessageBox.Show(entDtCmd.ErrMsg, cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                trans.Commit();

                var form = new FormConfig(cmpAttribute, entDtSlabType, entDtCmd);
                form.ShowDialog();
                if (form.DialogResult != DialogResult.OK)
                {
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                trans.Start("CreateSlab");

                if (!cmpService.CreateSlab(entDtSlabType, elemBeams,
                    entDtCmd.Data[0], entDtCmd.Data[2]))
                {
                    MessageBox.Show(cmpService.ErrMsg, cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                cmpElements.RvtDBDoc.Regenerate();
                trans.Commit();

                trans.Start("SetParamValue");
                entDtCmd.SetData();
                trans.Commit();

                retExtCom = Result.Succeeded;
            }
            catch (Exception)
            {
                MessageBox.Show(
                    cmpAttribute.ResourceText("IDS_ERR_COMMAND"),
                    cmpAttribute.ResourceText("IDS_TXT_ERROR"));
                if (trans.GetStatus() != TransactionStatus.Committed)
                    trans.RollBack();
            }

            cmpParameters.SetSharedParamDefault();
            transGroup.Assimilate();
            return retExtCom;
        }
    }
}
