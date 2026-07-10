using System.Windows;
using System.Windows.Controls;

namespace ADSK.JExtRAC.AreaSchedule.Utils
{
    public partial class WeaveMessageDialog : Window, IWeaveChromeWindow
    {
        public WeaveMessageDialog(string message, string title, string okText)
        {
            InitializeComponent();
            MessageText.Text = message ?? string.Empty;
            btnOK.Content = okText ?? "OK";
            WeaveTheme.Apply(this, this, title, () =>
            {
                DialogResult = true;
                Close();
            });
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
    }
}
