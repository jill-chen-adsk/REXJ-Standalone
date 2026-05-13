using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;

namespace ADSK.JExtRAC.FloorHeightDimension.UI
{
    public partial class FormConfig : Form
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private RvtExtApp.Entities.DtCmd _EntDtCmd;
        private Dictionary<Revit.DB.DimensionType, string> _Dic_dimensionType;
        private Revit.DB.DimensionType _SelectedType;

        public FormConfig(RvtExtApp.Components.Attribute cmpAttribute,
                          RvtExtApp.Entities.DtCmd entDtCmd, IList<Revit.DB.DimensionType> list_dimensionType)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _EntDtCmd = entDtCmd;
            _Dic_dimensionType = list_dimensionType.ToDictionary(x => x, x => x.Name);
            SetText();
            SetData();
        }

        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_FLOORHEIGHTDIMENSION") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.gpbDist.Text = _CmpAttribute.ResourceText("IDS_TXT_GROUP");
            this.lblDist.Text = _CmpAttribute.ResourceText("IDS_TXT_DIST");
            this.lblDistUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");

            this.lblTotalDist.Text = _CmpAttribute.ResourceText("IDS_TXT_TOTALDIST");
            this.lblTotalDistUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");
            this.lblType.Text = _CmpAttribute.ResourceText("IDS_TXT_TYPE");

            this.btnSelPos.Text = _CmpAttribute.ResourceText("IDS_TXT_SELPOS");

            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            this.ckbMultiView.Text = _CmpAttribute.ResourceText("IDS_TXT_MULTIVIEW");

            this.picDist.Image = _CmpAttribute.ResourceImage("IDI_PIC_LEVEL") as System.Drawing.Image;

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        private static bool TryParseDouble(string s, out double d)
        {
            return double.TryParse(s?.Trim(), NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out d);
        }

        private static bool IsBool(string s)
        {
            return bool.TryParse(s?.Trim(), out _);
        }

        private void SetData()
        {
            double dValue = 0.0;
            string sValue = "";
            bool checkValue = false;

            if (_EntDtCmd.Data.Count >= 4)
            {
                sValue = _EntDtCmd.Data[0];
                dValue = 0.0;
                if (!string.IsNullOrWhiteSpace(sValue) && TryParseDouble(sValue, out dValue))
                    this.txtDist.Text = dValue.ToString(CultureInfo.CurrentCulture);
                else
                    this.txtDist.Text = "0";

                sValue = _EntDtCmd.Data[1];
                dValue = 0.0;
                if (!string.IsNullOrWhiteSpace(sValue) && TryParseDouble(sValue, out dValue))
                    this.txtTotalDist.Text = dValue.ToString(CultureInfo.CurrentCulture);
                else
                    this.txtTotalDist.Text = "0";

                sValue = _EntDtCmd.Data[2];
                checkValue = false;
                if (IsBool(sValue))
                    checkValue = bool.Parse(sValue);

                this.ckbMultiView.Checked = checkValue;

                cbDimensionType.DataSource = new BindingSource(_Dic_dimensionType, null);
                cbDimensionType.DisplayMember = "Value";
                cbDimensionType.ValueMember = "Key";

                sValue = _EntDtCmd.Data[3];
                if (!string.IsNullOrEmpty(sValue))
                {
                    if (_Dic_dimensionType.ContainsValue(sValue))
                        cbDimensionType.SelectedIndex = _Dic_dimensionType.Values.ToList().IndexOf(sValue);
                }
            }
        }

        public Revit.DB.DimensionType GetSelectDimensionType => _SelectedType;

        private void GetData()
        {
            if (_EntDtCmd.Data.Count >= 4)
            {
                double dValue = 0.0;
                string sValue = "";

                dValue = 0.0;
                sValue = this.txtDist.Text;
                if (TryParseDouble(sValue, out dValue))
                    _EntDtCmd.Data[0] = dValue.ToString(CultureInfo.InvariantCulture);

                dValue = 0.0;
                sValue = this.txtTotalDist.Text;
                if (TryParseDouble(sValue, out dValue))
                    _EntDtCmd.Data[1] = dValue.ToString(CultureInfo.InvariantCulture);

                _EntDtCmd.Data[2] = ckbMultiView.Checked.ToString();

                _SelectedType = (Revit.DB.DimensionType)cbDimensionType.SelectedValue;

                _EntDtCmd.Data[3] = _SelectedType.Name;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GetData();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnSelPos_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        private void cbDimensionType_DropDown(object sender, EventArgs e)
        {
            try
            {
                object[] items = new object[cbDimensionType.Items.Count];
                cbDimensionType.Items.CopyTo(items, 0);
                cbDimensionType.DropDownWidth = items.Select(obj => TextRenderer.MeasureText(cbDimensionType.GetItemText(obj), cbDimensionType.Font).Width).Max();
            }
            catch { }
        }
    }
}
