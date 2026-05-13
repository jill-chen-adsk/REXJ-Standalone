using System;
using System.Reflection;
using System.Windows.Forms;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;

namespace ADSK.JExtRAC.AutoLayoutTag.UI
{
    /// ================================================================================
    /// <summary>FormInfor</summary>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public partial class FormInfo : Form
    {
        // Member variable

        #region Member Variables

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _cmpAttribute;

        /// <summary>Log</summary>
        private System.Text.StringBuilder _strLog;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute"  >Parameter</param>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================

        public FormInfo(RvtExtApp.Components.Attribute cmpAttribute, System.Text.StringBuilder strLog)
        {
            InitializeComponent();

            _cmpAttribute = cmpAttribute;
            _strLog = strLog;

            this.TopMost = true;

            SetText();
            SetData();
        }

        #endregion Constructor

        // Member function

        #region Member Functions

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_LOG") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            this.Icon = _cmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
            this.btnSave.Text = _cmpAttribute.ResourceText("IDS_TXT_SAVELOG");
            this.btnClose.Text = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");
        }

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private void SetData()
        {
            RtxtLog.Text = _strLog.ToString();
        }

        /// ================================================================================
        /// <summary>Save dialog</summary>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private bool SaveLog()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Export log";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text Documents (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RtxtLog.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.UnicodePlainText);
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
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnSave control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveLog())
                this.Close();
        }

        #endregion Events
    }
}