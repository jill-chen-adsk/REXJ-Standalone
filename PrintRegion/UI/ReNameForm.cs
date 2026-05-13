using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.PrintRegion;

namespace ADSK.JExtRAC.PrintRegion.UI
{
    /// ================================================================================
    /// <summary>Rename</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public partial class ReNameForm : System.Windows.Forms.Form
    {
        //Member variable

        #region Member variable

        /// <summary>interface</summary>
        private ISettingNameOperation m_settingWithNameOperation;

        /// <summary>RvtExtApp.Components.Attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        #endregion Member variable

        //Member Functions

        #region Member functions

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="settingWithNameOperation">Interface</param>
        /// <param name="cmpAttribute">Attribute</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public ReNameForm(ISettingNameOperation settingWithNameOperation, RvtExtApp.Components.Attribute cmpAttribute)
        {
            InitializeComponent();
            m_settingWithNameOperation = settingWithNameOperation;
            _CmpAttribute = cmpAttribute;
            SetText();
            SetData();
        }

        #endregion Member functions

        //Member functions

        #region Member functions

        /// ================================================================================
        /// <summary>Set Text</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_RENAME");
            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;

            this.lblNew.Text = _CmpAttribute.ResourceText("IDS_TXT_NEW") + "(N):";
            this.lblPrevious.Text = _CmpAttribute.ResourceText("IDS_TXT_PREVIOUS");

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
            previousNameTextBox.Text =
            newNameTextBox.Text =
            m_settingWithNameOperation.SettingName;
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
            m_settingWithNameOperation.Rename(newNameTextBox.Text);
        }

        #endregion Event
    }
}