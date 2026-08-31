using System.Windows;
using System.Windows.Controls;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils
{
    public partial class WeaveConfirmDialog : Window, IWeaveChromeWindow
    {
        public WeaveConfirmDialog(string message, string title, string okText, string cancelText)
        {
            InitializeComponent();
            MessageText.Text = message ?? string.Empty;
            btnOK.Content = okText ?? "OK";
            btnCancel.Content = cancelText ?? "Cancel";
            WeaveTheme.Apply(this, this, title, CancelDialog);
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

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
