using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Logic;
using CmdDuctDisplacement.Resource;
using CmdDuctDisplacement.UI.Common;
using CmdDuctDisplacement.UI.Controller;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.Model.Entity;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// ↑ボタン押下イベント関連クラス
    /// </summary>
    class TopArrangementCommand : ICommand
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        private MEPOperation mep;
        private bool CommandSwitch;
        #endregion

        //コンストラクタ
        #region Constructor
        public TopArrangementCommand(ConnectionSettingViewModel ductPropaty, Logger _log, MEPOperation _mep)
        {
            ductp = ductPropaty;
            CommandSwitch = false;
            mep = _mep;
            log = _log;
        }
        #endregion

        // メンバ関数
        #region Member Functions

#pragma warning disable 0067
        // 本クラスでは使用しない
        //コマンドの実行の可否が変化したときのイベント
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        //現在の状態でこのコマンドを実行できるかどうかを判断するメソッドを定義します。
        public bool CanExecute(object parameter)
        {
            if (ductp.TopArrangementColor == Brushes.LightSteelBlue.ToString() &&
                CommandSwitch == true)
            {
                return false;
            }

            CommandSwitch = true;
            return true;
        }

        /// <summary>
        /// 移動対象図形を回避対象図形の下に配置したときの想定FLを算出
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            WindowControl windowcontrol = new WindowControl();
            RoundNum roundnum = new RoundNum();
            WindowReceiveProperty windowsprop = WindowReceiveProperty.Instance;
            double hDiff = 0;
            double clearance = 0;
            double offset = 0;
            double FireProofingValue = 0;
            double xx;
            int offsetpos;

            offsetpos = windowcontrol.RefalenceLineConvert(ductp.DuctReferenceLine);

            ControlStatus controlstatus = ControlStatus.Instance;
            CalCulation calculation = new CalCulation();
            log.Trace("ButtonPush Class:" + this.GetType().Name);
            WindowControl cont = new WindowControl();
            controlstatus.CallRoute = DuctDisplacementDefine.TextChangeRoute.TopArrangementButton;
            cont.UpDownButtonSwapColor(ductp, DuctDisplacementDefine.PressButton.Top);
            cont.UpDownObjLabelSwapColor(ductp, DuctDisplacementDefine.PressButton.Top);

            //耐火被覆厚が有効か無効か判断する
            if (windowcontrol.FireProofingType(ductp))
            {
                double.TryParse(ductp.FireProofingValue, out xx);
                FireProofingValue = xx;
            }

            else
            {
                FireProofingValue = 0;
            }

            log.Trace("CalculateDiff param in offsetPos:" + offsetpos + "direction:" + DuctDisplacementDefine.DIR_UPPER +
                "roundUnit:" + windowsprop.RoundType() + "minClear:" + windowsprop.BetweenObjValue + "insulate:" + FireProofingValue);

            double.TryParse(ductp.BetweenObjValue, out xx);

            mep.CalculateDiff(out hDiff, out clearance, out offset,
                offsetpos, DuctDisplacementDefine.DIR_UPPER, cont.RoundType(ductp), xx, FireProofingValue);

            log.Trace("CalculateDiff param out hDiff:" + hDiff + "clearance:" + clearance + "offset:" + offset);

            ductp.DuctOffsetLevel = roundnum.RoundUnnecessaryNum(offset).ToString();
            ductp.IsolationValue = roundnum.RoundUnnecessaryNum(clearance);
            //値再算出時は、FLテキストボックスをFLにセット
            ductp.LevelButtonName = ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel);
        }
        #endregion
    }
}
