using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Logic;
using CmdDuctDisplacement.Resource;
using CmdDuctDisplacement.UI.Commands.DuctConnection;
using CmdDuctDisplacement.UI.Common;
using CmdDuctDisplacement.UI.Controller;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.Model.Entity;
using CmdDuctDisplacement.UI.Model.InheritBase;
using CmdDuctDisplacement.UI.View;
using RevitMEPAddin.Common;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.ViewModel
{
    /// <summary>
    /// View-model for <see cref="ConnectionSettingView"/> bindings.
    /// </summary>
    public class ConnectionSettingViewModel : ModelBase
    {
        #region Memeber Variables
        private Logger log;
        private PropertySet popset;

        private ConnectionSettingView uiwindow;
        private MEPOperation mep;

        private string _SelectObjName;
        private string _TargetObjName;
        private string _SelectObjName_MoveMethod;
        private string _Location_TopObj;
        private string _Location_BottomObj;
        private string _SelectObjName_Move;
        private string _MoveOptionShow;
        private string _TopObjAddColorName;
        private string _BottomObjAddColorName;

        private bool _OffsetRadioButton;
        private bool _UnityRadioButton;
        private bool _FortyFiveElbowRadioButton;
        private bool _NinetyElbowRadioButton;
        private bool _ScarveElbowRadioButton;

        private bool _MoveOptionExpandedFlag;

        private string _OptionFontWeight;

        private bool _FiftyButton;
        private bool _OneHandredButton;
        private bool _NothingButton;

        private bool _FireProofingValidButton;
        private bool _FireProofingInValidButton;
        private string _SetDisplayFireProofing;

        private string _BetweenObjValue;

        private string _FireProofingValue;

        /// <remarks>For display/editing bind to <see cref="DuctOffsetLevel"/>; for numeric logic use <see cref="InternalDuctOffsetLevel"/>.</remarks>
        private string _DuctOffsetLevel;
        private double _InternalDuctOffsetLevel;
        //private double _DuctOffsetLevel_RoundOnlyNum;
        private string _LevelButtonName;
        private string _DuctReferenceLine;
        private bool _BaseLineControlFlag;
        private double _SelctObj_InitalFL;
        private double _IsolationValue;
        private string _IsolationValueColor;
        private string _IsolationWorningMessage;

        private string _TopArrangementColor;
        private string _BottomArrangementColor;
        private string _Location_BottomObjColor;
        private string _Location_TopObjColor;

        private string _SetDisplay_TOBJ;
        #endregion

        #region Constructor
        public ConnectionSettingViewModel(ConnectionSettingView callclass, MEPOperation _mep)
        {
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

            PropertySet _popset = new PropertySet(this, _mep);
            this.popset = _popset;

            mep = _mep;

            uiwindow = callclass;
            ConnectionOKCommand = new OKCommand(this, log);
            CancelCommand = new CancelCommand(this, log);
            DecreaseCommand = new DecreaseCommand(this, log);
            IncreaseCommand = new IncreaseCommand(this, log);
            MoveDIstanceCalCommand = new MoveDistanceCalCommand(this, log);
            MoveDIstanceCalLinkdCommand = new MoveDistanceCalLinkdCommand(this, log);
            SwitchCalMethodCommand = new SwitchCalMethodCommand(this, log, mep);
            TopArrangementCommand = new TopArrangementCommand(this, log, mep);
            BottomArrangmentCommand = new BottomArrangmentCommand(this, log, mep);
            EndRoutineCommand = new EndRoutineCommand(this, log);

            KeyDownCommand = new KeyDownCommand(this, log);
            FLTextKeyboardFocusCommand = new FLTextKeyboardFocusCommand(this, log);
            FLPreviewTextCommand = new FLPreviewTextCommand(this, log, uiwindow);
            FLTextBoxLostFocusCommand = new FLTextBoxLostFocusCommand(this, log, uiwindow);
            FireProofingPreviewTextCommand = new FireProofingPreviewTextCommand(this, log, uiwindow);
            FireProofingLostFocusCommand = new FireProofingLostFocusCommand(this, log, uiwindow);
            BetweenPreviewTextCommand = new BetweenPreviewTextCommand(this, log, uiwindow);
            BetweenLostFocusCommand = new BetweenLostFocusCommand(this, log, uiwindow);
            StartWork();
        }
        #endregion

        #region Properties
        #region Command

        public ICommand ConnectionOKCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand DecreaseCommand { get; set; }
        public ICommand IncreaseCommand { get; set; }
        public ICommand MoveDIstanceCalCommand { get; set; }
        public ICommand MoveDIstanceCalLinkdCommand { get; set; }
        public ICommand SwitchCalMethodCommand { get; set; }
        public ICommand TopArrangementCommand { get; set; }
        public ICommand BottomArrangmentCommand { get; set; }
        public ICommand EndRoutineCommand { get; set; }
        public ICommand KeyDownCommand { get; set; }
        public ICommand FLTextKeyboardFocusCommand { get; set; }
        public ICommand FLTextBoxLostFocusCommand { get; set; }
        public ICommand FireProofingPreviewTextCommand { get; set; }
        public ICommand FireProofingLostFocusCommand { get; set; }
        public ICommand BetweenPreviewTextCommand { get; set; }
        public ICommand BetweenLostFocusCommand { get; set; }
        public ICommand FLPreviewTextCommand { get; set; }
        #endregion

        #region View
        public string SelectObjName
        {
            get { return _SelectObjName; }
            set
            {
                _SelectObjName = value;
                popset.ChangesReflect();
                RaisePropertyChanged("SelectObjName");
            }
        }

        public string TargetObjName
        {
            get { return _TargetObjName; }
            set
            {
                _TargetObjName = value;
                TopObjAddColorName = _TargetObjName;
                RaisePropertyChanged("TargetObjName");
            }
        }
        public string SelectObjName_MoveMethod
        {
            get { return _SelectObjName_MoveMethod; }
            set
            {
                _SelectObjName_MoveMethod = value;
                RaisePropertyChanged("SelectObjName_MoveMethod");
            }
        }
        public string Location_TopObj
        {
            get { return _Location_TopObj; }
            set
            {
                var controlstatus = ControlStatus.Instance;
                _Location_TopObj = value;
                RaisePropertyChanged("Location_TopObj");
            }
        }

        public string Location_BottomObj
        {
            get { return _Location_BottomObj; }
            set
            {
                var controlstatus = ControlStatus.Instance;
                _Location_BottomObj = value;
                RaisePropertyChanged("Location_BottomObj");
            }
        }
        public string SelectObjName_Move
        {
            get { return _SelectObjName_Move; }
            set
            {
                _SelectObjName_Move = value;
                RaisePropertyChanged("SelectObjName_Move");
            }
        }

        public string MoveOptionShow
        {
            get { return _MoveOptionShow; }
            set
            {
                _MoveOptionShow = value;
                RaisePropertyChanged("MoveOptionShow");
            }
        }


        public string OptionFontWeight
        {
            get { return _OptionFontWeight; }
            set
            {
                _OptionFontWeight = value;
                RaisePropertyChanged("OptionFontWeight");
            }
        }

        public bool FiftyButton
        {
            get { return _FiftyButton; }
            set { _FiftyButton = value; RaisePropertyChanged("FiftyButton"); }
        }

        public bool OneHandredButton
        {
            get { return _OneHandredButton; }
            set { _OneHandredButton = value; RaisePropertyChanged("OneHandredButton"); }
        }

        public bool NothingButton
        {
            get { return _NothingButton; }
            set { _NothingButton = value; RaisePropertyChanged("NothingButton"); }
        }

        public bool FireProofingValidButton
        {
            get { return _FireProofingValidButton; }
            set
            {
                _FireProofingValidButton = value;
                popset.ChangesReflect();
                RaisePropertyChanged("FireProofingValidButton");
            }
        }
        public bool FireProofingInValidButton
        {
            get { return _FireProofingInValidButton; }
            set
            {
                _FireProofingInValidButton = value;
                popset.ChangesReflect();
                RaisePropertyChanged("FireProofingInValidButton");
            }
        }

        public string SetDisplayFireProofing
        {
            get { return _SetDisplayFireProofing; }
            set
            {

                _SetDisplayFireProofing = value;
                RaisePropertyChanged("SetDisplayFireProofing");
            }
        }


        public string BetweenObjValue
        {
            get { return _BetweenObjValue; }
            set
            {

                string str;
                double rc;
                bool acceptflag = true;
                ControlStatus controlstatus = ControlStatus.Instance;
                WindowControl windowcontrol = new WindowControl();
                RoundNum roundnum = new RoundNum();

                acceptflag = windowcontrol.TextBoxSetterControl(value, _BetweenObjValue);

                // Reject if more than one minus sign sneaks past IME filters.
                if (value.Length - value.Replace("-", "").Length > 0)
                {
                    acceptflag &= false;
                }

                if (acceptflag == true)
                {
                    str = Regex.Replace(value, @"[^0-9|.]", "");

                    if (double.TryParse(str, out rc))
                    {
                        if ((1000000 > rc) && (-1000000 < rc))
                        {
                            _BetweenObjValue = value;
                            RaisePropertyChanged("BetweenObjValue");

                        }
                    }

                    else
                    {
                        _BetweenObjValue = str;
                    }
                }
            }
        }

        public string FireProofingValue
        {
            get
            {
                return _FireProofingValue;
            }
            set
            {

                string str;
                double rc;
                bool acceptflag = true;
                ControlStatus controlstatus = ControlStatus.Instance;
                WindowControl windowcontrol = new WindowControl();
                RoundNum roundnum = new RoundNum();

                acceptflag = windowcontrol.TextBoxSetterControl(value, _FireProofingValue);

                // Reject if more than one minus sign sneaks past IME filters.
                if (value.Length - value.Replace("-", "").Length > 0)
                {
                    acceptflag &= false;
                }

                if (acceptflag == true)
                {
                    str = Regex.Replace(value, @"[^0-9|.]", "");

                    if (double.TryParse(str, out rc))
                    {
                        if ((1000000 > rc) && (-1000000 < rc))
                        {

                            _FireProofingValue = value;
                            RaisePropertyChanged("FireProofingValue");

                        }
                    }

                    else
                    {
                        _FireProofingValue = str;
                    }
                }
            }
        }

        public string TopObjAddColorName
        {
            get { return _TopObjAddColorName; }
            set
            {
                _TopObjAddColorName = value;
                RaisePropertyChanged("TopObjAddColorName");
            }
        }

        public string BottomObjAddColorName
        {
            get { return _BottomObjAddColorName; }
            set
            {
                _BottomObjAddColorName = value;
                RaisePropertyChanged("BottomObjAddColorName");
            }
        }

        public bool OffsetRadioButton
        {
            get { return _OffsetRadioButton; }
            set { _OffsetRadioButton = value; RaisePropertyChanged("OffsetRadioButton"); }
        }

        public bool UnityRadioButton
        {
            get { return _UnityRadioButton; }
            set { _UnityRadioButton = value; RaisePropertyChanged("UnityRadioButton"); }
        }

        public bool FortyFiveElbowRadioButton
        {
            get { return _FortyFiveElbowRadioButton; }
            set { _FortyFiveElbowRadioButton = value; RaisePropertyChanged("FortyFiveElbowRadioButton"); }
        }

        public bool NinetyElbowRadioButton
        {
            get { return _NinetyElbowRadioButton; }
            set { _NinetyElbowRadioButton = value; RaisePropertyChanged("NinetyElbowRadioButton"); }
        }

        public bool ScarveElbowRadioButton
        {
            get { return _ScarveElbowRadioButton; }
            set { _ScarveElbowRadioButton = value; RaisePropertyChanged("ScarveElbowRadioButton"); }
        }
        public bool MoveOptionExpandedFlag
        {
            get { return _MoveOptionExpandedFlag; }
            set
            {

                _MoveOptionExpandedFlag = value;
                popset.ChangesReflect();
                RaisePropertyChanged("MoveOptionExpandedFlag");
            }
        }

        public string DuctOffsetLevel
        {
            get
            {
                RoundNum roundnum = new RoundNum();
                var controlstatus = ControlStatus.Instance;
                PropertyGet propset = new PropertyGet();
                WindowControl windowcontrol = new WindowControl();
                _InternalDuctOffsetLevel = roundnum.RoundUnnecessaryNum(roundnum.ApointDecimalRound(controlstatus.RevitProjectDecimalAccuracy,
                                                                       propset.DuctOffsetLevelChangeCheck(this, mep)));
                if ((controlstatus.CallRoute != DuctDisplacementDefine.TextChangeRoute.RefalenceLine) &&
                    (SetDisplay_TOBJ == "Visible"))
                {
                    IsolationValue = propset.SetIsolationValue(this, mep);
                }

                if (controlstatus.CallRoute != DuctDisplacementDefine.TextChangeRoute.Text)
                {
                    _DuctOffsetLevel = _InternalDuctOffsetLevel.ToString();
                }

                return _DuctOffsetLevel;
            }
            set
            {

                string str;
                double rc;
                bool acceptflag = true;
                ControlStatus controlstatus = ControlStatus.Instance;
                WindowControl windowcontrol = new WindowControl();
                RoundNum roundnum = new RoundNum();


                acceptflag = windowcontrol.TextBoxSetterControl(value, _DuctOffsetLevel);

                // Reject misplaced minus signs during direct text edits.
                if ((controlstatus.CallRoute == DuctDisplacementDefine.TextChangeRoute.Text) &&
                    (value.Length - value.Replace("-", "").Length > 1) ||
                    ((value.LastIndexOf("-") != -1) &&
                    (value.LastIndexOf("-") != 0)))
                {
                    acceptflag &= false;
                }

                if (acceptflag == true)
                {
                    str = Regex.Replace(value, @"[^0-9|.|-]", "");

                    if (double.TryParse(str, out rc))
                    {
                        if ((1000000 > rc) && (-1000000 < rc))
                        {
                            _DuctOffsetLevel = str;
                            _InternalDuctOffsetLevel = rc;
                            RaisePropertyChanged("DuctOffsetLevel");
                        }
                    }
                    else
                    {
                        _DuctOffsetLevel = str;
                    }


                }
            }
        }

        public double InternalDuctOffsetLevel
        {
            get
            {
                return _InternalDuctOffsetLevel;
            }
        }

        public string LevelButtonName
        {
            get { return _LevelButtonName; }
            set
            {
                _LevelButtonName = value;
                popset.ChangesReflect();
                RaisePropertyChanged("LevelButtonName");
            }
        }

        public string DuctReferenceLine
        {
            get { return _DuctReferenceLine; }
            set
            {
                var controlstatus = ControlStatus.Instance;

                if (_DuctReferenceLine != string.Empty)
                {
                    controlstatus.ReferenceLine_Befor = _DuctReferenceLine;
                }

                _DuctReferenceLine = value;

                if ((_DuctReferenceLine != controlstatus.ReferenceLine_Befor) &&
                    (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide)) ||
                    (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_Center)) ||
                    (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide)))
                {
                    controlstatus.CallRoute = DuctDisplacementDefine.TextChangeRoute.RefalenceLine;
                }
                popset.ChangesReflect();
                RaisePropertyChanged("DuctReferenceLine");
            }
        }

        public bool BaseLineControlFlag
        {
            get { return _BaseLineControlFlag; }
            set { _BaseLineControlFlag = value; RaisePropertyChanged("BaseLineControlFlag"); }
        }

        public double SelctObj_InitalFL
        {
            get { return _SelctObj_InitalFL; }
            set
            {
                RoundNum roundnum = new RoundNum();
                _SelctObj_InitalFL = roundnum.RoundUnnecessaryNum(value);
                RaisePropertyChanged("SelctObj_InitalFL");
            }
        }


        public double IsolationValue
        {
            get
            {
                return _IsolationValue;
            }
            set
            {
                RoundNum roundnum = new RoundNum();
                _IsolationValue = roundnum.RoundUnnecessaryNum(value);
                RaisePropertyChanged("IsolationValue");
                popset.ChangesReflect();
            }
        }

        public string IsolationValueColor
        {
            get { return _IsolationValueColor; }
            set
            { _IsolationValueColor = value; RaisePropertyChanged("IsolationValueColor"); }
        }

        public string IsolationWorningMessage
        {
            get { return _IsolationWorningMessage; }
            set
            { _IsolationWorningMessage = value; RaisePropertyChanged("IsolationWorningMessage"); }
        }

        public string TopArrangementColor
        {
            get { return _TopArrangementColor; }
            set
            {
                _TopArrangementColor = value;
                popset.ChangesReflect();
                RaisePropertyChanged("TopArrangementColor");
            }
        }
        public string BottomArrangementColor
        {
            get { return _BottomArrangementColor; }
            set
            {
                _BottomArrangementColor = value;
                popset.ChangesReflect();
                RaisePropertyChanged("BottomArrangementColor");
            }
        }


        public string Location_BottomObjColor
        {
            get { return _Location_BottomObjColor; }
            set { _Location_BottomObjColor = value; RaisePropertyChanged("Location_BottomObjColor"); }
        }

        public string Location_TopObjColor
        {
            get { return _Location_TopObjColor; }
            set { _Location_TopObjColor = value; RaisePropertyChanged("Location_TopObjColor"); }
        }

        public string SetDisplay_TOBJ
        {
            get { return _SetDisplay_TOBJ; }
            set { _SetDisplay_TOBJ = value; RaisePropertyChanged("SetDisplay_TOBJ"); }
        }
        #endregion
        #endregion


        #region Member Functions

        #region On dialog open

        /// <summary>Initial binding pass before showing the obstruction dialog.</summary>
        public void StartWork()
        {
            log.Info("");
            this.TargetObjName = string.Empty;

            var controlstatus = ControlStatus.Instance;
            var windowpropertys = WindowReceiveProperty.Instance;
            WindowControl windowcontrol = new WindowControl();
            RoundNum roundnum = new RoundNum();

            this.SelectObjName = mep.GetFamilyName(DuctDisplacementDefine.InstructionObj.MoveObj_1);

            this.DuctReferenceLine = windowcontrol.GetInitRefalenceLine(mep, DuctDisplacementDefine.InstructionObj.MoveObj_1);
            this.OffsetRadioButton = windowpropertys.OffsetRadioButton;
            this.UnityRadioButton = windowpropertys.UnityRadioButton;
            this.FortyFiveElbowRadioButton = windowpropertys.FortyFiveElbowRadioButton;
            this.NinetyElbowRadioButton = windowpropertys.NinetyElbowRadioButton;
            this.ScarveElbowRadioButton = windowpropertys.ScarveElbowRadioButton;
            this.LevelButtonName = ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel);
            this.MoveOptionExpandedFlag = false;


            string str = windowcontrol.GetInitRefalenceLine(mep, DuctDisplacementDefine.InstructionObj.MoveObj_1);
            double height = windowcontrol.AddHeight(str, mep);
            this.SelctObj_InitalFL = windowcontrol.GetInitSelectOBJRefCenterValue(mep) + height;

            this.DuctOffsetLevel = windowcontrol.GetInitSelectOBJRefCenterValue(mep).ToString();

            this.Location_BottomObjColor = "Red";
            this.Location_TopObjColor = "Blue";

            // Optional inputs (rounding preset, fire coating, minimum spacing)
            this.FiftyButton = windowpropertys.FiftyButton;
            this.OneHandredButton = windowpropertys.OneHandredButton;
            this.NothingButton = windowpropertys.NothingButton;
            this.FireProofingInValidButton = windowpropertys.InValidButton;
            this.FireProofingValidButton = windowpropertys.ValidButton;
            this.BetweenObjValue = windowpropertys.BetweenObjValue.ToString();
            this.FireProofingValue = windowpropertys.FireProofingValue.ToString();
        }
        #endregion

        #region Before closing or hiding

        /// <summary>Pushes dialog results into <see cref="WindowReceiveProperty"/> and closes the Revit-modeless window.</summary>
        /// <param name="num">Route that triggered the close action.</param>
        public void EndWork(DuctDisplacementDefine.WindowReturnNum num)
        {
            log.Trace("WindowReturnNum:" + num);
            if (num == DuctDisplacementDefine.WindowReturnNum.GraphicInstructions_General ||
                num == DuctDisplacementDefine.WindowReturnNum.GraphicInstructions_Linkd ||
                num == DuctDisplacementDefine.WindowReturnNum.OK ||
                num == DuctDisplacementDefine.WindowReturnNum.EndRoutine)
            {
                var windowpropertys = WindowReceiveProperty.Instance;
                WindowControl windowcontrol = new WindowControl();

                double fl;
                double xx;

                windowpropertys.OffsetRadioButton = this.OffsetRadioButton;
                windowpropertys.UnityRadioButton = this.UnityRadioButton;
                windowpropertys.FortyFiveElbowRadioButton = this.FortyFiveElbowRadioButton;
                windowpropertys.NinetyElbowRadioButton = this.NinetyElbowRadioButton;
                windowpropertys.ScarveElbowRadioButton = this.ScarveElbowRadioButton;

                windowpropertys.FiftyButton = this.FiftyButton;
                windowpropertys.OneHandredButton = this.OneHandredButton;
                windowpropertys.NothingButton = this.NothingButton;

                double.TryParse(this.BetweenObjValue, out xx);
                windowpropertys.BetweenObjValue = xx;
                windowpropertys.InValidButton = this.FireProofingInValidButton;
                windowpropertys.ValidButton = this.FireProofingValidButton;
                double.TryParse(this.FireProofingValue, out xx);
                windowpropertys.FireProofingValue = xx;
                windowpropertys.DuctReferenceLine = this.DuctReferenceLine;

                if (num == DuctDisplacementDefine.WindowReturnNum.GraphicInstructions_General ||
                    num == DuctDisplacementDefine.WindowReturnNum.GraphicInstructions_Linkd ||
                    num == DuctDisplacementDefine.WindowReturnNum.OK)
                {
                    if (this.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel))
                    {
                        fl = this.InternalDuctOffsetLevel;

                    }

                    else if (this.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement))
                    {
                        fl = (double)((decimal)this.InternalDuctOffsetLevel + ((decimal)windowcontrol.GetInitSelectOBJRefCenterValue(mep) + (decimal)windowcontrol.AddHeight(this.DuctReferenceLine, mep)));
                    }

                    else
                    {
                        //error
                        log.Error("LevelButtonName:" + LevelButtonName);
                        fl = this.InternalDuctOffsetLevel;
                    }

                    windowpropertys.FlValue =
                        windowcontrol.AmountOfMovementABSCal(mep, this.DuctReferenceLine, fl, windowcontrol.GetInitSelectOBJRefCenterValue(mep));
                }
            }

            CloseDuctPropertySetWindow(num);
        }
        #endregion

        #region Close transition

        /// <summary>Applies the routed close code and hides the WPF shell.</summary>
        private void CloseDuctPropertySetWindow(DuctDisplacementDefine.WindowReturnNum num)
        {
            log.Info("WindowReturnNum:" + num);
            var controlstatus = ControlStatus.Instance;
            controlstatus.ButtonType = num;
            uiwindow.CloseWindow();
        }
        #endregion

        #region RaisePropertyChanged forwarding

        /// <summary>Delegates INotifyPropertyChanged for external UI helpers.</summary>
        public void OutsideRaisePropertyChanged(string str)
        {
            RaisePropertyChanged(str);
        }
        #endregion
        #endregion
    }
}
