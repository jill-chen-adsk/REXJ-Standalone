using ADSK.JExtRAC.ValueCopy.Utils;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using Microsoft.Win32;
using RvtExtApp = ADSK.JExtRAC.ValueCopy;

namespace ADSK.JExtRAC.ValueCopy.UI
{
    /// ================================================================================
    /// <summary>FormLogWPF</summary>
    ///
    /// <history>2024/03/21 Created</history>
    /// ================================================================================
    public partial class FormLogWPF : Window, IWeaveChromeWindow
    {
        // Member variable

        #region Member Variables

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _cmpAttribute;

        /// <summary>Log</summary>
        private StringBuilder _strLog;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute">Parameter</param>
        /// <param name="strLog">Log text</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
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

        #endregion Constructor

        // Weave chrome parts

        #region Weave Chrome

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        #endregion Weave Chrome

        // Member function

        #region Member Functions

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void SetText()
        {
            WeaveWindowChrome.SetTitle(this, this, _cmpAttribute.ResourceText("IDS_TXT_LOG"));
            this.btnSave.Content = _cmpAttribute.ResourceText("IDS_TXT_SAVELOG");
            this.btnClose.Content = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");
        }

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void SetData()
        {
            RtxtLog.AppendText(_strLog.ToString());
        }

        /// ================================================================================
        /// <summary>Save dialog</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private bool SaveLog()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = _cmpAttribute.ResourceText("IDS_TXT_EXPORTLOG"),
                    DefaultExt = "txt",
                    Filter = _cmpAttribute.ResourceText("IDS_TXT_LOGFILEFILTER"),
                    FilterIndex = 1
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, RtxtLog.Document.ContentStart.GetTextInRun(LogicalDirection.Forward));
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                WeaveDialogHost.ShowMessage(
                    new WindowInteropHelper(this).Handle,
                    ex.Message,
                    _cmpAttribute.ResourceText("IDS_TXT_ERROR"),
                    _cmpAttribute.ResourceText("IDS_TXT_OK"));
                return false;
            }
        }

        #endregion Member Functions

        // Events

        #region Events

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSave control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveLog())
                this.Close();
        }

        #endregion Events
    }
} 