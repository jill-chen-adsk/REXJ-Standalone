using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.ExportSchedule;

namespace ADSK.JExtRAC.ExportSchedule.UI
{
    public partial class FormLog : Form
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
        /// <param name="cmpAttribute">Attributes</param>
        /// <param name="strLog">Log</param>
        ///
        /// <history><p>2021/10/13 Created Applied Technology</p></history>
        /// ================================================================================

        public FormLog(RvtExtApp.Components.Attribute cmpAttribute, System.Text.StringBuilder strLog)
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
        /// <history><p>2021/10/13 Created Applied Technology</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_LOG") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            this.btnSave.Text = _cmpAttribute.ResourceText("IDS_TXT_SAVELOG");
            this.btnClose.Text = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");
        }

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history><p>2021/10/13 Created Applied Technology</p></history>
        /// ================================================================================
        private
        void SetData()
        {
            RtxtLog.Text = _strLog.ToString();
        }

        /// ================================================================================
        /// <summary>Save dialog</summary>
        ///
        /// <history><p>2021/10/13 Created Applied Technology</p></history>
        /// ================================================================================
        private
        bool SaveLog()
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
                Autodesk.Revit.UI.TaskDialog.Show(_cmpAttribute.ResourceText("IDS_TXT_ERROR"), ex.ToString());
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
        /// <history>2021/10/13 Created Applied Technology</history>
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
        /// <history>2021/10/13 Created Applied Technology</history>
        /// ================================================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveLog())
                this.Close();
        }

        #endregion Events
    }
}
