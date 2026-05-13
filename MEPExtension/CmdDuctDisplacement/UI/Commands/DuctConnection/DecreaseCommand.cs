using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.UI.Common;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// ▼ボタン押下イベント関連クラス
    /// </summary>
    class DecreaseCommand : ICommand
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        #endregion

        //コンストラクタ
        #region Constructor
        public DecreaseCommand(ConnectionSettingViewModel ductPropaty, Logger _log)
        {
            ductp = ductPropaty;
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
        public bool CanExecute(object parameter) { return true; }

        /// <summary>
        /// ▼押下でFLの値を減算する
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            log.Trace("ButtonPush Class:" + this.GetType().Name);
            CalCulation cal = new CalCulation();
            var controlstatus = ControlStatus.Instance;
            controlstatus.CallRoute = DuctDisplacementDefine.TextChangeRoute.DecreaseButton;
            ductp.DuctOffsetLevel = cal.AddValueToOriginarValue(ductp, DuctDisplacementDefine.MethodOfCalculation.Sub).ToString();
        }
        #endregion
    }
}
