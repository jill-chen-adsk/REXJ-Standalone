using ADSK.JExtRAC.ParameterFilter.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.ParameterFilter;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    public partial class FormParameterGroup : Form
    {
        #region Member Variables

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>List group all project</summary>
        public List<ObjectSelectGroup> _LstGroupAllProject = null;

        private int _checkBoxSize;

        #endregion Member Variables

        #region Constructor

        public FormParameterGroup(RvtExtApp.Components.Attribute cmpAttribute, List<ObjectSelectGroup> lstGroupAllProject)
        {
            InitializeComponent();
            AdjustGridSizesForDpi();

            this._CmpAttribute = cmpAttribute;
            this._LstGroupAllProject = lstGroupAllProject;
        }

        #endregion Constructor

        #region Events

        /// ================================================================================
        /// <summary>Load form</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void FormParameterGroup_Load(object sender, EventArgs e)
        {
            // Set label
            this.SetText();

            // Set data
            SetData();

            // Set disable selection
            SetDisableSelection();

            // Set button uncheck and check all
            SetEnableOrDisableBtnCheck();
        }

        /// ================================================================================
        /// <summary>Set visible border</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvSetting_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var cellValue = dgvSetting.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
            {
                e.PaintBackground(e.ClipBounds, true);
                e.Handled = true;
                return;
            }

            if (dgvSetting.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                e.PaintBackground(e.ClipBounds, true);

                int size = _checkBoxSize;
                int x = e.CellBounds.Left + (e.CellBounds.Width - size) / 2;
                int y = e.CellBounds.Top + (e.CellBounds.Height - size) / 2;
                var boxRect = new Rectangle(x, y, size, size);

                bool isChecked = false;
                if (cellValue != DBNull.Value)
                    isChecked = Convert.ToBoolean(cellValue);

                ControlPaint.DrawCheckBox(
                    e.Graphics, boxRect,
                    isChecked
                        ? ButtonState.Checked | ButtonState.Flat
                        : ButtonState.Normal | ButtonState.Flat);

                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Button unCheck all is clicked</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnUnCheck_Click(object sender, EventArgs e)
        {
            // Set uncheck all
            SetStatusCheckbox(false);
        }

        /// ================================================================================
        /// <summary>Button Check all is clicked</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            // Set check all
            SetStatusCheckbox(true);
        }

        /// ================================================================================
        /// <summary>User is click in datagridview</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                dgvSetting.Rows[e.RowIndex].Cells["dgvCbk1"].Value = !(bool)dgvSetting.Rows[e.RowIndex].Cells["dgvCbk1"].Value;
            else
            {
                if (dgvSetting.Rows[e.RowIndex].Cells["dgvCbk2"].Value == null || string.IsNullOrEmpty(dgvSetting.Rows[e.RowIndex].Cells["dgvCbk2"].Value.ToString()))
                    return;

                if (dgvSetting.Rows[e.RowIndex].Cells["dgvGroupName2"].Value == null || string.IsNullOrEmpty(dgvSetting.Rows[e.RowIndex].Cells["dgvGroupName2"].Value.ToString()))
                    return;

                dgvSetting.Rows[e.RowIndex].Cells["dgvCbk2"].Value = !(bool)dgvSetting.Rows[e.RowIndex].Cells["dgvCbk2"].Value;
            }

            // Parser Data
            ParserData();

            // Set button uncheck and check all
            SetEnableOrDisableBtnCheck();
        }

        /// ================================================================================
        /// <summary>Button clicked</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnApply_Click(object sender, EventArgs e)
        {
            // Update data
            ParserData();
        }

        #endregion Events

        #region Member Functions

        /// ================================================================================
        /// <summary>Set disable user select</summary>
        ///
        /// <returns>List of object group element</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetDisableSelection()
        {
            dgvSetting.DefaultCellStyle.SelectionBackColor = dgvSetting.DefaultCellStyle.BackColor;
            dgvSetting.DefaultCellStyle.SelectionForeColor = dgvSetting.DefaultCellStyle.ForeColor;
        }

        /// ================================================================================
        /// <summary>Set text</summary>
        ///
        /// <history>2021/11/18 Created Applied Technology</history>
        /// ================================================================================
        private void SetText()
        {
            // Set text to form
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_PARAMETERGROUP");
            this.gboxSettingParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_DGVPARAMETERGROUP");
            this.btnCheckAll.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            this.btnUnCheck.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            this.btnApply.Text = _CmpAttribute.ResourceText("IDS_TXT_PNTCHECK");
        }

        /// ================================================================================
        /// <summary>Set data</summary>
        ///
        /// <history>2021/11/18 Created Applied Technology</history>
        /// ================================================================================
        private void SetData()
        {
            // Set all data to datagridview
            for (int i = 0; i < _LstGroupAllProject.Count; i = i + 2)
            {
                ObjectSelectGroup currentGroup = _LstGroupAllProject[i];
                ObjectSelectGroup nextGroup = null;

                if (i == _LstGroupAllProject.Count - 1 && i % 2 == 0)
                    nextGroup = null;
                else
                    nextGroup = _LstGroupAllProject[i + 1];

                if (nextGroup != null)
                {
                    int indexAdded = dgvSetting.Rows.Add(currentGroup.IsSelected, currentGroup.ParameterGroupVal,
                                        nextGroup.IsSelected, nextGroup.ParameterGroupVal);

                    dgvSetting.Rows[indexAdded].Cells[0].Tag = currentGroup;
                    dgvSetting.Rows[indexAdded].Cells[2].Tag = nextGroup;
                }
                else
                {
                    int indexAdded = dgvSetting.Rows.Add(currentGroup.IsSelected, currentGroup.ParameterGroupVal, null, null);
                    dgvSetting.Rows[indexAdded].Cells[0].Tag = currentGroup;

                    // Disable last checkbox
                    dgvSetting.Rows[indexAdded].Cells[2].ReadOnly = true;
                }
            }
        }

        /// ================================================================================
        /// <summary>Set status of checkBox</summary>
        ///
        /// <param name="status" >Status</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetStatusCheckbox(bool status)
        {
            foreach (DataGridViewRow row in dgvSetting.Rows)
            {
                if (row.Cells["dgvGroupName1"].Value != null && string.IsNullOrEmpty(row.Cells["dgvGroupName1"].Value.ToString()) == false)
                    row.Cells["dgvCbk1"].Value = status;

                if (row.Cells["dgvGroupName2"].Value != null && string.IsNullOrEmpty(row.Cells["dgvGroupName2"].Value.ToString()) == false)
                    row.Cells["dgvCbk2"].Value = status;
            }

            // Parser Data
            ParserData();

            // Set button uncheck and check all
            SetEnableOrDisableBtnCheck();
        }

        /// ================================================================================
        /// <summary>Parser data to data class object</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void ParserData()
        {
            // Clear all data
            _LstGroupAllProject.Clear();

            // Get data user input
            foreach (DataGridViewRow row in dgvSetting.Rows)
            {
                ObjectSelectGroup obj1 = row.Cells[0].Tag as ObjectSelectGroup;
                ObjectSelectGroup obj2 = row.Cells[2].Tag as ObjectSelectGroup;
                if (obj1 != null)
                {
                    // Value 1
                    obj1.IsSelected = (bool)row.Cells["dgvCbk1"].Value;
                    _LstGroupAllProject.Add(obj1);
                }

                if (obj2 != null)
                {
                    // Value 2
                    if (row.Cells["dgvGroupName2"].Value != null && string.IsNullOrEmpty(row.Cells["dgvGroupName2"].Value.ToString()) == false)
                        obj2.IsSelected = (bool)row.Cells["dgvCbk2"].Value;
                    _LstGroupAllProject.Add(obj2);
                }
            }
        }

        /// ================================================================================
        /// <summary>Set status of button</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetEnableOrDisableBtnCheck()
        {
            if (_LstGroupAllProject.Where(x => x.IsSelected).Count() == 0)
            {
                btnCheckAll.Enabled = true;
                btnUnCheck.Enabled = false;
            }
            else if (_LstGroupAllProject.Where(x => x.IsSelected).Count() == _LstGroupAllProject.Count)
            {
                btnCheckAll.Enabled = false;
                btnUnCheck.Enabled = true;
            }
            else
            {
                btnCheckAll.Enabled = true;
                btnUnCheck.Enabled = true;
            }
        }

        private void AdjustGridSizesForDpi()
        {
            using (var g = this.CreateGraphics())
            {
                var cellFont = new Font("Segoe UI", 8.25F);
                int textHeight = (int)Math.Ceiling(g.MeasureString("Wg", cellFont).Height);
                int rowHeight = textHeight + 8;
                _checkBoxSize = Math.Max(13, textHeight);

                dgvSetting.RowTemplate.Height = rowHeight;
                foreach (DataGridViewRow row in dgvSetting.Rows)
                    row.Height = rowHeight;

                int cbkColWidth = _checkBoxSize + 16;
                dgvSetting.Columns["dgvCbk1"].Width = cbkColWidth;
                dgvSetting.Columns["dgvCbk2"].Width = cbkColWidth;

                cellFont.Dispose();
            }
        }

        #endregion Member Functions
    }
}