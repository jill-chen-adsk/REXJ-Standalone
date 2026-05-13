using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System;
using System.Windows;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// 終了ルーチン移行イベント関連クラス
    /// </summary>
    class EndRoutineCommand : ICommand
    {

        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        #endregion

        //コンストラクタ
        #region Constructor
        public EndRoutineCommand(ConnectionSettingViewModel ductPropaty, Logger _log)
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
        /// Escキーが押下された時または、コマンド終了ボタンが押下されたとき
        /// 終了ルーチンへ移行する
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            log.Info("ButtonPush Class:" + this.GetType().Name);
            ductp.EndWork(DuctDisplacementDefine.WindowReturnNum.EndRoutine);
        }
        #endregion
    }
}
