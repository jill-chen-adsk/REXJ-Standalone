using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Resource;
using CmdDuctDisplacement.UI.Common;
using CmdDuctDisplacement.UI.Controller;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.Model.Entity;
using CmdDuctDisplacement.UI.View;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media;
using TaskDialog = Autodesk.Revit.UI.TaskDialog ;

namespace CmdDuctDisplacement.Logic
{
    public class CmdDuctDisplacement
    {
        private ExternalCommandData commandData;
        private Logger log;

        /// <summary>
        /// Creates the command façade for the routed displacement workflow.
        /// </summary>
        /// <param name="commandData"></param>
        public CmdDuctDisplacement(ExternalCommandData commandData)
        {
            this.commandData = commandData;
            int min, max;
            if (!int.TryParse(ExResources.ResxString(DuctDisplacementDefine.LOG_LEVEL_MAX), out max))
            {
                max = DuctDisplacementDefine.LOG_LEVEL_MAX_DEF;
            }
            if (!int.TryParse(ExResources.ResxString(DuctDisplacementDefine.LOG_LEVEL_MIN), out min))
            {
                min = DuctDisplacementDefine.LOG_LEVEL_MIN_DEF;
            }
            this.log = new Logger(max, min, DuctDisplacementDefine.LOG_FOLDER_PATH_DEF);
        }

