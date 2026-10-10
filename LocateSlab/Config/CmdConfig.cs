using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.LocateSlab.Utils;

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

            IntPtr revitHandle = WeaveDialogHost.RevitWindowHandle;
            string okText = cmpAttribute.ResourceText("IDS_TXT_OK");
            string errorTitle = cmpAttribute.ResourceText("IDS_TXT_ERROR");

            string WithSkippedBeamNote(string text)
            {
                if (cmpService.SkippedBeamCount == 0)
                    return text;
                return text + "\n\n" + string.Format(
                    cmpAttribute.ResourceText("IDS_INFO_SKIPPED_BEAMS"),
                    cmpService.SkippedBeamCount,
                    string.Join(", ", cmpService.SkippedBeamUsages));
            }

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
                    WeaveDialogHost.ShowMessage(revitHandle,
                        cmpAttribute.ResourceText("IDS_INFO_ACTIVE_VIEW"), errorTitle, okText);
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                var elemBeams = cmpService.GetSelSetBeams();
                if (elemBeams.Count == 0)
                {
                    WeaveDialogHost.ShowMessage(revitHandle,
                        WithSkippedBeamNote(cmpAttribute.ResourceText("IDS_INFO_NO_EXIST_BEAMS")),
                        errorTitle, okText);
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                var elemFloorTypes = cmpElements.FloorTypes;
                if (elemFloorTypes.Count == 0)
                {
                    WeaveDialogHost.ShowMessage(revitHandle,
                        cmpAttribute.ResourceText("IDS_ERR_NO_FLOOR_TYPE"), errorTitle, okText);
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                trans.Start("SetCommand");

                var entDtSlabType = new Entities.DtSlabType(cmpAttribute, cmpElements,
                    cmpGeometry, cmpParameters, cmpSettings);
                if (entDtSlabType.ErrMsg != "")
                {
                    WeaveDialogHost.ShowMessage(revitHandle, entDtSlabType.ErrMsg, errorTitle, okText);
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
                    WeaveDialogHost.ShowMessage(revitHandle, entDtCmd.ErrMsg, errorTitle, okText);
                    trans.RollBack();
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                trans.Commit();

                var form = new FormConfigWPF(cmpAttribute, entDtSlabType, entDtCmd);
                if (WeaveDialogHost.ShowDialog(form, revitHandle) != true)
                {
                    cmpParameters.SetSharedParamDefault();
                    transGroup.Assimilate();
                    return retExtCom;
                }

                trans.Start("CreateSlab");

                if (!cmpService.CreateSlab(entDtSlabType, elemBeams,
                    entDtCmd.Data[0], entDtCmd.Data[2]))
                {
                    WeaveDialogHost.ShowMessage(revitHandle,
                        WithSkippedBeamNote(cmpService.ErrMsg), errorTitle, okText);
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
                WeaveDialogHost.ShowMessage(revitHandle,
                    cmpAttribute.ResourceText("IDS_ERR_COMMAND"), errorTitle, okText);
                if (trans.GetStatus() != TransactionStatus.Committed)
                    trans.RollBack();
            }

            cmpParameters.SetSharedParamDefault();
            transGroup.Assimilate();
            return retExtCom;
        }
    }
}
