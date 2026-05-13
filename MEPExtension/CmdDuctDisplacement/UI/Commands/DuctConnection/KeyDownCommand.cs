using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// キーボード押下イベント関連クラス
    /// </summary>
    class KeyDownCommand : ICommand
    {

        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        #endregion

        //コンストラクタ
        #region Constructor
        public KeyDownCommand(ConnectionSettingViewModel ductPropaty, Logger _log)
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
        /// キーが押下時のイベント
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            log.Trace("Event Class:" + this.GetType().Name);
            var controlstatus = ControlStatus.Instance;

            //画面にフォーカスが当たっている時、Escキー押下で終了ルーチンに移行

            if (controlstatus.LastKey == Key.Escape)
            {
                ductp.EndWork(DuctDisplacementDefine.WindowReturnNum.EndRoutine);
            }
        }
        #endregion
    }
}

