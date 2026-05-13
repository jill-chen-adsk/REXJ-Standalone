using CmdDuctDisplacement.UI.Common;
using System;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Model.InheritBase
{
    class TextBoxLostFocusBase : ICommand
    {
        //メンバ変数
        #region Memeber Variables
        #endregion


        //コンストラクタ
        #region Constructor
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
        { return true; }

        /// <summary>
        /// TextBoxのロストフォーカス時 
        /// 必ずオーバーライドして使うこと
        /// entityに実体を代入する
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Execute(object parameter)
        {
            string textbox = "";
            string entity;
            double rc = 0;
            ControlStatus controlstatus = ControlStatus.Instance;
            RoundNum roundnum = new RoundNum();

            //テキストボックス内に文字列がセットされていれば、文字列を排除する
            string str = Regex.Replace(textbox, @"[^0-9|.|-]", "");

            if (!(double.TryParse(str, out rc)))
            {
                //doubleに変換できない時
                //フェールセーフ
                entity = 0.ToString();
            }
            else
            {
                //doubleに変換できるとき、有効少数桁をセットする
                rc = roundnum.ApointDecimalRound(controlstatus.RevitProjectDecimalAccuracy, rc);
                entity = rc.ToString();
            }
        }
        #endregion                                                                                                                                                                                                                                                                                                                                               
    }
}
