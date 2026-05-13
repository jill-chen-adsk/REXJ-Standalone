using ADSK.JExtRAC.PrintRegion.Components;
using System;
using System.Data;
using System.Drawing.Printing;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.PrintRegion;
using ADSK.JExtRAC.PrintRegion.Commands;
using ADSK.JExtRAC.PrintRegion.Request;

namespace ADSK.JExtRAC.PrintRegion.UI
{
    /// ================================================================================
    /// <summary>Form setting</summary>
    ///
    /// <history><p>2022/01/17 Created Applied Technology</p></history>
    /// ================================================================================
    public partial class PrintFrm : Form
    {
        // Member variables

        #region Member Variables

        /// <summary>Elements</summary>
        private Elements _CmpElements;

        /// <summary>RvtExtApp.Components.Attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>PrintMgr</summary>
        private PrintMgr m_printMgr = null;

        /// <summary>Is show</summary>
        private bool m_isShowing = false;

        /// <summary>Is user press OK or Cancel button</summary>
        private bool _isOkOrCancel = false;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        ///  <param name="cmpAttribute">Attribute</param>
        /// <param name="cmpElements">Elements</param>
        /// <param name="pMgr">PrintMgr</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        public PrintFrm(RvtExtApp.Components.Attribute cmpAttribute, Elements cmpElements, PrintMgr pMgr)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            m_printMgr = pMgr;

            cbScale.LostFocus += CbScale_LostFocus;
        }

        #endregion Constructor

        // Member functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Set Text</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_PRINT_REGION");
            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;

            this.lblPrintName.Text = _CmpAttribute.ResourceText("IDS_TXT_PRINTER_NAME");

            this.btPropetives.Text = _CmpAttribute.ResourceText("IDS_TXT_PROPERTIES");
            this.lblScale.Text = _CmpAttribute.ResourceText("IDS_TXT_PRINT_SCALE");

            this.btPreview.Text = _CmpAttribute.ResourceText("IDS_TXT_PREVIEW");
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
            try
            {
                PrinterSettings.StringCollection printers = PrinterSettings.InstalledPrinters;
                string[] printerNames = new string[printers.Count];
                printers.CopyTo(printerNames, 0);

                for (int i = 0; i < printerNames.Length; i++)
                {
                    string name = printerNames[i];
                    this.cbPrintName.Items.Add(name);
                }
                int printerIndex = cbPrintName.Items.IndexOf(m_printMgr.PrinterName);
                if (printerIndex != -1)
                {
                    cbPrintName.SelectedIndex = printerIndex;
                }
            }
            catch (Exception)
            {
            }

            DataTable dataScale = _CmpElements.GetDataScale();
            int index = 0;
            bool found = false;
            for (int i = 0; i < dataScale.Rows.Count; i++)
            {
                DataRow row = dataScale.Rows[i];
                if ((int)row["Value"] == CmdPrint._entData._viewScale)
                {
                    found = true;
                    index = i;
                    break;
                }
            }
            cbScale.DataSource = dataScale;
            cbScale.DisplayMember = "Name";
            cbScale.ValueMember = "Value";
            if (found)
            {
                cbScale.SelectedIndex = index;
            }
            else
            {
                DataRow row = dataScale.NewRow();
                row["Name"] = string.Format(_CmpAttribute.ResourceText("IDS_TXT_SCALE_FORMAT"), CmdPrint._entData._viewScale);
                row["Value"] = CmdPrint._entData._viewDuplicate.Scale;
                dataScale.Rows.InsertAt(row, 0);
                cbScale.SelectedIndex = 0;
            }
        }

        /// ================================================================================
        /// <summary>Get Data</summary>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private bool GetDataAndCheckError()
        {
            this.errPvd.SetError(this.cbScale, "");

            if (cbScale.SelectedValue != null)
            {
                CmdPrint._entData._viewScale = (int)(cbScale.SelectedValue);
                return false;
            }
            else
            {
                // start split val
                var text = cbScale.Text;
                var splitVal = text.Split(':');

                bool error = false;

                if (splitVal.Length < 2)
                {
                    error = true;
                }
                else
                {
                    int one = 0;
                    if (int.TryParse(splitVal[0], out one) == false)
                    {
                        error = true;
                    }
                    else
                    {
                        if (one != 1)
                        {
                            error = true;
                        }
                    }

                    if (int.TryParse(splitVal[1], out CmdPrint._entData._viewScale) == false)
                    {
                        error = true;
                    }
                }

                if (error)
                    this.errPvd.SetError(this.cbScale, _CmpAttribute.ResourceText("IDS_ERROR_FORMAT_SCALE"));

                return error;
            }
        }

        #endregion Member Functions

        // Events

        #region Events

        /// ================================================================================
        /// <summary>Load form</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void PrintFrm_Load(object sender, EventArgs e)
        {
            m_isShowing = true;
            SetText();
            SetData();

            m_isShowing = false;
        }

        /// ================================================================================
        /// <summary>Selected index change combobox</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void cbPrintName_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_printMgr.PrinterName = cbPrintName.SelectedItem as string;
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void btOK_Click(object sender, EventArgs e)
        {
            // Check error data
            if (GetDataAndCheckError())
                return;

            _isOkOrCancel = true;

            RequestHandler.Execute(RequestId.OK);
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btProperties control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2022/01/17 Created Applied Technology</p></history>
        /// ================================================================================
        private void btPropetives_Click(object sender, EventArgs e)
        {
            RequestHandler.Execute(RequestId.CHANGESETUP);
        }

        /// <summary> Key Press</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbScale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ':'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == ':') && ((sender as ComboBox).Text.IndexOf(':') > -1))
            {
                e.Handled = true;
            }
        }

        /// <summary>Lost Focus </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbScale_LostFocus(object sender, EventArgs e)
        {
            GetDataAndCheckError();
        }

        /// <summary>Selected Value Changed</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbScale_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_isShowing == false)
                GetDataAndCheckError();
        }

        /// <summary>Preview button click</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPreview_Click(object sender, EventArgs e)
        {
            // Check error data
            if (GetDataAndCheckError())
                return;

            RequestHandler.Execute(RequestId.PREVIEW);
        }

        /// <summary>Cancel button</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, EventArgs e)
        {
            _isOkOrCancel = true;

            RequestHandler.Execute(RequestId.CANCEL);

            this.Close();
        }

        /// <summary>Close form</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_isOkOrCancel)
                return;

            RequestHandler.Execute(RequestId.CANCEL);
        }

        #endregion Events
    }
}