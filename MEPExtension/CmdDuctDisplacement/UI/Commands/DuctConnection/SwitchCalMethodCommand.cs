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

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// FMボタン押下イベント関連クラス
    /// </summary>
    class SwitchCalMethodCommand : ICommand
    {

        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductConnection;
        private MEPOperation mep;
        #endregion

        //コンストラクタ
        #region Constructor
        public SwitchCalMethodCommand(ConnectionSettingViewModel ductPropaty, Logger _log, MEPOperation _mep)
        {
            ductConnection = ductPropaty;
            log = _log;
            mep = _mep;
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
        public bool CanExecute(object parameter) { return true; }

        /// <summary>
        /// FLと移動量切り替え
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            log.Trace("ButtonPush Class:" + this.GetType().Name);
            var windowprop = WindowReceiveProperty.Instance;

            WindowControl windowcontrol = new WindowControl();
            RoundNum roundnum = new RoundNum();

            var controlstatus = ControlStatus.Instance;
            controlstatus.CallRoute = DuctDisplacementDefine.TextChangeRoute.SwitchCalMethodButton;

            double addheight;

            addheight = windowcontrol.AddHeight(ductConnection.DuctReferenceLine, mep);


            if (ductConnection.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel))
            {
                ductConnection.LevelButtonName = ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement);
                //FLの表示値を移動量に変換する
                ductConnection.DuctOffsetLevel = roundnum.RoundUnnecessaryNum((double)((decimal)ductConnection.InternalDuctOffsetLevel - ((decimal)windowcontrol.GetInitSelectOBJRefCenterValue(mep) + (decimal)addheight))).ToString();
            }
            else if (ductConnection.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement))
            {
                ductConnection.LevelButtonName = ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel);
                //移動量の表示値をFLに変換する
                ductConnection.DuctOffsetLevel = roundnum.RoundUnnecessaryNum((double)((decimal)ductConnection.InternalDuctOffsetLevel + ((decimal)windowcontrol.GetInitSelectOBJRefCenterValue(mep) + (decimal)addheight))).ToString();
            }

            else
            {
                //erroe
                log.Error("check SwitchCalMethodCommand");
            }

        }
        #endregion
    }
}
