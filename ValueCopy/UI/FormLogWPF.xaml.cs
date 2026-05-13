using System;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Win32;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.ValueCopy;

namespace ADSK.JExtRAC.ValueCopy.UI
{
    /// ================================================================================
    /// <summary>FormLogWPF</summary>
    ///
    /// <history>2024/03/21 Created</history>
    /// ================================================================================
    public partial class FormLogWPF : Window
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

            _cmpAttribute = cmpAttribute;
            _strLog = strLog;

            SetText();
            SetData();
        }

        #endregion Constructor

        // Member function

        #region Member Functions

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void SetText()
        {
            this.Title = _cmpAttribute.ResourceText("IDS_TXT_LOG");
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
                Revit.UI.TaskDialog.Show(_cmpAttribute.ResourceText("IDS_TXT_ERROR"), ex.ToString());
                return false;
            }
        }

        #endregion Member Functions

        // Events

        #region Events

        /// ================================================================================
        /// <summary>Handles the Click event of the btnClose control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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