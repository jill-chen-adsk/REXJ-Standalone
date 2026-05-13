using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// 回避対象物選択ボタン押下イベント関連クラス
    /// </summary>
    class MoveDistanceCalCommand : ICommand
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        #endregion

        //コンストラクタ
        #region Constructor
        public MoveDistanceCalCommand(ConnectionSettingViewModel ductPropaty, Logger _log)
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

        //コマンドが実行された時の処理
        public void Execute(object parameter)
        {
            log.Trace("ButtonPush Class:" + this.GetType().Name);
            var controlstatus = ControlStatus.Instance;
            controlstatus.CallRoute = DuctDisplacementDefine.TextChangeRoute.MoveDistanceCalButton;
            ductp.EndWork(DuctDisplacementDefine.WindowReturnNum.GraphicInstructions_General);
        }
        #endregion
    }
}
