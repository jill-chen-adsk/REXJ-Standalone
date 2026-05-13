using System;
using System.Reflection;
using System.Windows.Forms;
using ADSK.JExtRAC.AutomaticFloor.Utils;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.Config
{
    public partial class FormConfig : Form
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private RvtExtApp.Entities.DtSlabType _EntDtSlabType;
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        public FormConfig(RvtExtApp.Components.Attribute cmpAttribute,
                          RvtExtApp.Entities.DtSlabType entDtSlabType,
                          RvtExtApp.Entities.DtCmd entDtCmd,
                          eFloorType eFloorType)
        {
            _CmpAttribute = cmpAttribute;
            _EntDtSlabType = entDtSlabType;
            _EntDtCmd = entDtCmd;

            InitializeComponent();
            SetText(eFloorType);
            SetData();
        }

        private void SetText(eFloorType eFloorType)
        {
            if (eFloorType == eFloorType.Arch)
            {
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_ARCHITECTURE") + string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
                this.lblSlabType.Text = _CmpAttribute.ResourceText("IDS_TXT_ARCHITECTURE_FTYPE");
                this.lblLock.Text = _CmpAttribute.ResourceText("IDS_TXT_LOCK_ARCHITECT");
                this.lblDirectionAngle.Text = _CmpAttribute.ResourceText("IDS_TXT_SLAB_DIRECTION");
            }
            else if (eFloorType == eFloorType.Struct)
            {
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_STRUCTURAL") + string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
                this.lblSlabType.Text = _CmpAttribute.ResourceText("IDS_TXT_STRUCTURAL_FTYPE");
                this.lblLock.Text = _CmpAttribute.ResourceText("IDS_TXT_LOCK_STRUCTURAL");
                this.lblDirectionAngle.Text = _CmpAttribute.ResourceText("IDS_TXT_SLAB_DIRECTION");
            }
            else
            {
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_FOUDATION_SLAB") + string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
                this.lblSlabType.Text = _CmpAttribute.ResourceText("IDS_TXT_FOUDATION_SLAB_FTYPE");
                this.lblLock.Text = _CmpAttribute.ResourceText("IDS_TXT_LOCK_FOUNDATION_SLAB");
                this.lblDirectionAngle.Text = _CmpAttribute.ResourceText("IDS_TXT_SLAB_DIRECTION");
            }

            this.lblHeightOffset.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELHEIGHTOFFSET");
            this.lblHeightOffsetUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");
            this.lblDegree.Text = _CmpAttribute.ResourceText("IDS_DEGREE");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            var icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
            if (icon != null) this.Icon = icon;
        }

        private void SetData()
        {
            double dValue = 0.0;
            string sValue = "";

            this.cboSlabType.DataSource = _EntDtSlabType.Data;
            this.cboSlabType.DisplayMember = _EntDtSlabType.ColNameName;
            this.cboSlabType.ValueMember = _EntDtSlabType.ColNameID;

            this.cboDirectionAngle.DataSource = _EntDtCmd.DataDirection;
            this.cboDirectionAngle.DisplayMember = "Name";
            this.cboDirectionAngle.ValueMember = "Value";

            if (_EntDtCmd.Data.Count >= 4)
            {
                sValue = _EntDtCmd.Data[0];
                dValue = 0.0;
                if (double.TryParse(sValue, out double parsedVal))
                    dValue = parsedVal;
                this.txtHeightOffset.Text = dValue.ToString();

                sValue = _EntDtCmd.Data[1] ?? "";
                this.cboSlabType.Text = sValue;

                this.chbLock.Checked = _EntDtCmd.Data[2] == "true";

                if (!string.IsNullOrEmpty(_EntDtCmd.Data[3]))
                {
                    this.cboDirectionAngle.SelectedValue = _EntDtCmd.Data[3];
                    this.cboDirectionAngle.Text = _EntDtCmd.Data[3];
                }
                else if (this.cboDirectionAngle.Items.Count > 0)
                    this.cboDirectionAngle.SelectedValue = "0";
            }
        }

        private void GetData()
        {
            string sValue;
            if (_EntDtCmd.Data.Count >= 4)
            {
                double dValue = 0.0;
                sValue = this.txtHeightOffset.Text;
                if (double.TryParse(sValue, out double parsedVal))
                    dValue = parsedVal;
                _EntDtCmd.Data[0] = dValue.ToString();

                sValue = this.cboSlabType.Text ?? "";
                _EntDtCmd.Data[1] = sValue;

                _EntDtCmd.Data[2] = this.chbLock.Checked ? "true" : "false";

                _EntDtCmd.DegreeAngle = 0.0;
                if (this.cboDirectionAngle.SelectedValue != null)
                    _EntDtCmd.DegreeAngle = double.Parse(this.cboDirectionAngle.SelectedValue.ToString());
                else
                    _EntDtCmd.DegreeAngle = double.Parse(this.cboDirectionAngle.Text);
                _EntDtCmd.Data[3] = _EntDtCmd.DegreeAngle.ToString();
            }

            int iValue = 0;
            sValue = this.cboSlabType.SelectedValue?.ToString() ?? "0";
            if (int.TryParse(sValue, out int parsedInt))
                iValue = parsedInt;
            _EntDtSlabType.GetWorkElem(iValue);
        }

        private void ChkErrPvd()
        {
            this.errPvd.SetError(this.cboDirectionAngle, _EntDtCmd.SetErrPvdDecimalText(this.cboDirectionAngle.Text.Trim()));
        }

        private bool GetErrPvd()
        {
            return string.IsNullOrEmpty(this.errPvd.GetError(this.cboDirectionAngle));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ChkErrPvd();
            if (GetErrPvd())
            {
                GetData();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void txtHeightOffset_KeyPress(object sender, KeyPressEventArgs e)
        {
            Common.NumberCheck(sender, e, true);
        }

        private void cboDirectionAngle_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.cboDirectionAngle, _EntDtCmd.SetErrPvdDecimalText(this.cboDirectionAngle.Text.Trim()));
        }
    }
}