        /// <summary>
        /// Main command interpreter loop.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Result Main(DuctDisplacementDefine.Frow frow)
        {
            bool result = true;

            // Placement-zone group visibility toggle.
            string setdisplay = "Visible";

            bool windowloopflag;

            bool escflag = false;

            var controlstatus = ControlStatus.Instance;
            var windowsprop = WindowReceiveProperty.Instance;

            MessageBoxButtons buttons = MessageBoxButtons.OK;
            string message = null;

            CalCulation cal = new CalCulation();
            WindowControl windowcontrol = new WindowControl();

            Result res = Result.Succeeded;
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;


            WrpGeometry geometry = new WrpGeometry(uidoc, log);
            MEPOperation _mep = new MEPOperationForDuct(app, uidoc, log);
            WrpViews _view = new WrpViews(uidoc, log);
            // Tracks whether optional S-curve families were loaded for this command session.
            Dictionary<int, bool> doneSCurveLoad = new Dictionary<int, bool>();
            doneSCurveLoad.Add((int)DuctDisplacementDefine.S_CURVE_PTN.RECT, false);
            doneSCurveLoad.Add((int)DuctDisplacementDefine.S_CURVE_PTN.ROUND, false);
            doneSCurveLoad.Add((int)DuctDisplacementDefine.S_CURVE_PTN.OVAL, false);
            doneSCurveLoad.Add((int)DuctDisplacementDefine.S_CURVE_PTN.PIPE, false);

            if (!_view.IsViewPlan())
            {
                TaskDialog.Show(ExResources.ResxString(DuctDisplacementDefine.DIALOG_TITLE_WARN), ExResources.ResxString(DuctDisplacementDefine.MSG_WARN2));
                log.Trace("MIss User Operation ViewSelect");
                return Result.Succeeded;
            }

            Autodesk.Revit.DB.View currView = doc.ActiveView;
            uidoc.ActiveView = currView;

            this.WindowStandPreparation_Init(out setdisplay);
            this.WindowStandPreparation_SetProperty(uidoc);

            log.Trace("Operation:" + frow);

            while (true)
            {
                using (TransactionGroup txGrp = new TransactionGroup(doc))
                {
                    txGrp.Start("CmdDuctDisplacement");
                    log.Info("Transaction group started.");
                    result = true;
                    try
                    {

                        windowloopflag = true;

                        res = _mep.PickCutPoints(/*pickPtnForm.GetPtn()*/2);
                        log.Trace("One Two Point Get:" + res.ToString());

                        if (res == Result.Failed)
                        {
                            txGrp.RollBack();
                            _mep = new MEPOperationForDuct(app, uidoc, log);
                            continue;
                        }
                        else if (res == Result.Cancelled)
                        {
                            log.Trace("Pick Point res:" + res);
                            return Result.Succeeded;
                        }

                        // Swap to piping logic when both picked references are piping.
                        if (!_mep.IsDuct() && _mep.IsPipe())
                        {
                            _mep = new MEPOperationForPipe(app, uidoc, log,
                                _mep.Curve1, _mep.Pt1, _mep.Curve2, _mep.Pt2, _mep.SLineId, _mep.ELineId, _mep.view1);

                        }

                        if (setdisplay != "Visible")
                        {
                            controlstatus.clear();
                        }

                        // Open the auxiliary window seeded with routed segment context.
                        controlstatus.CallRoute = DuctDisplacementDefine.TextChangeRoute.PointClick_1_2;
                        ConnectionSettingView Window = new ConnectionSettingView(_mep);

                        // Extra pick passes for clearance references.
                        if (frow == DuctDisplacementDefine.Frow.ThreePick_GeneralModel ||
                            frow == DuctDisplacementDefine.Frow.ThreePick_LinkdModel)
                        {
                            int offsetpos;
                            bool modeltypeflag = true;
                            string refline = windowcontrol.GetInitRefalenceLine(_mep, DuctDisplacementDefine.InstructionObj.MoveObj_1);
                            controlstatus.CallRoute = DuctDisplacementDefine.TextChangeRoute.PointClick_3;
                            offsetpos = windowcontrol.RefalenceLineConvert(refline);

                            if (frow == DuctDisplacementDefine.Frow.ThreePick_GeneralModel)
                            {
                                log.Trace("3Pick Type:" + frow);
                                modeltypeflag = false;
                            }
                            else if (frow == DuctDisplacementDefine.Frow.ThreePick_LinkdModel)
                            {
                                log.Trace("3Pick Type:" + frow);
                                modeltypeflag = true;
                            }
                            else
                            {
                                log.Error("3Pick Type");
                                modeltypeflag = false;
                            }

                            res = ThreepickMethod(_mep, ref setdisplay, Window, out escflag, offsetpos, modeltypeflag);

                            // Abort the command when ESC cancels third pick intake.
                            if (escflag == true)
                            {
                                log.Trace("3Pick cancel");
                                return Result.Succeeded;
                            }

                        }

                        while (windowloopflag)
                        {

                            SpecificWindowProperty_ReSet(Window, setdisplay);
                            log.Trace("WindowDisplay:" + setdisplay);
                            Window.ShowDialog();
                            log.Trace("ControlStatus.ButtonType:" + controlstatus.ButtonType);
                            switch (controlstatus.ButtonType)
                            {
                                case (DuctDisplacementDefine.WindowReturnNum.NoSelect):
                                    log.Error("Check ControlStatus.ButtonType:" + controlstatus.ButtonType);
                                    setdisplay = "Collapsed";
                                    windowloopflag = true;
                                    break;
                                case (DuctDisplacementDefine.WindowReturnNum.Cancel):
                                    setdisplay = "Collapsed";
                                    windowloopflag = false;
                                    break;
                                case (DuctDisplacementDefine.WindowReturnNum.OK):
                                    setdisplay = "Collapsed";
                                    windowloopflag = false;

                                    log.Info("Set ModDuctLevelPartially" + "(MovingMethodType:" + windowsprop.MovingMethodType() + " ElbowType:" + windowsprop.ElbowType() + " MovingValue:" + windowsprop.FlValue + ")");
                                    result = _mep.ModDuctLevelPartially(windowsprop.MovingMethodType(), windowsprop.ElbowType(), windowsprop.FlValue, ref doneSCurveLoad, out message);
                                    break;
                                case (DuctDisplacementDefine.WindowReturnNum.EndRoutine):
                                    setdisplay = "Collapsed";
                                    windowloopflag = false;
                                    log.Info("Successful completion");
                                    return Result.Succeeded;
                                case (DuctDisplacementDefine.WindowReturnNum.GraphicInstructions_General):
                                    windowloopflag = true;
                                    res = ThreepickMethod(_mep, ref setdisplay, Window, out escflag, windowsprop.ReferenceLineType(), false);
                                    if (escflag == true)
                                    {
                                        log.Trace("Target obj select cancel <General model>");
                                    }
                                    break;
                                case (DuctDisplacementDefine.WindowReturnNum.GraphicInstructions_Linkd):
                                    windowloopflag = true;
                                    res = ThreepickMethod(_mep, ref setdisplay, Window, out escflag, windowsprop.ReferenceLineType(), true);
                                    if (escflag == true)
                                    {
                                        log.Trace("Target obj select cancel <Linkd Model>");
                                    }
                                    break;
                                default:
                                    log.Error("Check ControlStatus.ButtonType Out");
                                    break;
                            }
                        }

                        _mep.DeleteCutLines();

                        _mep = new MEPOperationForDuct(app, uidoc, log);

                        if (result)
                        {
                            txGrp.Assimilate();
                            log.Info("Transaction assimilated.");
                        }
                        else
                        {
                            txGrp.RollBack();
                            log.Info("Transaction rolled back.");

                            string caption = DuctDisplacementDefine.DIALOG_TITLE_WARN;
                            MessageBox.Show(message, ExResources.ResxString(caption), buttons);
                        }

                    }
                    catch (Exception ex)
                    {
                        string messege = DuctDisplacementDefine.Worn_ExceptionMessege;
                        string caption = DuctDisplacementDefine.Worn_Caption;
                        log.Error(ex.GetType().ToString() + ":" + ex.Message);
                        log.Error("[Source]" + ex.Source);
                        log.Error("[StackTrace]" + ex.StackTrace);
                        MessageBox.Show(ExResources.ResxString(messege), ExResources.ResxString(caption), buttons);

                        txGrp.RollBack();

                        log.Info("Transaction rolled back.");

                        _mep = new MEPOperationForDuct(app, uidoc, log);
                        setdisplay = "Collapsed";

                    }
                }

            }
        }

