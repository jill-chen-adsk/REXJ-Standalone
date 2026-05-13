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
    public partial class FormBeamOption : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private Components.Attribute _CmpAttribute;

        private List<(string, bool, bool)> _TupleList = null;

        private bool _ExportImage = false;

        #region Variables for header checkbox

        private int _TotalCheckBoxes = 0;
        private int _TotalCheckedCheckBoxes = 0;
        private CheckBox _HeaderCheckBox = null;
        private bool _IsHeaderCheckBoxClicked = false;

        private int _iType = 0;

        #endregion Variables for header checkbox

        #endregion Member Variables

        #region Properties

        public bool IsGirderTypeChecked = false;
        public bool IsCantiBeamTypeChecked = false;
        public bool IsFoundationGirderTypeChecked = false;
        public bool IsCantiFoundationGirderTypeChecked = false;
        public bool IsFoundationBeamTypeChecked = false;
        public bool IsCantiGirderTypeChecked = false;
        public bool IsBeamTypeChecked = false;
        public bool IsCantiFoundationBeamTypeChecked = false;

        public bool IsEnabledFoundationBeamType = false;
        public bool IsEnabledCantiFoundationGirderType = false;
        public bool IsEnabledFoundationGirdeType = false;
        public bool IsEnabledCantiBeamType = false;
        public bool IsEnabledBeamType = false;
        public bool IsEnabledGirderType = false;
        public bool IsEnabledCantiGirderType = false;
        public bool IsEnabledCantiFoundationBeamType = false;

        private List<string> _Settings = null;

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
                if (IsBeamTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_BEAM_TYPE_ENG"));

                if (IsGirderTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_GIRDER_TYPE_ENG"));

                if (IsFoundationGirderTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_FOUD_GIRDER_TYPE_ENG"));

                if (IsFoundationBeamTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_FOUD_BEAM_TYPE_ENG"));

                if (IsCantiGirderTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_TYPE_ENG"));

                if (IsCantiBeamTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_CANTIBEAM_TYPE_ENG"));

                if (IsCantiFoundationGirderTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_CANTI_FOUD_GIRDER_TYPE_ENG"));

                if (IsCantiFoundationBeamTypeChecked)
                    list.Add(_CmpAttribute.ResourceText("IDS_TXT_CANTI_FOUD_BEAM_TYPE_ENG"));

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

        // コンストラクタ

        #region Constructor

        public FormBeamOption(Components.Attribute cmpAttribute, bool exportImage, List<string> valueSetting, int iType)
        {
            InitializeComponent();
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

            _CmpAttribute = cmpAttribute;

            dgvItems.RowHeadersVisible = false;
            dgvItems.AllowUserToResizeRows = false;
            dgvItems.AllowUserToResizeColumns = false;

            AddHeaderCheckBox();
            foreach (DataGridViewColumn col in dgvItems.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font(this.dgvItems.Font.Name, 12F, FontStyle.Bold, GraphicsUnit.Pixel);
            }

            _Settings = valueSetting;
            BtnEnabledChange();

            _iType = iType;
        }

        #endregion Constructor

        #region Member Functions

        /// ================================================================================
        /// <summary>Form loaded event </summary>
        /// ================================================================================
        private void FormBeamOption_Load(object sender, EventArgs e)
        {
            ReadSetting();
            SetDefault();

            _TupleList = new List<(string, bool, bool)>
  {
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_GIRDER"), IsGirderTypeChecked, IsEnabledGirderType),
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_FOUNDATION_GIRDER"), IsFoundationGirderTypeChecked, IsEnabledFoundationGirdeType),
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_GIRDER"), IsCantiGirderTypeChecked, IsEnabledCantiGirderType),
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_FOUNDATION_GIRDER"), IsCantiFoundationGirderTypeChecked, IsEnabledCantiFoundationGirderType),
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_BEAM"), IsBeamTypeChecked, IsEnabledBeamType),
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_FOUNDATION_BEAM"), IsFoundationBeamTypeChecked, IsEnabledFoundationBeamType),
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_BEAM"), IsCantiBeamTypeChecked, IsEnabledCantiBeamType),
      (_CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_FOUNDATION_BEAM"), IsCantiFoundationBeamTypeChecked, IsEnabledCantiFoundationBeamType),
  };

            SetData();

            InitDataGridView();

            _HeaderCheckBox.KeyUp += new KeyEventHandler(HeaderCheckBox_KeyUp);
            _HeaderCheckBox.MouseClick += new MouseEventHandler(HeaderCheckBox_MouseClick);

            BtnEnabledChange();
        }

        #region Events for header checkbox

        /// ================================================================================
        /// <summary>Header checkBox mouse click event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ================================================================================
        private void HeaderCheckBox_MouseClick(object sender, MouseEventArgs e)
        {
            HeaderCheckBoxClick((CheckBox)sender);
        }

        /// ================================================================================
        /// <summary>Header checkBox key up event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ================================================================================
        private void HeaderCheckBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                HeaderCheckBoxClick((CheckBox)sender);
        }

        /// ================================================================================
        /// <summary>Row CheckBox click</summary>
        /// <param name="rCheckBox"></param>
        /// ================================================================================
        private void RowCheckBoxClick(DataGridViewCheckBoxCell rCheckBox)
        {
            if (rCheckBox != null)
            {
                //Modifiy Counter;
                if ((bool)rCheckBox.Value && _TotalCheckedCheckBoxes < _TotalCheckBoxes)
                {
                    _TotalCheckedCheckBoxes++;
                }
                else if (_TotalCheckedCheckBoxes > 0)
                {
                    _TotalCheckedCheckBoxes--;
                }
                //Change state of the header CheckBox.
                ValidateHeaderCheckbox();
                ValidateRadioAll();

                BtnEnabledChange();
            }
        }

        #endregion Events for header checkbox

        /// ================================================================================
        /// <summary>Add header checkBox</summary>
        /// ================================================================================
        private void AddHeaderCheckBox()
        {
            _HeaderCheckBox = new CheckBox();

            _HeaderCheckBox.Size = new Size(15, 15);

            //Add the CheckBox into the DataGridView
            this.dgvItems.Controls.Add(_HeaderCheckBox);
        }

        /// ================================================================================
        /// <summary> Reset header CheckBox location </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// ================================================================================
        private void ResetHeaderCheckBoxLocation(int columnIndex, int rowIndex)
        {
            //Get the column header cell bounds
            var oRectangle = this.dgvItems.GetCellDisplayRectangle(columnIndex, rowIndex, true);

            var oPoint = new System.Drawing.Point();

            oPoint.X = oRectangle.Location.X + (oRectangle.Width - _HeaderCheckBox.Width) / 2 + 1;
            oPoint.Y = oRectangle.Location.Y + (oRectangle.Height - _HeaderCheckBox.Height) / 2 + 1;

            //Change the location of the CheckBox to make it stay on the header
            _HeaderCheckBox.Location = oPoint;
        }

        /// ================================================================================
        /// <summary> Header CheckBox click </summary>
        /// <param name="hCheckBox"></param>
        /// ================================================================================
        private void HeaderCheckBoxClick(CheckBox hCheckBox)
        {
            _IsHeaderCheckBoxClicked = true;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                DataGridViewCheckBoxCell rCheckBox = (DataGridViewCheckBoxCell)row.Cells[0];
                var item = _TupleList.ElementAt(row.Index);

                if (hCheckBox.Checked == true && item.Item3 == true)
                {
                    rCheckBox.Value = hCheckBox.Checked;
                }
                else
                    rCheckBox.Value = false;
            }

            dgvItems.RefreshEdit();

            _TotalCheckedCheckBoxes = hCheckBox.Checked ? _TotalCheckBoxes : 0;

            _IsHeaderCheckBoxClicked = false;

            ValidateRadioAll();

            BtnEnabledChange();
        }

        /// ================================================================================
        /// <summary>Validate header Checkbox</summary>
        /// ================================================================================
        private void ValidateHeaderCheckbox()
        {
            if (_TotalCheckedCheckBoxes == 0)
            {
                _HeaderCheckBox.CheckState = CheckState.Unchecked;
            }
            else if (_TotalCheckedCheckBoxes < _TotalCheckBoxes)
            {
                _HeaderCheckBox.CheckState = CheckState.Indeterminate;
            }
            else if (_TotalCheckedCheckBoxes == _TotalCheckBoxes)
            {
                _HeaderCheckBox.CheckState = CheckState.Checked;
            }
        }

        /// ================================================================================
        /// <summary> Init DataGridView</summary>
        /// ================================================================================
        private void InitDataGridView()
        {
            dgvItems.Rows.Clear();

            dgvItems.Columns[0].HeaderText = "";
            dgvItems.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_OBJECT_TYPE");

            _TotalCheckBoxes = 0;
            _TotalCheckedCheckBoxes = 0;
            foreach (var item in _TupleList)
            {
                if (item.Item2 == true)
                    _TotalCheckedCheckBoxes++;
                if (item.Item3 == true)
                    _TotalCheckBoxes++;

                dgvItems.Rows.Add(item.Item2, item.Item1);
            }

            ValidateHeaderCheckbox();
            ValidateRadioAll();
        }

        /// ================================================================================
        /// <summary>Set text for control </summary>
        /// ================================================================================
        private void SetData()
        {
            if(_iType == 0)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAM_OPTION");
            else if (_iType == 1)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_EACHONEBEAM_OPTIONS");
            else if (_iType == 2)
                this.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_BEAM_IMAGE_OPTIONS");


            this.grpBoxSelectType.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECT_TYPE");

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
        /// <summary> Read setting value</summary>
        /// ================================================================================
        private void ReadSetting()
        {
            if (_Settings == null || _Settings.Count != 9)
                return;

            var type = _Settings[0];
            var splitTypes = type.Split(';');

            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_BEAM_TYPE_ENG")) && IsEnabledBeamType)
            {
                IsBeamTypeChecked = true;
            }
            else
            {
                IsBeamTypeChecked = false;
            }
            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_CANTIBEAM_TYPE_ENG")) && IsEnabledCantiBeamType)
            {
                IsCantiBeamTypeChecked = true;
            }
            else
            {
                IsCantiBeamTypeChecked = false;
            }

            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_GIRDER_TYPE_ENG")) && IsEnabledGirderType)
            {
                IsGirderTypeChecked = true;
            }
            else
            {
                IsGirderTypeChecked = false;
            }
            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_TYPE_ENG")) && IsEnabledCantiGirderType)
            {
                IsCantiGirderTypeChecked = true;
            }
            else
            {
                IsCantiGirderTypeChecked = false;
            }

            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_FOUD_GIRDER_TYPE_ENG")) && IsEnabledFoundationGirdeType)
            {
                IsFoundationGirderTypeChecked = true;
            }
            else
            {
                IsFoundationGirderTypeChecked = false;
            }

            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_CANTI_FOUD_GIRDER_TYPE_ENG")) && IsEnabledCantiFoundationGirderType)
            {
                IsCantiFoundationGirderTypeChecked = true;
            }
            else
            {
                IsCantiFoundationGirderTypeChecked = false;
            }

            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_CANTI_FOUD_BEAM_TYPE_ENG")) && IsEnabledCantiFoundationBeamType)
            {
                IsCantiFoundationBeamTypeChecked = true;
            }
            else
            {
                IsCantiFoundationBeamTypeChecked = false;
            }
            if (splitTypes.ToList().Contains(_CmpAttribute.ResourceText("IDS_TXT_FOUD_BEAM_TYPE_ENG")) && IsEnabledFoundationBeamType)
            {
                IsFoundationBeamTypeChecked = true;
            }
            else
            {
                IsFoundationBeamTypeChecked = false;
            }

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
            if (IsGirderTypeChecked == false && IsCantiGirderTypeChecked == false && IsBeamTypeChecked == false && IsCantiBeamTypeChecked == false
                      && IsCantiFoundationBeamTypeChecked == false && IsCantiFoundationGirderTypeChecked == false && IsFoundationBeamTypeChecked == false && IsFoundationGirderTypeChecked == false)
            {
                if (IsEnabledFoundationBeamType)
                {
                    IsFoundationBeamTypeChecked = true;
                }
                if (IsEnabledCantiFoundationGirderType)
                {
                    IsCantiFoundationGirderTypeChecked = true;
                }
                if (IsEnabledFoundationGirdeType)
                {
                    IsFoundationGirderTypeChecked = true;
                }
                if (IsEnabledCantiBeamType)
                {
                    IsCantiBeamTypeChecked = true;
                }
                if (IsEnabledBeamType)
                {
                    IsBeamTypeChecked = true;
                }
                if (IsEnabledGirderType)
                {
                    IsGirderTypeChecked = true;
                }
                if (IsEnabledCantiGirderType)
                {
                    IsCantiGirderTypeChecked = true;
                }
                if (IsEnabledCantiFoundationBeamType)
                {
                    IsCantiFoundationBeamTypeChecked = true;
                }
            }
        }

        /// ================================================================================
        /// <summary>Get checked </summary>
        /// ================================================================================
        private void GetChecked()
        {
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                var cell = row.Cells[0] as DataGridViewCheckBoxCell;

                if ((bool)cell.ReadOnly == true)
                    continue;

                var text = row.Cells[1].Value;

                if (text == null)
                    continue;

                if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_GIRDER"))
                {
                    IsGirderTypeChecked = (bool)cell.Value;
                }
                else if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_FOUNDATION_GIRDER"))
                {
                    IsFoundationGirderTypeChecked = (bool)cell.Value;
                }
                else if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_GIRDER"))
                {
                    IsCantiGirderTypeChecked = (bool)cell.Value;
                }
                else if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_FOUNDATION_GIRDER"))
                {
                    IsCantiFoundationGirderTypeChecked = (bool)cell.Value;
                }
                else if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_BEAM"))
                {
                    IsBeamTypeChecked = (bool)cell.Value;
                }
                else if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_FOUNDATION_BEAM"))
                {
                    IsFoundationBeamTypeChecked = (bool)cell.Value;
                }
                else if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_BEAM"))
                {
                    IsCantiBeamTypeChecked = (bool)cell.Value;
                }
                else if (text.ToString() == _CmpAttribute.ResourceText("IDS_TXT_CHECKBOX_CANTI_FOUNDATION_BEAM"))
                {
                    IsCantiFoundationBeamTypeChecked = (bool)cell.Value;
                }
            }
        }

        /// ================================================================================
        /// <summary>Button clicked event </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            GetChecked();

            if (IsGirderTypeChecked == false && IsCantiGirderTypeChecked == false && IsBeamTypeChecked == false && IsCantiBeamTypeChecked == false
                && IsCantiFoundationBeamTypeChecked == false && IsCantiFoundationGirderTypeChecked == false && IsFoundationBeamTypeChecked == false && IsFoundationGirderTypeChecked == false)
            {
                MessageBox.Show("Please select at least one type to create the beam list.");
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
        /// <summary>Validate radio button</summary>
        /// ================================================================================
        private void ValidateRadioAll()
        {
            rdoSelection.Enabled = true;

            if (_TotalCheckedCheckBoxes > 1)
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
        /// <summary> Row post paint event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ================================================================================
        private void dgvItems_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                var item = _TupleList.ElementAt(e.RowIndex);

                DataGridViewCheckBoxCell chkCell = dgvItems.Rows[e.RowIndex].Cells[0] as DataGridViewCheckBoxCell;
                DataGridViewTextBoxCell textCell = dgvItems.Rows[e.RowIndex].Cells[1] as DataGridViewTextBoxCell;

                if (item.Item3 == false)
                {
                    chkCell.FlatStyle = FlatStyle.Flat;
                    chkCell.Style.ForeColor = Color.DarkGray;
                    chkCell.ReadOnly = true;
                    chkCell.ThreeState = true;

                    textCell.Style.ForeColor = Color.DarkGray;
                    textCell.ReadOnly = true;
                }
                else
                {
                    chkCell.ReadOnly = false;
                    textCell.ReadOnly = false;
                }
            }
        }

        /// ================================================================================
        /// <summary> Cell painting event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ================================================================================
        private void dgvItems_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 0)
            {
                //Get the column header cell bounds
                var oRectangle = this.dgvItems.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                var oPoint = new System.Drawing.Point();

                oPoint.X = oRectangle.Location.X + (oRectangle.Width - _HeaderCheckBox.Width) / 2 + 1;
                oPoint.Y = oRectangle.Location.Y + (oRectangle.Height - _HeaderCheckBox.Height) / 2 + 1;

                //Change the location of the CheckBox to make it stay on the header
                _HeaderCheckBox.Location = oPoint;
            }
        }

        /// ================================================================================
        /// <summary> Current cell dirty state changed event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ================================================================================
        private void dgvItems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvItems.CurrentCell is DataGridViewCheckBoxCell)
                dgvItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// ================================================================================
        /// <summary>Cell value changed event</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ================================================================================
        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_IsHeaderCheckBoxClicked && e.RowIndex >= 0)
            {
                RowCheckBoxClick((DataGridViewCheckBoxCell)dgvItems[e.ColumnIndex, e.RowIndex]);
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
            GetChecked();

            if (IsGirderTypeChecked == false && IsCantiGirderTypeChecked == false && IsBeamTypeChecked == false && IsCantiBeamTypeChecked == false
    && IsCantiFoundationBeamTypeChecked == false && IsCantiFoundationGirderTypeChecked == false && IsFoundationBeamTypeChecked == false && IsFoundationGirderTypeChecked == false)
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