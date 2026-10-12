using System.Windows;
using System.Windows.Controls;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    public partial class WeaveMessageDialog : Window, IWeaveChromeWindow
    {
        /// <param name="cancelText">Text for the second button; null shows a single OK button.</param>
        public WeaveMessageDialog(string message, string title, string okText, string cancelText = null)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");
            MessageText.Text = message ?? string.Empty;
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
        public Button ChromeCloseButton => chromeCloseButton;

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelDialog();
        }

        private void CancelDialog()
        {
            DialogResult = false;
            Close();
        }
    }
}
