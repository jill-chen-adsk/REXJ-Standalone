using System.Windows;
using System.Windows.Controls;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
    public partial class WeaveMessageDialog : Window, IWeaveChromeWindow
    {
        /// <param name="cancelText">Text for the second button; null shows a single OK button.</param>
        public WeaveMessageDialog(string message, string title, string okText, string cancelText = null)
        {
            InitializeComponent();
            messageText.Text = message ?? string.Empty;
            btnOK.Content = okText ?? "OK";

            if (cancelText == null)
                btnCancel.Visibility = Visibility.Collapsed;
            else
                btnCancel.Content = cancelText;

            WeaveTheme.Apply(this, this, title, CancelDialog);
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public System.Windows.Controls.Button ChromeCloseButton => chromeCloseButton;

        void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelDialog();
        }

        void CancelDialog()
        {
            DialogResult = false;
            Close();
        }
    }
}
