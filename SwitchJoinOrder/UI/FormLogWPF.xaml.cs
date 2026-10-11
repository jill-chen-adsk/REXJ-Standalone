using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfGrid = System.Windows.Controls.Grid;
using System.Windows.Documents;
using ADSK.JExtRAC.SwitchJoinOrder.Utils;
using Microsoft.Win32;
using RvtExtApp = ADSK.JExtRAC.SwitchJoinOrder;

namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    public partial class FormLogWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;

        public FormLogWPF(RvtExtApp.Components.Attribute cmpAttribute, StringBuilder strLog)
        {
            _cmpAttribute = cmpAttribute;

            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            SetText();
            rtbLog.Document.Blocks.Clear();
            rtbLog.Document.Blocks.Add(new Paragraph(new Run(strLog?.ToString() ?? string.Empty)));

            string title = _cmpAttribute.ResourceText("IDS_TXT_LOG")
                + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            WeaveTheme.Apply(this, this, title, Close);
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public WpfGrid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void SetText()
        {
            btnSave.Content = _cmpAttribute.ResourceText("IDS_TXT_SAVELOG");
            btnClose.Content = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Export log",
                DefaultExt = "txt",
                Filter = "Text Documents (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog(this) != true)
                return;

            try
            {
                var range = new TextRange(rtbLog.Document.ContentStart, rtbLog.Document.ContentEnd);
                File.WriteAllText(dialog.FileName, range.Text, Encoding.Unicode);
                Close();
            }
            catch (Exception ex)
            {
                WeaveDialogHost.ShowMessage(
                    new System.Windows.Interop.WindowInteropHelper(this).Owner,
                    ex.ToString(),
                    _cmpAttribute.ResourceText("IDS_TXT_ERROR"),
                    _cmpAttribute.ResourceText("IDS_TXT_OK"));
            }
        }
    }
}
