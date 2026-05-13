using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SectionListRC.Setting
{
    public partial class FormColumnOption : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private Components.Attribute _CmpAttribute;

        private bool _ExportImage = false;

        private List<string> _Settings = null;

        private int _iType = 0;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        public FormColumnOption(Components.Attribute cmpAttribute, bool exportImage, List<string> settings, int iType)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _ExportImage = exportImage;

            rdoAll.Checked = true;

            chkRange.Enabled = true;
            if (_ExportImage == true)
            {
                chkRange.Enabled = false;

                txtMaxLength.Enabled = chkRange.Checked;
                txtMaxWidth.Enabled = chkRange.Checked;

                lblMaxHeight.Enabled = chkRange.Checked;
                lblMaxWidth.Enabled = chkRange.Checked;

                lblmm1.Enabled = chkRange.Checked;
                lblmm2.Enabled = chkRange.Checked;
            }

            txtMaxLength.Enabled = false;
            txtMaxWidth.Enabled = false;
            lblMaxHeight.Enabled = false;
            lblMaxWidth.Enabled = false;

            lblmm1.Enabled = false;
            lblmm2.Enabled = false;

            _Settings = settings;
            BtnEnabledChange();

            _iType = iType;
        }

        #endregion Constructor

        // プロパティ

        #region Properties

        public bool IsColumnTypeChecked
        {
            get
            {
                return chkColumnType.Checked;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEnabledColumnType
        {
            set
            {
                chkColumnType.Enabled = value;

                if (value == false)
                    chkColumnType.Checked = false;
            }

            get
            {
                return chkColumnType.Enabled;
            }
        }

        public bool IsPostTypeChecked
        {
            get
            {
                return chkColumnPost.Checked;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEnabledPostType
        {
            set
            {
                chkColumnPost.Enabled = value;

                if (value == false)
                    chkColumnPost.Checked = false;
            }

            get
            {
                return chkColumnPost.Enabled;
            }
        }

        public bool IsExportAllChecked
        {
            get
            {
                return rdoAll.Checked;
            }
        }

        public bool ByRange
        {
            get
            {
                return chkRange.Checked;
            }
        }

        public bool IsEnableByRange
        {
            get
            {
                return chkRange.Enabled;
            }
        }

        public double MaxHeight
        {
            get
            {
                double value = 0;
                if (double.TryParse(txtMaxLength.Text.Trim(), out value) == false)
                    return double.MinValue;

                if (value == 0)
                    return double.MinValue;

                return value;
            }
        }

        public double MaxWidth
        {
            get
            {
                double value = 0;
                if (double.TryParse(txtMaxWidth.Text.Trim(), out value) == false)
                    return double.MinValue;

                if (value == 0)
                    return double.MinValue;

                return value;
            }
        }

        public List<string> GetStringSetting
        {
            get
            {
                if (_Settings == null || _Settings.Count != 9)
                    return _Settings;

                List<string> list = new List<string>();
                if (IsColumnTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_COLUMN_TYPE_ENG"));

                if (IsPostTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_POST_TYPE_ENG"));

                _Settings[0] = string.Join(";", list.ToArray());

                if (IsExportAllChecked)
                {
                    _Settings[1] = _CmpAttribute.ResourceText("IDS_TXT_ALL_ENG");
                }
                else
                    _Settings[1] = _CmpAttribute.ResourceText("IDS_TXT_PART_ENG");

                if (ByRange)
                {
                    _Settings[2] = _CmpAttribute.ResourceText("IDS_TXT_TRUE_ENG");
                }
                else
                {
                    _Settings[2] = _CmpAttribute.ResourceText("IDS_TXT_FALSE_ENG");
                }
                _Settings[3] = (txtMaxLength.Text.Trim());
                _Settings[4] = (txtMaxWidth.Text.Trim());

                return _Settings;
            }
        }

        #endregion Properties

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>Set text for control </summary>
        /// ================================================================================
        private void SetData()
        {
            if(_iType == 0)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMN_OPTION");
            else if(_iType == 1)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_EACHONECOL_OPTIONS");
            else if (_iType == 2)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_COL_IMAGE_OPTIONS");

            this.grpBoxSelectType.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_TYPE");
            this.chkColumnType.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNTYPE");
            this.chkColumnPost.Text = _CmpAttribute.ResourceText("IDS_TXT_POSTTYPE");

            this.grpBoxRange.Text = _CmpAttribute.ResourceText("IDS_TXT_GROUP_RANGE");
            this.rdoAll.Text = _CmpAttribute.ResourceText("IDS_TXT_CHECK_ALL");
            this.chkRange.Text = _CmpAttribute.ResourceText("IDS_TXT_CHECK_RANGER");

            this.lblMaxHeight.Text = _CmpAttribute.ResourceText("IDS_TXT_MAX_HEIGHT");
            this.lblMaxWidth.Text = _CmpAttribute.ResourceText("IDS_TXT_MAX_WIDTH");

            this.rdoSelection.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_ITEMS");

            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            this.lblmm1.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblmm2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.txtMaxLength.MaxLength = 5;
            this.txtMaxWidth.MaxLength = 5;
        }

        /// ================================================================================
        /// <summary>Form loaded event </summary>
        /// ================================================================================
        private void FormColumnSelection_Load(object sender, EventArgs e)
        {
            ReadSetting();
            SetDefault();
            SetData();

            BtnEnabledChange();
        }

        /// ================================================================================
        /// <summary> Read setting value</summary>
        /// ================================================================================
        private void ReadSetting()
        {
            if (_Settings == null || _Settings.Count != 9)
                return;

            var type = _Settings[0];
            var splitTypes = type.Split(';');

            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_COLUMN_TYPE_ENG")) && IsEnabledColumnType)
                chkColumnType.Checked = true;
            else
                chkColumnType.Checked = false;

            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_POST_TYPE_ENG")) && IsEnabledPostType)
                chkColumnPost.Checked = true;
            else
                chkColumnPost.Checked = false;

            if (_Settings.Contains(_CmpAttribute.ResourceText("IDS_TXT_ALL_ENG")))
            {
                rdoAll.Checked = true;
            }
            else
                rdoAll.Checked = false;

            if (_Settings.Contains(_CmpAttribute.ResourceText("IDS_TXT_TRUE_ENG")))
            {
                chkRange.Checked = true;
            }
            else
                chkRange.Checked = false;

            int index = _Settings.IndexOf(_CmpAttribute.ResourceText("IDS_TXT_TRUE_ENG"));
            if (index == -1)
            {
                index = _Settings.IndexOf(_CmpAttribute.ResourceText("IDS_TXT_FALSE_ENG"));
            }

            if (index != -1 && index + 2 < _Settings.Count)
            {
                var maxLengthStr = _Settings[index + 1];
                var maxWidthStr = _Settings[index + 2];

                txtMaxLength.Text = maxLengthStr;
                txtMaxWidth.Text = maxWidthStr;
            }

            if (_Settings.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_PART_ENG")))
                rdoSelection.Checked = true;
            else
                rdoSelection.Checked = false;

            //
            if (_Settings.Contains(_CmpAttribute.ResourceText("IDS_TXT_ALL_ENG")) == false && _Settings.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_PART_ENG")) == false)
                rdoAll.Checked = true;
            //
        }

        /// <summary>Set default checkbox </summary>
        private void SetDefault()
        {
            if (IsColumnTypeChecked == false && IsPostTypeChecked == false)
            {
                if (IsEnabledColumnType)
                    chkColumnType.Checked = true;

                if (IsEnabledPostType)
                    chkColumnPost.Checked = true;
            }
        }

        /// ================================================================================
        /// <summary>Button clicked event </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsColumnTypeChecked == false && IsPostTypeChecked == false)
            {
                MessageBox.Show("Please select at least one type to create the column list.");
                return;
            }
            if (IsExportAllChecked && ByRange)
            {
                if (MaxHeight == double.MinValue)
                {
                    txtMaxLength.Focus();
                    return;
                }
                if (MaxWidth == double.MinValue)
                {
                    txtMaxWidth.Focus();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        /// ================================================================================
        /// <summary>Button clicked event </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// ================================================================================
        /// <summary>Checked changed event </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void chkColumnType_CheckedChanged(object sender, EventArgs e)
        {
            ValidateRadioAll();

            BtnEnabledChange();
        }

        /// ================================================================================
        /// <summary>Checked changed event </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void chkColumnPost_CheckedChanged(object sender, EventArgs e)
        {
            ValidateRadioAll();

            BtnEnabledChange();
        }

        /// ================================================================================
        /// <summary>Validate radio button</summary>
        /// ================================================================================
        private void ValidateRadioAll()
        {
            rdoSelection.Enabled = true;
            if (chkColumnType.Checked && chkColumnPost.Checked)
            {
                rdoAll.Checked = true;

                rdoSelection.Enabled = false;
            }
        }

        /// ================================================================================
        /// <summary>Checked changed event </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void chkRange_CheckedChanged(object sender, EventArgs e)
        {
            txtMaxLength.Enabled = chkRange.Checked;
            txtMaxWidth.Enabled = chkRange.Checked;

            lblMaxHeight.Enabled = chkRange.Checked;
            lblMaxWidth.Enabled = chkRange.Checked;

            lblmm1.Enabled = chkRange.Checked;
            lblmm2.Enabled = chkRange.Checked;

            if (_ExportImage == true)
            {
                chkRange.Enabled = false;

                txtMaxLength.Enabled = chkRange.Enabled;
                txtMaxWidth.Enabled = chkRange.Enabled;

                lblMaxHeight.Enabled = chkRange.Enabled;
                lblMaxWidth.Enabled = chkRange.Enabled;

                lblmm1.Enabled = chkRange.Enabled;
                lblmm2.Enabled = chkRange.Enabled;
            }

            BtnEnabledChange();
        }

        /// ================================================================================
        /// <summary>Checked changed event </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void rdoAll_CheckedChanged(object sender, EventArgs e)
        {
            if (_ExportImage == false)
            {
                chkRange.Enabled = rdoAll.Checked;
                txtMaxLength.Enabled = chkRange.Checked;
                txtMaxWidth.Enabled = chkRange.Checked;

                lblMaxHeight.Enabled = chkRange.Checked;
                lblMaxWidth.Enabled = chkRange.Checked;

                lblmm1.Enabled = chkRange.Checked;
                lblmm2.Enabled = chkRange.Checked;
            }
            BtnEnabledChange();
        }

        /// ================================================================================
        /// <summary>Key press event</summary>
        /// <param name="sender">Textbox control</param>
        /// <param name="e">Key press event</param>
        /// ================================================================================
        private void txtMaxLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Key press event</summary>
        /// <param name="sender">Textbox control</param>
        /// <param name="e">Key press event</param>
        /// ================================================================================
        private void txtMaxWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Checked changed event </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void rdoSelection_CheckedChanged(object sender, EventArgs e)
        {
            chkRange.Enabled = false;
            txtMaxLength.Enabled = false;
            txtMaxWidth.Enabled = false;

            lblMaxHeight.Enabled = false;
            lblMaxWidth.Enabled = false;

            lblmm1.Enabled = false;
            lblmm2.Enabled = false;
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定</summary>
        ///
        /// <history><p>2019/10/2 Created Applied Technology</p></history>
        /// ================================================================================
        private bool IsDoubleStr(string strVal)
        {
            bool ret = false;

            double outDouble = 0;

            if (double.TryParse(strVal, out outDouble))
            {
                if (outDouble != 0 && outDouble != 0.0 && outDouble > 0)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ボタン使用可否切り替え</summary>
        ///
        /// <history><p>2013/02/26 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void BtnEnabledChange()
        {
            if (IsColumnTypeChecked == false && IsPostTypeChecked == false)
            {
                this.btnOK.Enabled = false;
                return;
            }

            if (IsExportAllChecked && ByRange && (MaxHeight == double.MinValue || MaxWidth == double.MinValue))
            {
                this.btnOK.Enabled = false;
                return;
            }

            this.errorProviderInvalid.SetError(this.txtMaxLength, "");
            this.errorProviderInvalid.SetError(this.txtMaxWidth, "");

            this.btnOK.Enabled = true;
        }

        /// <summary> Textbox leave event </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void txtMaxLength_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleStr(this.txtMaxLength.Text))
            {
                this.errorProviderInvalid.SetError(this.txtMaxLength, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                   "\r\n" +
                                                                                   _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEGREATERTHANZERO"));
                this.txtMaxLength.Select();
                this.txtMaxLength.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtMaxLength, "");
            }
            BtnEnabledChange();
        }

        /// <summary> Textbox leave event </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void txtMaxWidth_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleStr(this.txtMaxWidth.Text))
            {
                this.errorProviderInvalid.SetError(this.txtMaxWidth, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                   "\r\n" +
                                                                                   _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLEGREATERTHANZERO"));
                this.txtMaxWidth.Select();
                this.txtMaxWidth.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtMaxWidth, "");
            }

            BtnEnabledChange();
        }

        #endregion Member Functions
    }
}