        /// <summary>
        /// Initializes UI-control singletons before prompting the dialog.
        /// </summary>
        /// <param name="setdisplay">Collapsed/Visible sentinel for avoidance UI.</param>
        public void WindowStandPreparation_Init(out string setdisplay)
        {
            log.Trace(MethodBase.GetCurrentMethod().Name);
            var controlstatus = ControlStatus.Instance;
            controlstatus.Init();
            setdisplay = "Collapsed";
        }

        /// <summary>
        /// Seeds default elbow options on singleton property storage.
        /// </summary>
        /// <param name="uidoc">Active UIDocument.</param>
        public void WindowStandPreparation_SetProperty(UIDocument uidoc)
        {
            log.Trace(MethodBase.GetCurrentMethod().Name);
            ControlStatus controlstatus = ControlStatus.Instance;
            WindowReceiveProperty windowreceiveproperty = WindowReceiveProperty.Instance;

            controlstatus.SetProperty(uidoc);
            windowreceiveproperty.FortyFiveElbowRadioButton = true;
            windowreceiveproperty.NinetyElbowRadioButton = false;
            windowreceiveproperty.ScarveElbowRadioButton = false;
        }

        /// <summary>
        /// Resynchronizes avoidance UI visibility toggles whenever the pane reopens.
        /// </summary>
        /// <param name="Window">Connection settings window.</param>
        /// <param name="setdisplay">Collapsed/Visible sentinel for avoidance UI.</param>
        public void SpecificWindowProperty_ReSet(ConnectionSettingView Window, string setdisplay)
        {
            log.Trace(MethodBase.GetCurrentMethod().Name);
            WindowReceiveProperty windowreceiveproperty = WindowReceiveProperty.Instance;
            ConnectionSettingViewModel ductp = Window.GetviewModel();
            ductp.SetDisplay_TOBJ = setdisplay;
            windowreceiveproperty.FortyFiveElbowRadioButton = true;
            windowreceiveproperty.NinetyElbowRadioButton = false;
            windowreceiveproperty.ScarveElbowRadioButton = false;
        }

