using ADSK.JExtRAC.AutoCreateDimension.Utils;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using RvtExtApp = ADSK.JExtRAC.AutoCreateDimension;

namespace ADSK.JExtRAC.AutoCreateDimension.UI
{
    public partial class FormLogWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly StringBuilder _strLog;

        public FormLogWPF(RvtExtApp.Components.Attribute cmpAttribute, StringBuilder strLog)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _cmpAttribute = cmpAttribute;
            _strLog = strLog;

            WeaveTheme.Apply(this, this, cmpAttribute.ResourceText("IDS_TXT_LOG"));
            SetText();
            SetData();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(this, this, _cmpAttribute.ResourceText("IDS_TXT_LOG"));
            btnClose.Content = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");
        }

        private void SetData()
        {
            RtxtLog.AppendText(_strLog.ToString());
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
