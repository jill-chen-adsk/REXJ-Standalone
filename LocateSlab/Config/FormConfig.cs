using System;
using System.Reflection;
using System.Windows.Forms;
using ADSK.JExtRAC.LocateSlab.Components;
using ADSK.JExtRAC.LocateSlab.Entities;

namespace ADSK.JExtRAC.LocateSlab.Config
{
    public partial class FormConfig : Form
    {
        private readonly Components.Attribute _cmpAttribute;
        private readonly DtSlabType _entDtSlabType;
        private readonly DtCmd _entDtCmd;

        public FormConfig(Components.Attribute cmpAttribute, DtSlabType entDtSlabType, DtCmd entDtCmd)
        {
            InitializeComponent();
            _cmpAttribute = cmpAttribute;
            _entDtSlabType = entDtSlabType;
            _entDtCmd = entDtCmd;

            SetText();
            SetData();
        }

        private void SetText()
        {
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_LOCATESLAB") +
                string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            this.lblSlabType.Text = _cmpAttribute.ResourceText("IDS_TXT_SLABTYPE");
            this.lblHeightOffset.Text = _cmpAttribute.ResourceText("IDS_TXT_LEVELHEIGHTOFFSET");
            this.lblHeightOffsetUnit.Text = _cmpAttribute.ResourceText("IDS_UNIT_MM");
            this.lblDirectionAngle.Text = _cmpAttribute.ResourceText("IDS_TXT_DIRECTION");
            this.lblDegree.Text = _cmpAttribute.ResourceText("IDS_DEGREE");
            this.btnOK.Text = _cmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        private void SetData()
        {
            this.cboSlabType.DataSource = _entDtSlabType.Data;
            this.cboSlabType.DisplayMember = _entDtSlabType.ColNameName;
            this.cboSlabType.ValueMember = _entDtSlabType.ColNameID;

            this.cboDirectionAngle.DataSource = _entDtCmd.DataDirection;
            this.cboDirectionAngle.DisplayMember = "Name";
            this.cboDirectionAngle.ValueMember = "Value";

            if (_entDtCmd.Data != null && _entDtCmd.Data.Count >= 3)
            {
                double dValue = 0.0;
                if (double.TryParse(_entDtCmd.Data[0], out double ho)) dValue = ho;
                this.txtHeightOffset.Text = dValue.ToString();

                string sValue = _entDtCmd.Data[1] ?? "";
                this.cboSlabType.Text = sValue;

                if (!string.IsNullOrEmpty(_entDtCmd.Data[2]))
                {
                    this.cboDirectionAngle.SelectedValue = _entDtCmd.Data[2];
                    this.cboDirectionAngle.Text = _entDtCmd.Data[2];
                }
                else if (this.cboDirectionAngle.Items.Count > 0)
                {
                    this.cboDirectionAngle.SelectedValue = "0";
                }
            }
        }

        private void GetData()
        {
            if (_entDtCmd.Data != null && _entDtCmd.Data.Count >= 3)
            {
                double dValue = 0.0;
                if (double.TryParse(this.txtHeightOffset.Text, out double ho)) dValue = ho;
                _entDtCmd.Data[0] = dValue.ToString();

                _entDtCmd.Data[1] = this.cboSlabType.Text ?? "";

                _entDtCmd.DegreeAngle = 0;
                if (this.cboDirectionAngle.SelectedValue != null)
                    _entDtCmd.DegreeAngle = double.Parse(this.cboDirectionAngle.SelectedValue.ToString());
                else
                    double.TryParse(this.cboDirectionAngle.Text, out double da);

                _entDtCmd.Data[2] = _entDtCmd.DegreeAngle.ToString();
            }

            if (this.cboSlabType.SelectedValue != null &&
                int.TryParse(this.cboSlabType.SelectedValue.ToString(), out int elemId))
            {
                _entDtSlabType.GetWorkElem(elemId);
            }
        }

        private void ChkErrPvd()
        {
            this.errPvd.SetError(this.cboDirectionAngle,
                _entDtCmd.SetErrPvdDecimalText(this.cboDirectionAngle.Text.Trim()));
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

        private void cboDirectionAngle_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.cboDirectionAngle,
                _entDtCmd.SetErrPvdDecimalText(this.cboDirectionAngle.Text.Trim()));
        }
    }
}