        /// <summary>
        /// Picks obstruction graphics to calculate vertical offsets relative to routed segment.
        /// </summary>
        /// <param name="mep">Active MEPOperation instance.</param>
        /// <param name="setdisplay">Collapsed/Visible sentinel for avoidance UI.</param>
        /// <param name="Window">Dialog surface that receives calculated values.</param>
        /// <param name="escflag">True when the user escapes the pick prompts.</param>
        /// <param name="refline">Internal reference-line enumerator.</param>
        /// <param name="selectobjmodeltype"><c>true</c> linked model pick; <c>false</c> host model.</param>
        /// <returns>Command outcome for the supplementary pick passes.</returns>
        public Result ThreepickMethod(MEPOperation mep, ref string setdisplay, ConnectionSettingView Window, out bool escflag, int refline, bool selectobjmodeltype)
        {
            WindowReceiveProperty windowsprop = WindowReceiveProperty.Instance;
            ConnectionSettingViewModel ductp = Window.GetviewModel();
            RoundNum roundnum = new RoundNum();
            CalCulation calculation = new CalCulation();
            WindowControl cont = new WindowControl();

            double FireProofingValue = 0;

            double hDiff = 0;
            double clearance = 0;
            double offset = 0;

            int dir = DuctDisplacementDefine.DIR_DOWN;

            Result res = Result.Succeeded;
            res = mep.PickTargetAndDirectionToAvoid(ref dir, selectobjmodeltype);

            if (res == Result.Succeeded)
            {
                setdisplay = "Visible";
                log.Trace("Target Obj Select:succece");
            }

            else
            {
                escflag = true;
                log.Trace("Target Obj Select:Cancel");
                return Result.Succeeded;
            }

            if (windowsprop.FireProofingType())
            {
                FireProofingValue = windowsprop.FireProofingValue;
            }

            else
            {
                FireProofingValue = 0;
            }

            // Compute finish level offset versus clearance from the obstruction pick.
            log.Trace("CalculateDiff param in offsetPos:" + refline + "direction:" + dir +
                "roundUnit:" + windowsprop.RoundType() + "minClear:" + windowsprop.BetweenObjValue + "insulate:" + FireProofingValue);
            res = mep.CalculateDiff(out hDiff, out clearance, out offset, refline, dir, windowsprop.RoundType(), windowsprop.BetweenObjValue, FireProofingValue);
            log.Trace("CalculateDiff param out hDiff:" + hDiff + "clearance:" + clearance + "offset:" + offset);

            ductp.LevelButtonName = ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel);

            ductp.TargetObjName = mep.GetFamilyName(DuctDisplacementDefine.InstructionObj.TargetObj);

            if (dir == DuctDisplacementDefine.DIR_DOWN)
            {
                ductp.TopArrangementColor = Brushes.LightGray.ToString();
                ductp.BottomArrangementColor = Brushes.LightSteelBlue.ToString();
                cont.UpDownButtonSwapColor(ductp, DuctDisplacementDefine.PressButton.Bottom);
                cont.UpDownObjLabelSwapColor(ductp, DuctDisplacementDefine.PressButton.Bottom);
            }

            else if (dir == DuctDisplacementDefine.DIR_UPPER)
            {
                ductp.TopArrangementColor = Brushes.LightSteelBlue.ToString();
                ductp.BottomArrangementColor = Brushes.LightGray.ToString();
                cont.UpDownButtonSwapColor(ductp, DuctDisplacementDefine.PressButton.Top);
                cont.UpDownObjLabelSwapColor(ductp, DuctDisplacementDefine.PressButton.Top);
            }
            else
            {
                //error
                log.Error("Check dir Out:" + dir);
                ductp.TopArrangementColor = Brushes.LightGray.ToString();
                ductp.BottomArrangementColor = Brushes.LightSteelBlue.ToString();
            }

            ductp.DuctOffsetLevel = offset.ToString();
            ductp.IsolationValue = clearance;

            escflag = false;
            return res;
        }
    }


}
