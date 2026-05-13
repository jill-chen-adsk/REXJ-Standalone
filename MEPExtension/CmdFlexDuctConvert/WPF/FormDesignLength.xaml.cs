using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Interop;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace CmdFlexDuctConvert
{
    /// <summary>
    /// </summary>
    public partial class FormDesignLength : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private Autodesk.Revit.UI.UIApplication uiApp = null;

        #region "最大化・最小化・閉じるボタンの非表示設定"
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(handle, GWL_STYLE);
            style = style & (~WS_SYSMENU);
            SetWindowLong(handle, GWL_STYLE, style);
        }
        #endregion

        public FormDesignLength(Autodesk.Revit.UI.UIApplication uiapp)
        {
            InitializeComponent();

            uiApp = uiapp;
            Autodesk.Revit.UI.UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            // Enter キーで Revit ウィンドウをアクティブにする
            this.KeyDown += (sender, e) =>
            {
                if (e.Key != Key.Enter) { return; }
                var process = Process.GetCurrentProcess();
                var handle = process.MainWindowHandle;
                SetForegroundWindow(handle);
            };

            this.txtDesignLength.Text = "1000";
            this.DataContext = new DataSourceInputDesignLength(txtDesignLength.Text);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        /// <summary>
        /// メインウィンドウにESCキーを送信する
        /// </summary>
        public static void sendEscKey()
        {
            var process = Process.GetCurrentProcess();
            var handle = process.MainWindowHandle;
            PostMessage(handle, 0x0100, 0x1B, 0);
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            sendEscKey();
        }

        protected virtual void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            sendEscKey();
            e.Cancel = true;
        }

        private void txtBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
            int n;
            if (int.TryParse(textBox.Text, out n))
            {
                if (n > 0)
                {
                    return;
                }
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                var textBox = sender as TextBox;
                string text = Clipboard.GetText();
                e.Handled = !new Regex("[0-9]").IsMatch(text);
                int n;
                if (!e.Handled && int.TryParse(text, out n))
                {
                    if (n > 0)
                    {
                        textBox.Paste();
                        return;
                    }
                }
            }
            e.Handled = true;
        }

        private void txtBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = e.Source as TextBox;
            if (textBox != null)
            {
                textBox.Background = System.Windows.Media.Brushes.LightPink;
            }
        }

        private void txtBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = e.Source as TextBox;
            if (textBox != null)
            {
                textBox.Background = System.Windows.Media.Brushes.White;
            }
        }

        public int designLength
        {
            get
            {
                if (this.txtDesignLength.Text == string.Empty)
                {
                    return 0;
                }

                try
                {
                    return int.Parse(this.txtDesignLength.Text);
                }
                catch (FormatException /*e*/)
                {
                    return 0;
                }
            }
        }
    }

    class DataSourceInputDesignLength : IDataErrorInfo
    {
        public string DesignLength { get; set; }
        public string Error { get { return null; } }

        public DataSourceInputDesignLength(string designLength)
        {
            DesignLength = designLength;
        }

        public string this[string propertyName]
        {
            get
            {
                string result = null;

                //string text = string.Empty;
                //switch (propertyName)
                //{
                //    case "DesignLength":
                //        text = DesignLength;
                //        if (text == null)
                //        {
                //            return null;
                //        }

                //        int n;
                //        try
                //        {
                //            n = int.Parse(text);
                //        }
                //        catch (Exception /*e*/)
                //        {
                //            result = "整数値を入力してください。";
                //            break;
                //        }
                //        if (n < 1)
                //        {
                //            result = "1以上の整数値を入力してください。";
                //        }
                //        break;
                //}
                return result;
            }
        }
    }
}
