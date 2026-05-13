using System;
using System.Windows.Forms;

namespace ADSK.JExtRAC.AutoCreateDimension.UI
{
    /// ================================================================================
    /// <summary>FormLog</summary>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public partial class FormLog : Form
    {
        // Member variable

        #region Member Variables

        /// <summary>Attributes</summary>
        private Components.Attribute _cmpAttribute;

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

        public FormLog(Components.Attribute cmpAttribute, System.Text.StringBuilder strLog)
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
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_LOG");
            this.Icon = Resources.Image.IDI_SUBS_ICON;
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

        #endregion Events
    }
}
