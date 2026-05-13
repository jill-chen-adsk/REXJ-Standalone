using CmdDuctDisplacement.UI.Common;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.Model.InheritBase;
using CmdDuctDisplacement.UI.View;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System.Text.RegularExpressions;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// FMテキストボックスLostFocusイベント関連クラス
    /// </summary>
    class FLTextBoxLostFocusCommand : TextBoxLostFocusBase
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        private System.Windows.Controls.TextBox textbox;
        #endregion


        //コンストラクタ
        #region Constructor
        public FLTextBoxLostFocusCommand(ConnectionSettingViewModel ductPropaty, Logger _log, ConnectionSettingView _uiwindow)
        {
            ductp = ductPropaty;
            log = _log;
            textbox =_uiwindow.FLTextBox;
        }
        #endregion

        // メンバ関数
        #region Member Functions
        /// <summary>
        /// TextBoxのロストフォーカス時 
        /// 必ずオーバーライドして使うこと
        /// entityに実体を代入する
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            double rc = 0;
            ControlStatus controlstatus = ControlStatus.Instance;
            RoundNum roundnum = new RoundNum();

            //テキストボックス内に文字列がセットされていれば、文字列を排除する
            string str = Regex.Replace(textbox.Text, @"[^0-9|.|-]", "");

            if (!(double.TryParse(str, out rc)))
            {
                //doubleに変換できない時
                //フェールセーフ
                ductp.DuctOffsetLevel = 0.ToString();
            }
            else
            {
                //doubleに変換できるとき、有効少数桁をセットする
                rc = roundnum.ApointDecimalRound(controlstatus.RevitProjectDecimalAccuracy, rc);
                ductp.DuctOffsetLevel = rc.ToString();
            }
        }
        #endregion                                                                                                                                                                                                                                                                                                                                               
    }
}
