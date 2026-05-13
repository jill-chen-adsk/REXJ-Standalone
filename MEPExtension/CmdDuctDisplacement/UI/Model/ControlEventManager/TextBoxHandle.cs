using CmdDuctDisplacement.UI.View;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Model.ControlEventManager
{
    /// <summary>
    /// テキストボックスのイベント制御関連
    /// </summary>
    class TextBoxHandle
    {
        //メンバ変数
        #region Memeber Variables
        #endregion

        //コンストラクタ
        #region Constructor
        public TextBoxHandle(ConnectionSettingView connectionsettingview)
        {
            connectionsettingview.FLTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, ExecutePaste));
            connectionsettingview.FireTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, ExecutePaste));
            connectionsettingview.BetweenTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, ExecutePaste));
        }
        #endregion

        // メンバ関数
        #region Member Functions
        /// <summary>
        /// ペーストを無視する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">イベントハンドラ</param>
        private void ExecutePaste(object sender, ExecutedRoutedEventArgs e)
        {
            // ※数値だけ受け取りたい場合は以下を実装する
            
            
            
            //TextBox textbox = (TextBox)sender;
            //string text = Clipboard.GetText();

            //int result = 0;
            //if (int.TryParse(text, out result))
            //{
            //    textbox.Paste();
            //}
            //else
            //{
            //}

        }
        #endregion
    }
}
