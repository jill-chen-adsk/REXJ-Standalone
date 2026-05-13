using CmdDuctDisplacement.UI.Model;
using System.Windows.Controls;
using System.Windows.Input;
#if NETCORE
using Microsoft.Xaml.Behaviors;
#else
using Microsoft.Xaml.Behaviors;
#endif


namespace CmdDuctDisplacement.UI.Utility.CodeBehind
{
    /// <summary>
    /// テキストボックスイベント関連のパラメータを捕捉する
    /// </summary>
    class FLTextBoxKeyDownDispatcher : Behavior<TextBox>
    {
        // メンバ関数
        #region Member Functions

        /// <summary>
        /// イベントの登録
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewTextInput += TextBox_PreviewTextInput;
            this.AssociatedObject.PreviewKeyDown += TextBox_PreviewKeyDown;
        }

        /// <summary>
        /// イベントの解除
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewTextInput -= TextBox_PreviewTextInput;
            this.AssociatedObject.PreviewKeyDown -= TextBox_PreviewKeyDown;
        }

        /// <summary>
        /// テキストボックスにフォーカスが当たっているときにコントロールキー以外が押下された時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var ins = ControlStatus.Instance;
            ins.TextCompositionEvent = e;
        }

        /// <summary>
        /// テキストボックス上の押下キーを捕捉する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Insert) ||
                (e.Key == Key.Space))
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}
