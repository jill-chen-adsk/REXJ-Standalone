using CmdDuctDisplacement.UI.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

#if NETCORE
using Microsoft.Xaml.Behaviors;
#else
using Microsoft.Xaml.Behaviors;
#endif


namespace CmdDuctDisplacement.UI.Utility.CodeBehind
{
    /// <summary>
    /// 画面上でのキー入力を捕捉する
    /// </summary>
    class ConnectionWindowDispatcher : Behavior<Window>
    {
        // メンバ関数
        #region Member Functions

        /// <summary>
        /// イベントの登録
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            this.AssociatedObject.Closing += AssociatedObject_XmarkPush;
        }

        /// <summary>
        /// イベントの解除
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            this.AssociatedObject.Closing -= AssociatedObject_XmarkPush;
        }

        /// <summary>
        /// 押下されたキーを捕捉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            var ins = ControlStatus.Instance;
            ins.LastKey = e.Key;
        }

        /// <summary>
        /// 画面の×ボタンを無効化する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_XmarkPush(object sender, CancelEventArgs e)
        {
            var ins = ControlStatus.Instance;
            e.Cancel = true;
        }
        

        #endregion
    }
}