using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.PrintRegion;
using Autodesk.Revit;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.PrintRegion.UI
{
    /// ================================================================================
    /// <summary>New</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public partial class SaveAsForm : System.Windows.Forms.Form
    {
        //Member variable

        #region Member variable

        /// <summary>interface</summary>
        private ISettingNameOperation m_settingNameOperation;

        /// <summary>RvtExtApp.Components.Attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        #endregion Member variable

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public SaveAsForm(ISettingNameOperation settingNameOperation, RvtExtApp.Components.Attribute cmpAttribute)
        {
            InitializeComponent();
            m_settingNameOperation = settingNameOperation;
            _CmpAttribute = cmpAttribute;
            SetText();
            SetData();
        }

        #endregion Constructor

        //Member Functions

        #region Member functions

        /// ================================================================================
        /// <summary>Set Text</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_NEW");
            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;

            this.lblName.Text = _CmpAttribute.ResourceText("IDS_TXT_NAME") + "(N):";

            this.btOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        /// ================================================================================
        /// <summary>Set Data</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetData()
        {
            newNameTextBox.Text = m_settingNameOperation.Prefix
                + m_settingNameOperation.SettingCount.ToString();
        }

        #endregion Member functions

        //Event

        #region Event

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void okButton_Click(object sender, EventArgs e)
        {
            if (m_settingNameOperation.SaveAs(newNameTextBox.Text))
                m_settingNameOperation.SettingName = newNameTextBox.Text;
        }

        #endregion Event
    }
}