using ADSK.JExtRAC.ParameterFilter.Components;
using ADSK.JExtRAC.ParameterFilter.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB ;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.ParameterFilter;
using Form = System.Windows.Forms.Form ;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    public partial class FormParameterFilter : Form
    {
        #region Member Variables

        /// <summary>UI document</summary>
        private Revit.UI.UIDocument _RvtUIDoc = null;

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>Elements</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>List of all element selected</summary>
        public List<ObjectElement> _ObjectElements;

        /// <summary>Current selected tab</summary>
        private int _CurrentIndexTab = 0;

        /// <summary>List of group parameter selected</summary>
        private List<ObjectSelectGroup> _LstObjectSelectedGroup = null;

        private int _checkBoxSize;

        #endregion Member Variables

        #region Constructor

        public FormParameterFilter(Revit.UI.UIDocument rvtUIDoc, RvtExtApp.Components.Attribute cmpAttribute, RvtExtApp.Components.Elements cmpElements, List<ObjectElement> objectElements)
        {
            this.InitializeComponent();
            AdjustGridSizesForDpi();

            this._CmpAttribute = cmpAttribute;
            this._CmpElements = cmpElements;
            this._ObjectElements = objectElements;
            this._RvtUIDoc = rvtUIDoc;
            this._LstObjectSelectedGroup = new List<ObjectSelectGroup>();

            // Set status button Previous and Next
            SetEnableNextAndPreviousButton();

            // Set label of control
            this.SetText();

            // Set data value
            this.SetData();
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
        private void FormLevelFilter_Load(object sender, EventArgs e)
        {
            // Load add data
            FormLevelFilterLoad();
        }

        public void FormLevelFilterLoad()
        {
            this.dgvCategory.Sort(dgvCategory.Columns["dgvCategory_Category"], System.ComponentModel.ListSortDirection.Ascending);
            this.dgvFamily.Sort(dgvFamily.Columns["dgvFamily_Family"], System.ComponentModel.ListSortDirection.Ascending);
            this.dgvFamilyType.Sort(dgvFamilyType.Columns["dataFamilyTypeB"], System.ComponentModel.ListSortDirection.Ascending);
            this.dgvParameter.Sort(dgvParameter.Columns["dgvParameter_Parameter"], System.ComponentModel.ListSortDirection.Ascending);

            // Set button select all and clear all category
            if (this.dgvCategory.Rows.Count < 1)
            {
                this.btnSelectAllCategory.Enabled = false;
                this.btnSelectClearCategory.Enabled = false;
            }
            else
            {
                this.btnSelectAllCategory.Enabled = false;
                this.btnSelectClearCategory.Enabled = true;
            }

            // Set label number selected
            this.lblCounterCategory.Text = Convert.ToString(dgvCategory.Rows.Count);
            int totalCount = 0;
            foreach (DataGridViewRow row in dgvCategory.Rows)
            {
                row.Cells["dgvCategory_CbkCategory"].Value = true;
                string valueNum = row.Cells["dgvCategory_Count"].Value.ToString();
                totalCount += int.Parse(valueNum);
            }
            this.lblObjCounterCategory.Text = Convert.ToString(totalCount);

            if (this.tabParameterFilter.SelectedTab == this.tabPageCategory)
                this.dgvCategory.Select();
            else if (this.tabParameterFilter.SelectedTab == this.tabPageFamily)
                this.dgvFamily.Select();
            else if (this.tabParameterFilter.SelectedTab == this.tabPageFamilyType)
                this.dgvFamilyType.Select();
            else
            {
                if (this.tabParameterFilter.SelectedTab != this.tabPageParameter)
                    return;
                this.dgvParameter.Select();
            }
        }

        /// ================================================================================
        /// <summary>Select all in category tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllCategory_Click(object sender, EventArgs e)
        {
            // Set checkbox = true
            foreach (DataGridViewRow dgvRow in dgvCategory.Rows)
                dgvRow.Cells["dgvCategory_CbkCategory"].Value = true;

            this.btnSelectAllCategory.Enabled = false;
            this.btnSelectClearCategory.Enabled = true;

            // Update counter
            int num = 0;
            int numberCheckbox = 0;
            foreach (DataGridViewRow row in dgvCategory.Rows)
            {
                if (row.Visible == false)
                    continue;

                string valueNum = row.Cells["dgvCategory_Count"].Value.ToString();
                num += int.Parse(valueNum);
                numberCheckbox++;
            }
            this.lblObjCounterCategory.Text = Convert.ToString(num);
            this.lblCounterCategory.Text = numberCheckbox.ToString();
        }

        /// ================================================================================
        /// <summary>Clear select category</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearCategory_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvCategory.Rows
                                where Convert.ToBoolean(r.Cells["dgvCategory_CbkCategory"].Value) == false
                                select r;
            if (unCheckedRows.Count() == dgvCategory.Rows.Count)
                return;
            // Set checkbox = false
            foreach (DataGridViewRow dgvRow in dgvCategory.Rows)
                dgvRow.Cells["dgvCategory_CbkCategory"].Value = false;

            this.btnSelectAllCategory.Enabled = true;
            this.btnSelectClearCategory.Enabled = false;

            // update counter
            this.lblCounterCategory.Text = "0";
            this.lblObjCounterCategory.Text = "0";
        }

        /// ================================================================================
        /// <summary>Preview button</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnPrewview_Click(object sender, EventArgs e)
        {
            // Set select item
            UpdateSelection();
        }

        /// ================================================================================
        /// <summary>Button OK</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Set select Item
            var retVal = UpdateSelection();
            if (retVal == false)
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// ================================================================================
        /// <summary>Button cancel</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// ================================================================================
        /// <summary>Select all in family tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllFamily_Click(object sender, EventArgs e)
        {
            // Set checkbox = true
            foreach (DataGridViewRow dgvRow in dgvFamily.Rows)
                dgvRow.Cells["dgvFamily_CbkFamily"].Value = true;

            this.btnSelectAllFamily.Enabled = false;
            this.btnSelectClearFamily.Enabled = true;

            // update counter
            this.lblTypeCounterFamily.Text = Convert.ToString(dgvFamily.Rows.Count);
            int num = 0;
            foreach (DataGridViewRow row in dgvFamily.Rows)
            {
                string valueNum = row.Cells["dgvFamily_Count"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterFamily.Text = Convert.ToString(num);
        }

        /// ================================================================================
        /// <summary>Clear select in family tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearFamily_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvFamily.Rows
                                where Convert.ToBoolean(r.Cells["dgvFamily_CbkFamily"].Value) == false
                                select r;
            if (unCheckedRows.Count() == dgvFamily.Rows.Count)
                return;
            // Set checkbox = false
            foreach (DataGridViewRow dgvRow in dgvFamily.Rows)
                dgvRow.Cells["dgvFamily_CbkFamily"].Value = false;

            this.btnSelectAllFamily.Enabled = true;
            this.btnSelectClearFamily.Enabled = false;

            // Update counter
            this.lblTypeCounterFamily.Text = "0";
            this.lblObjectCounterFamily.Text = "0";
        }

        /// ================================================================================
        /// <summary>Select all in family type tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllFamilyType_Click(object sender, EventArgs e)
        {
            // Set checkBox = true
            foreach (DataGridViewRow dgvRow in dgvFamilyType.Rows)
                dgvRow.Cells["dgvFamilyType_CbkFamilyType"].Value = true;

            this.btnSelectAllFamilyType.Enabled = false;
            this.btnSelectClearFamilyType.Enabled = true;

            // Update counter
            this.lblTypeCounterFamilyType.Text = Convert.ToString(dgvFamilyType.Rows.Count);
            int num = 0;
            foreach (DataGridViewRow row in dgvFamilyType.Rows)
            {
                string valueNum = row.Cells["dgvFamilyType_CountFamilyType"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterFamilyType.Text = Convert.ToString(num);
        }

        /// ================================================================================
        /// <summary>Clear select in family type tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearFamilyType_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvFamilyType.Rows
                                where Convert.ToBoolean(r.Cells["dgvFamilyType_CbkFamilyType"].Value) == false
                                select r;
            if (unCheckedRows.Count() == dgvFamilyType.Rows.Count)
                return;
            // Set checkBox = false
            foreach (DataGridViewRow dgvRow in dgvFamilyType.Rows)
                dgvRow.Cells["dgvFamilyType_CbkFamilyType"].Value = false;

            this.btnSelectAllFamilyType.Enabled = true;
            this.btnSelectClearFamilyType.Enabled = false;

            // Update counter
            this.lblTypeCounterFamilyType.Text = "0";
            this.lblObjectCounterFamilyType.Text = "0";
        }

        /// ================================================================================
        /// <summary>Select all in Parameter tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllParameter_Click(object sender, EventArgs e)
        {
            // Set checkBox = true
            foreach (DataGridViewRow dgvRow in dgvParameter.Rows)
                dgvRow.Cells["dgvParameter_CbkParameter"].Value = true;

            this.btnSelectAllParameter.Enabled = false;
            this.btnSelectClearParameter.Enabled = true;

            // Update counter
            SetCountParameter();
        }

        /// ================================================================================
        /// <summary>Clear select in parameter tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearParameter_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvParameter.Rows
                                where Convert.ToBoolean(r.Cells["dgvParameter_CbkParameter"].Value) == false
                                select r;

            if (unCheckedRows.Count() == dgvParameter.Rows.Count)
                return;
            // Set checkBox = false
            foreach (DataGridViewRow dgvRow in dgvParameter.Rows)
                dgvRow.Cells["dgvParameter_CbkParameter"].Value = false;

            this.btnSelectAllParameter.Enabled = true;
            this.btnSelectClearParameter.Enabled = false;

            // Update counter
            this.lblTypeCounterTypeParameter.Text = "0";
            this.lblObjectCounterTypeParameter.Text = "0";
        }

        /// ================================================================================
        /// <summary>Update form information</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            // Check value
            if (dgvCategory.Rows.Count == 0)
                return;

            if ((bool)dgvCategory.Rows[e.RowIndex].Cells["dgvCategory_CbkCategory"].Value)
                dgvCategory.Rows[e.RowIndex].Cells["dgvCategory_CbkCategory"].Value = false;
            else
                dgvCategory.Rows[e.RowIndex].Cells["dgvCategory_CbkCategory"].Value = true;

            // Set count and status button next and back
            SetCountAndStatusButtonTabCategory();
        }

        /// ================================================================================
        /// <summary>Merge cell</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/28 Created Applied Technology</history>
        /// ================================================================================
        private void dgvParameter_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvParameter.Rows[e.RowIndex].Cells["dgvParameter_CountParameter"].Value != null
                && dgvParameter.Rows[e.RowIndex].Cells["dgvParameter_CountParameter"].Value.ToString() != string.Empty)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.Single;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            }
            else
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            }

            if (e.RowIndex == 0)
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;

            if (e.RowIndex == dgvParameter.Rows.Count - 1)
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

            if (e.ColumnIndex >= 0 && dgvParameter.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                e.PaintBackground(e.ClipBounds, true);

                int size = _checkBoxSize;
                int x = e.CellBounds.Left + (e.CellBounds.Width - size) / 2;
                int y = e.CellBounds.Top + (e.CellBounds.Height - size) / 2;
                var boxRect = new System.Drawing.Rectangle(x, y, size, size);

                bool isChecked = false;
                if (e.Value != null && e.Value != DBNull.Value)
                    isChecked = Convert.ToBoolean(e.Value);

                ControlPaint.DrawCheckBox(
                    e.Graphics, boxRect,
                    isChecked
                        ? ButtonState.Checked | ButtonState.Flat
                        : ButtonState.Normal | ButtonState.Flat);

                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Update status button</summary>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        public void SetCountAndStatusButtonTabCategory()
        {
            var checkedRows = from DataGridViewRow r in dgvCategory.Rows
                              where Convert.ToBoolean(r.Cells["dgvCategory_CbkCategory"].Value) == true
                              select r;

            // Set enable and disable button clear all and select all
            if (checkedRows.Count() > 0 && checkedRows.Count() != dgvCategory.Rows.Count)
            {
                this.btnSelectAllCategory.Enabled = true;
                this.btnSelectClearCategory.Enabled = true;
            }
            if (checkedRows.Count() == 0)
            {
                this.btnSelectAllCategory.Enabled = true;
                this.btnSelectClearCategory.Enabled = false;
            }
            if (checkedRows.Count() == dgvCategory.Rows.Count)
            {
                this.btnSelectAllCategory.Enabled = false;
                this.btnSelectClearCategory.Enabled = true;
            }

            // Set count
            this.lblCounterCategory.Text = Convert.ToString(checkedRows.Count());
            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["dgvCategory_Count"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjCounterCategory.Text = Convert.ToString(num);
            dgvCategory.RefreshEdit();
        }

        /// ================================================================================
        /// <summary>Update form information</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvFamily_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            // Check value
            if (dgvFamily.Rows.Count == 0)
                return;
            if ((bool)dgvFamily.Rows[e.RowIndex].Cells["dgvFamily_CbkFamily"].Value)
                dgvFamily.Rows[e.RowIndex].Cells["dgvFamily_CbkFamily"].Value = false;
            else
                dgvFamily.Rows[e.RowIndex].Cells["dgvFamily_CbkFamily"].Value = true;

            // Set count and status button next and back
            SetCountAndStatusButtonInTabFamily();
        }

        public void SetCountAndStatusButtonInTabFamily()
        {
            var checkedRows = from DataGridViewRow r in dgvFamily.Rows
                              where Convert.ToBoolean(r.Cells["dgvFamily_CbkFamily"].Value) == true
                              select r;

            // Set enable and disable button clear all and select all
            if (checkedRows.Count() > 0 && checkedRows.Count() != dgvFamily.Rows.Count)
            {
                this.btnSelectAllFamily.Enabled = true;
                this.btnSelectClearFamily.Enabled = true;
            }
            if (checkedRows.Count() == 0)
            {
                this.btnSelectAllFamily.Enabled = true;
                this.btnSelectClearFamily.Enabled = false;
            }
            if (checkedRows.Count() == dgvFamily.Rows.Count)
            {
                this.btnSelectAllFamily.Enabled = false;
                this.btnSelectClearFamily.Enabled = true;
            }

            // Set count
            this.lblTypeCounterFamily.Text = Convert.ToString(checkedRows.Count());
            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["dgvFamily_Count"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterFamily.Text = Convert.ToString(num);

            dgvFamily.RefreshEdit();
        }

        /// ================================================================================
        /// <summary>Update form information</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvFamilyType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            // Check value
            if (dgvFamilyType.Rows.Count == 0)
                return;
            if ((bool)dgvFamilyType.Rows[e.RowIndex].Cells["dgvFamilyType_CbkFamilyType"].Value)
                dgvFamilyType.Rows[e.RowIndex].Cells["dgvFamilyType_CbkFamilyType"].Value = false;
            else
                dgvFamilyType.Rows[e.RowIndex].Cells["dgvFamilyType_CbkFamilyType"].Value = true;

            // Set count and status button next and back
            SetCountAndStatusButtonTabFamilyType();
        }

        public void SetCountAndStatusButtonTabFamilyType()
        {
            var checkedRows = from DataGridViewRow r in dgvFamilyType.Rows
                              where Convert.ToBoolean(r.Cells["dgvFamilyType_CbkFamilyType"].Value) == true
                              select r;

            // Set enable and disable button clear all and select all
            if (checkedRows.Count() > 0 && checkedRows.Count() != dgvFamilyType.Rows.Count)
            {
                this.btnSelectAllFamilyType.Enabled = true;
                this.btnSelectClearFamilyType.Enabled = true;
            }
            if (checkedRows.Count() == 0)
            {
                this.btnSelectAllFamilyType.Enabled = true;
                this.btnSelectClearFamilyType.Enabled = false;
            }
            if (checkedRows.Count() == dgvFamilyType.Rows.Count)
            {
                this.btnSelectAllFamilyType.Enabled = false;
                this.btnSelectClearFamilyType.Enabled = true;
            }

            // Set count
            this.lblTypeCounterFamilyType.Text = Convert.ToString(checkedRows.Count());
            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["dgvFamilyType_CountFamilyType"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterFamilyType.Text = Convert.ToString(num);

            dgvFamilyType.RefreshEdit();
        }

        /// ================================================================================
        /// <summary>Update form information</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvParameter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            // Min value and max value
            if (e.ColumnIndex == 4 || e.ColumnIndex == 5 || e.ColumnIndex == 7)
            {
                // Start edit
                dgvParameter.BeginEdit(true);
                return;
            }

            // Check value
            if (dgvParameter.Rows.Count == 0)
                return;

            if (dgvParameter.Rows[e.RowIndex].ReadOnly)
                return;

            if ((bool)dgvParameter.Rows[e.RowIndex].Cells["dgvParameter_CbkParameter"].Value)
                dgvParameter.Rows[e.RowIndex].Cells["dgvParameter_CbkParameter"].Value = false;
            else
                dgvParameter.Rows[e.RowIndex].Cells["dgvParameter_CbkParameter"].Value = true;

            SetEnableOrDisableButtonCheckParameter();

            // Set count
            SetCountParameter();

            dgvParameter.RefreshEdit();
        }

        /// ================================================================================
        /// <summary>Button setting parameter group is clicked</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnSettingParameterGroup_Click(object sender, EventArgs e)
        {
            // Get group parameter
            var lstGroupCurrent = _ObjectElements.SelectMany(x => x.ObjectLengths).GroupBy(x => x.ObjectGroupVal.ParameterGroupVal).Select(x => x.FirstOrDefault()).ToList();

            // Show form select group
            FormParameterGroup frm = new FormParameterGroup(_CmpAttribute, _LstObjectSelectedGroup);
            frm.ShowDialog();
            if (frm.DialogResult != DialogResult.OK)
                return;

            // Set data
            var lstStrSelectedGroup = frm._LstGroupAllProject.Where(x => x.IsSelected).Select(x => x.GroupTypeId).ToList();
            SetSelectedGroupParameter(lstStrSelectedGroup);

            // Update datagridview
            FilterCategory_Family_Type_Parameter();
        }

        /// ================================================================================
        /// <summary>User press next button</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_CurrentIndexTab >= tabParameterFilter.TabPages.Count - 1)
                return;

            _CurrentIndexTab += 1;
            tabParameterFilter.SelectedIndex = _CurrentIndexTab;

            if (tabParameterFilter.SelectedIndex == 1)          // Tab Family
                FilterCategory_Family();
            else if (tabParameterFilter.SelectedIndex == 2)     // Tab Type
                FilterCategory_Family_Type();
            else if (tabParameterFilter.SelectedIndex == 3)     // Tab Parameter
                FilterCategory_Family_Type_Parameter();

            // Set status button Previous and Next
            SetEnableNextAndPreviousButton();
        }

        /// ================================================================================
        /// <summary>User press Previous button</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_CurrentIndexTab <= 0)
                return;

            // Set active tab
            _CurrentIndexTab -= 1;
            tabParameterFilter.SelectedIndex = _CurrentIndexTab;

            // Set status button Previous and Next
            SetEnableNextAndPreviousButton();
        }

        /// ================================================================================
        /// <summary>Disable user change tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void tabParameterFilter_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == _CurrentIndexTab)
                return;

            e.Cancel = true;
        }

        #endregion Events

        #region Member Functions

        /// ================================================================================
        /// <summary>Filter tab category data</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void FilterCategory_Family()
        {
            // Clear data
            dgvFamily.Rows.Clear();

            foreach (DataGridViewRow row in dgvCategory.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                bool isChecked = (bool)row.Cells[0].Value;
                if (isChecked == false)
                    continue;

                List<ObjectElement> lstObjElement = row.Tag as List<ObjectElement>;
                if (lstObjElement == null)
                    continue;

                var gorupByFamily = lstObjElement.GroupBy(x => x.FamilyNameElement);
                foreach (var objectFamily in gorupByFamily)
                {
                    if (string.IsNullOrEmpty(objectFamily.Key))
                        continue;

                    // Add value
                    var indexAdded = dgvFamily.Rows.Add(true, row.Cells[1].Value.ToString(), objectFamily.Key, objectFamily.Count().ToString());
                    dgvFamily.Rows[indexAdded].Tag = objectFamily.ToList();
                }
            }

            // Set count
            this.lblTypeCounterFamily.Text = Convert.ToString(dgvFamily.Rows.Count);
            int num2 = 0;
            foreach (DataGridViewRow row in dgvFamily.Rows)
            {
                row.Cells["dgvFamily_CbkFamily"].Value = true;
                string valueNum = row.Cells["dgvFamily_Count"].Value.ToString();
                num2 += int.Parse(valueNum);
            }
            this.lblObjectCounterFamily.Text = Convert.ToString(num2);

            // Set button select all and clear all family
            if (this.dgvFamily.Rows.Count < 1)
            {
                this.btnSelectAllFamily.Enabled = false;
                this.btnSelectClearFamily.Enabled = false;
            }
            else
            {
                this.btnSelectAllFamily.Enabled = false;
                this.btnSelectClearFamily.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Filter tab Family type</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void FilterCategory_Family_Type()
        {
            // Clear data
            dgvFamilyType.Rows.Clear();

            foreach (DataGridViewRow row in dgvFamily.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                bool isChecked = (bool)row.Cells[0].Value;
                if (isChecked == false)
                    continue;

                List<ObjectElement> lstObjElement = row.Tag as List<ObjectElement>;
                if (lstObjElement == null)
                    continue;

                var gorupByType = lstObjElement.GroupBy(x => x.TypeNameElement);
                foreach (var objectFamilyType in gorupByType)
                {
                    if (string.IsNullOrEmpty(objectFamilyType.Key))
                        continue;

                    var indexAdded = dgvFamilyType.Rows.Add(true, row.Cells[1].Value.ToString(), objectFamilyType.Key, objectFamilyType.Count().ToString());
                    dgvFamilyType.Rows[indexAdded].Tag = objectFamilyType.ToList();
                }
            }

            // Set count
            this.lblTypeCounterFamilyType.Text = Convert.ToString(dgvFamilyType.Rows.Count);
            int totalCount = 0;
            foreach (DataGridViewRow row in dgvFamilyType.Rows)
            {
                row.Cells["dgvFamilyType_CbkFamilyType"].Value = true;
                string valueNum = row.Cells["dgvFamilyType_CountFamilyType"].Value.ToString();
                totalCount += int.Parse(valueNum);
            }
            this.lblObjectCounterFamilyType.Text = Convert.ToString(totalCount);

            // Set button select all and clear all family type
            if (this.dgvFamilyType.Rows.Count < 1)
            {
                this.btnSelectAllFamilyType.Enabled = false;
                this.btnSelectClearFamilyType.Enabled = false;
            }
            else
            {
                this.btnSelectAllFamilyType.Enabled = false;
                this.btnSelectClearFamilyType.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Filter tab Parameter data</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public void FilterCategory_Family_Type_Parameter()
        {
            // Clear data
            dgvParameter.Rows.Clear();

            // Get all group enum
            if (_LstObjectSelectedGroup.Count == 0)
                _LstObjectSelectedGroup = _CmpElements.GetAllGroupTypeElement_(_ObjectElements);

            foreach (DataGridViewRow row in dgvFamilyType.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                bool isChecked = (bool)row.Cells[0].Value;
                if (isChecked == false)
                    continue;

                List<ObjectElement> lstObjElement = row.Tag as List<ObjectElement>;
                if (lstObjElement == null)
                    continue;

                // Order columns
                lstObjElement = lstObjElement.OrderBy(x => x.CategoriesElement)
                                                .ThenBy(x => x.FamilyNameElement)
                                                .ThenBy(x => x.TypeNameElement).ToList();
                lstObjElement.ForEach(x =>
                {
                    x.ObjectLengths = x.ObjectLengths.OrderBy(y => y.NameParameterLength).ToList();
                });

                // Group by parameter length name
                var query = lstObjElement.SelectMany(x => x.ObjectLengths, (ele, objPara) => new { ele.ElementCurrent, objPara });
                var groupByName = query.GroupBy(x => x.objPara.NameParameterLength);

                // Same element type
                string guidSameElement = Guid.NewGuid().ToString();

                int indexRow = -1;
                List<int> lstIndexRowAdded = new List<int>();
                foreach (var objElement in groupByName)
                {
                    int indexAdded = -1;
                    indexRow++;

                    try
                    {
                        if (objElement.Count() == 0)  // Don't have length parameter
                        {
                            indexAdded = dgvParameter.Rows.Add(true, row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), "", "", "", "", "", "",
                                                                lstObjElement.Count().ToString());
                            dgvParameter.Rows[indexAdded].Tag = lstObjElement.SelectMany(x => x.ObjectLengths).ToList();

                            dgvParameter.Rows[indexAdded].Cells[0].Tag = guidSameElement;

                            break;
                        }
                        else
                        {
                            var groupValue = objElement.GroupBy(x => x.objPara.LengthVal);

                            if (groupValue.Count() == 1)        // Only 1 value
                            {
                                int count = objElement.Select(x => x.ElementCurrent.Id).GroupBy(x => x).Select(y => y.First()).Count();

                                indexAdded = dgvParameter.Rows.Add(true, row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), objElement.FirstOrDefault().objPara.NameParameterLength,
                                    "", "", "", "", "", count.ToString());

                                // Add tag
                                List<ObjectLengthParameter> lstObjPara = new List<ObjectLengthParameter>();
                                foreach (var item in objElement)
                                {
                                    if (lstObjPara.Any(x => x.ElementCurrent.Id == item.objPara.ElementCurrent.Id) == false)
                                        lstObjPara.Add(item.objPara);
                                }

                                dgvParameter.Rows[indexAdded].Tag = lstObjPara;

                                dgvParameter.Rows[indexAdded].Cells[0].Tag = guidSameElement;

                                continue;
                            }
                            else        // Get min max value
                            {
                                var minVal = groupValue.Min(x => x.Key);
                                var maxVal = groupValue.Max(x => x.Key);

                                indexAdded = dgvParameter.Rows.Add(true, row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString(), objElement.FirstOrDefault().objPara.NameParameterLength
                                    , "", "", "", "", "", objElement.Count().ToString());

                                List<ObjectLengthParameter> lstObjPara = new List<ObjectLengthParameter>();
                                foreach (var item in objElement)
                                    lstObjPara.Add(item.objPara);

                                // Add tag object
                                dgvParameter.Rows[indexAdded].Tag = lstObjPara;

                                dgvParameter.Rows[indexAdded].Cells[0].Tag = guidSameElement;

                                // add tag min and max Value
                                dgvParameter.Rows[indexAdded].Cells["dgvParameter_Min"].Tag = minVal;
                                dgvParameter.Rows[indexAdded].Cells["dgvParameter_Max"].Tag = maxVal;
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        if (indexAdded >= 0)
                        {
                            // Set visible
                            dgvParameter.Rows[indexAdded].Visible = objElement.FirstOrDefault().objPara.ObjectGroupVal.IsSelected;

                            // Element no have parameter length
                            if (objElement.FirstOrDefault().objPara.ObjectGroupVal.GroupTypeId == new Revit.DB.ForgeTypeId(string.Empty))
                                dgvParameter.Rows[indexAdded].Visible = false;

                            // Hide element is not in group
                            foreach (var objElementData in objElement)
                            {
                                if (_LstObjectSelectedGroup.Any(x => x.GroupTypeId == objElementData.objPara.ObjectGroupVal.GroupTypeId))
                                    continue;

                                dgvParameter.Rows[indexAdded].Visible = false;
                            }

                            lstIndexRowAdded.Add(indexAdded);
                        }
                    }
                }

                // Set merge count with same type
                bool isVisibleFirst = false;
                foreach (int indexRowAdd in lstIndexRowAdded)
                {
                    if (dgvParameter.Rows[indexRowAdd].Visible == false)
                        continue;

                    if (isVisibleFirst == false)
                    {
                        int count = groupByName.FirstOrDefault().Select(x => x.ElementCurrent.Id).GroupBy(x => x).Select(y => y.First()).Count();
                        dgvParameter.Rows[indexRowAdd].Cells["dgvParameter_CountParameter"].Value = count;
                    }
                    else
                        dgvParameter.Rows[indexRowAdd].Cells["dgvParameter_CountParameter"].Value = string.Empty;

                    isVisibleFirst = true;
                }
            }

            // Set group null to datagridview
            SetGroupNull(_LstObjectSelectedGroup);

            // Set count and status button next and back
            SetCountParameter();

            SetEnableOrDisableButtonCheckParameter();
        }

        /// ================================================================================
        /// <summary>Set enable or disable button check parameter</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2022/01/27 Created Applied Technology</history>
        /// ================================================================================
        ///

        private void SetEnableOrDisableButtonCheckParameter()
        {
            var checkedRows = from DataGridViewRow r in dgvParameter.Rows
                              where (Convert.ToBoolean(r.Cells["dgvParameter_CbkParameter"].Value) == true) && (r.Visible == true)
                              select r;
            var totalRow = from DataGridViewRow r in dgvParameter.Rows
                           where (r.Visible == true)
                           select r;

            // Set enable and disable button clear all and select all
            if (checkedRows.Count() > 0 && checkedRows.Count() != totalRow.Count())
            {
                this.btnSelectAllParameter.Enabled = true;
                this.btnSelectClearParameter.Enabled = true;
            }
            else if (checkedRows.Count() == 0 && totalRow.Count() == 0)
            {
                this.btnSelectAllParameter.Enabled = false;
                this.btnSelectClearParameter.Enabled = false;
            }
            else if (checkedRows.Count() == 0)
            {
                this.btnSelectAllParameter.Enabled = true;
                this.btnSelectClearParameter.Enabled = false;
            }
            else if (checkedRows.Count() == totalRow.Count())
            {
                this.btnSelectAllParameter.Enabled = false;
                this.btnSelectClearParameter.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Get data of parameter has value (Data row has number element)</summary>
        ///
        /// <param name="numberRow" >Total row count</param>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        ///
        private Dictionary<List<int>, int> GetDicDataParameterHasValue(out int numberRow)
        {
            // Get split data
            Dictionary<List<int>, int> dicdataRow = new Dictionary<List<int>, int>();
            List<int> lstIndexPreview = new List<int>();

            // Set count
            numberRow = 0;

            foreach (DataGridViewRow row in dgvParameter.Rows)
            {
                if (row.Visible == false)
                    continue;

                string valueNum = row.Cells["dgvParameter_CountParameter"].Value.ToString();
                if (string.IsNullOrEmpty(valueNum) == false)
                {
                    List<int> lstIndex = new List<int>();
                    lstIndexPreview = lstIndex;
                    dicdataRow.Add(lstIndex, int.Parse(valueNum));
                }

                if ((bool)row.Cells["dgvParameter_CbkParameter"].Value == true)
                    numberRow++;

                lstIndexPreview.Add(row.Index);
            }

            return dicdataRow;
        }

        /// ================================================================================
        /// <summary>Set select element</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        ///
        public void SetCountParameter()
        {
            // Get split data
            Dictionary<List<int>, int> dicdataRow = GetDicDataParameterHasValue(out int numberRow);

            // Set count
            int totalCount = 0;

            foreach (var itemData in dicdataRow)
            {
                int indexRowHasValue = itemData.Key.FirstOrDefault();

                foreach (var indexRow in itemData.Key)
                {
                    var row = dgvParameter.Rows[indexRow];

                    if (row.Visible == false)
                        continue;

                    if ((bool)row.Cells["dgvParameter_CbkParameter"].Value == true)
                    {
                        string valueNum = dgvParameter.Rows[indexRowHasValue].Cells["dgvParameter_CountParameter"].Value.ToString();
                        totalCount += int.Parse(valueNum);

                        break;
                    }
                }
            }

            this.lblTypeCounterTypeParameter.Text = numberRow.ToString();
            this.lblObjectCounterTypeParameter.Text = totalCount.ToString();
        }

        /// ================================================================================
        /// <summary>Select element in tab category, family, family type</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SelectElementByDGV(DataGridView dgv)
        {
            List<Revit.DB.ElementId> lstElementSelect = new List<Revit.DB.ElementId>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                if ((bool)row.Cells[0].Value == false)
                    continue;

                if (row.Tag == null)
                    continue;

                List<ObjectElement> lstObjElement = row.Tag as List<ObjectElement>;
                if (lstObjElement == null)
                    continue;

                lstElementSelect.AddRange(lstObjElement.Select(x => x.ElementCurrent.Id));
            }

            // Select element
            _RvtUIDoc.Selection.SetElementIds(lstElementSelect);
            _RvtUIDoc.RefreshActiveView();
        }

        /// ================================================================================
        /// <summary>Update select element when user press OK or preview</summary>
        ///
        /// <returns>True or false</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private bool UpdateSelection()
        {
            bool bRet = true;
            try
            {
                switch (this.tabParameterFilter.SelectedIndex)
                {
                    case 0:
                        SelectElementByDGV(dgvCategory);            // Tab category
                        break;

                    case 1:
                        SelectElementByDGV(dgvFamily);              // Tab family
                        break;

                    case 2:
                        SelectElementByDGV(dgvFamilyType);          // Tab Family type
                        break;

                    case 3:
                        bRet = SelectElementParameterByDGV(dgvParameter);  // Tab parameter
                        break;
                }
            }
            catch (Exception ex)
            {
                bRet = false;
                string errMsg = ex.Message;
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_COMMAND"), _CmpAttribute.ResourceText("IDS_ERR_ERROR"));
            }

            return bRet;
        }

        /// ================================================================================
        /// <summary>Filter element and select by tab parameter</summary>
        ///
        /// <param name="dgv" >Datagridview parameter</param>
        ///
        /// <returns>True or false</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private bool SelectElementParameterByDGV(DataGridView dgv)
        {
            if (ValidateValueUserInput() == false)
                return false;

            // Init data
            List<ObjectLengthParameter> lstElementSelect = new List<ObjectLengthParameter>();
            List<Revit.DB.ElementId> lstSelected = new List<Revit.DB.ElementId>();
            Dictionary<string, List<ObjectLengthParameter>> dicSameTypeSelect = new Dictionary<string, List<ObjectLengthParameter>>();

            // ProgressBar
            ProgressBarThread progressBarThread = new ProgressBarThread(false, true);
            progressBarThread.SetData(_CmpAttribute.ResourceText("IDS_TXT_PROGESSBAR"), 0);

            // Show ProgressBar
            progressBarThread.ShowDialog();

            int dgvCurrentCountVisible = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Visible == false)
                    continue;

                dgvCurrentCountVisible++;
            }
            progressBarThread.SetData(dgvCurrentCountVisible, 0);

            int count = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                // Set default count
                row.Cells["dgvParameter_CountParameter"].Tag = 0;

                count++;

                if (row.Visible == false)
                    continue;

                if (row.Cells[0].Value == null)
                    continue;

                if ((bool)row.Cells[0].Value == false)
                    continue;

                if (row.Tag == null)
                    continue;

                List<ObjectLengthParameter> lstObjElement = row.Tag as List<ObjectLengthParameter>;
                if (lstObjElement == null)
                    continue;

                // Get datagridview value
                var prValueDgv = row.Cells["dgvParameter_Value"].Value;
                var prMinDgv = row.Cells["dgvParameter_Min"].Value;
                var prMaxDgv = row.Cells["dgvParameter_Max"].Value;

                // Set data user input to class data
                foreach (var objElement in lstObjElement)
                {
                    objElement.prValueDgv = prValueDgv;
                    objElement.prMinDgv = prMinDgv;
                    objElement.prMaxDgv = prMaxDgv;
                }

                // Filter by user input value min max
                if (cbkSelectConnect.Checked)
                {
                    // Get all element connected
                    var lstElementNeedSelect = _CmpElements.GetSelectElementConnect(lstObjElement);

                    // Get real element select
                    UnionSelectWithSameType(row, ref dicSameTypeSelect, lstElementNeedSelect);

                    // Update count to datagridview
                    row.Cells["dgvParameter_CountParameter"].Tag = lstElementNeedSelect.Count;
                }
                else
                {
                    var lstObjElementNeedSelect = _CmpElements.FilterParameterByUserInput(lstObjElement, prValueDgv, prMinDgv, prMaxDgv);
                    if (lstObjElementNeedSelect == null)
                        return false;

                    // Get real element select
                    UnionSelectWithSameType(row, ref dicSameTypeSelect, lstObjElementNeedSelect);

                    // Update count to datagridview
                    row.Cells["dgvParameter_CountParameter"].Tag = lstObjElementNeedSelect.Count;
                }

                // Set data ProgressBar
                progressBarThread.SetData(count);
            }
            progressBarThread.Close();

            UpdateCount(dgvParameter, dicSameTypeSelect);

            foreach (var keyPair in dicSameTypeSelect)
                lstElementSelect.AddRange(keyPair.Value);

            lstSelected.AddRange(lstElementSelect.Select(x => x.ElementCurrent.Id).ToList());

            // Set select element
            _RvtUIDoc.Selection.SetElementIds(lstSelected);
            _RvtUIDoc.RefreshActiveView();

            // Set count
            SetCountParameter();

            return true;
        }

        /// ================================================================================
        /// <summary>Set single count of single row parameter</summary>
        ///
        /// <param name="dgvParameter" >Current datagridview row</param>
        /// <param name="dicData" >Selected element</param>
        ///
        /// <returns></returns>
        ///
        /// <history>2022/01/22 Created Applied Technology</history>
        /// ================================================================================
        private void UpdateCount(DataGridView dgvParameter, Dictionary<string, List<ObjectLengthParameter>> dicData)
        {
            // Get datagridview row has count
            var dicDataRowHasValue = GetDicDataParameterHasValue(out int numberRow);

            foreach (var keypair in dicDataRowHasValue)
            {
                List<string> lstValue = new List<string>();
                // Find max in datagridview
                foreach (DataGridViewRow row in dgvParameter.Rows)
                {
                    if (keypair.Key.Contains(row.Index) == false)
                        continue;

                    var objValue = row.Cells[0].Tag;
                    if (objValue == null)
                        continue;
                    if (string.IsNullOrEmpty(objValue.ToString()))
                        continue;

                    lstValue.Add(objValue.ToString());
                }

                lstValue = lstValue.GroupBy(x => x).Select(y => y.First()).ToList();
                string keyFind = lstValue.FirstOrDefault();
                if (keyFind == null)
                    continue;

                if (dicData.ContainsKey(keyFind))
                {
                    for (int i = 0; i < keypair.Key.Count; i++)
                    {
                        if (i == 0)
                            dgvParameter.Rows[keypair.Key[i]].Cells["dgvParameter_CountParameter"].Value = dicData[keyFind].Count;
                        else
                            dgvParameter.Rows[keypair.Key[i]].Cells["dgvParameter_CountParameter"].Value = string.Empty;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>Union 2 list with same type for get new element select</summary>
        ///
        /// <param name="currentRow" >Current datagridview row</param>
        /// <param name="dicData" >Dictionary element select with same type</param>
        /// <param name="listCurrent" >List element current check</param>
        ///
        /// <returns>True or false</returns>
        ///
        /// <history>2021/01/16 Created Applied Technology</history>
        /// ================================================================================
        private bool UnionSelectWithSameType(DataGridViewRow currentRow, ref Dictionary<string, List<ObjectLengthParameter>> dicData, List<ObjectLengthParameter> listCurrent)
        {
            if (currentRow.Cells[0].Tag == null)
                return false;

            string typeKeyData = currentRow.Cells[0].Tag.ToString();

            if (dicData.ContainsKey(typeKeyData) == false)
            {
                dicData.Add(typeKeyData, listCurrent);
                return true;
            }

            var lstObjectOld = dicData[typeKeyData];

            List<ObjectLengthParameter> retVal = new List<ObjectLengthParameter>();
            foreach (var objCurrentElement in listCurrent)
            {
                if (lstObjectOld.Any(x => x.ElementCurrent.Id == objCurrentElement.ElementCurrent.Id))
                    retVal.Add(objCurrentElement);
            }

            dicData[typeKeyData] = retVal;

            return true;
        }

        /// ================================================================================
        /// <summary>Check user input data</summary>
        ///
        /// <returns>True or false</returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private bool ValidateValueUserInput()
        {
            bool isOk = true;

            foreach (DataGridViewRow row in dgvParameter.Rows)
            {
                string errorMess = string.Empty;

                // Clear error mess
                row.Cells["dgvParameter_Error"].ErrorText = string.Empty;

                if (row.Visible == false)
                    continue;

                if (row.Cells[0].Value == null)
                    continue;

                // Get datagridview value
                var prValueDgv = row.Cells["dgvParameter_Value"].Value;
                var prMinDgv = row.Cells["dgvParameter_Min"].Value;
                var prMaxDgv = row.Cells["dgvParameter_Max"].Value;

                // Check White space
                if (CheckWhiteSpace(prValueDgv))
                    prValueDgv = string.Empty;
                if (CheckWhiteSpace(prMinDgv))
                    prMinDgv = string.Empty;
                if (CheckWhiteSpace(prMaxDgv))
                    prMaxDgv = string.Empty;

                bool isInputValue = false, isInputMin = false, isInputMax = false;
                isInputValue = (prValueDgv != null && string.IsNullOrEmpty(prValueDgv.ToString()) == false);
                isInputMin = (prMinDgv != null && string.IsNullOrEmpty(prMinDgv.ToString()) == false);
                isInputMax = (prMaxDgv != null && string.IsNullOrEmpty(prMaxDgv.ToString()) == false);

                // User didn't input value
                if (isInputValue == false && isInputMin == false && isInputMax == false)
                    continue;

                // Check value
                errorMess += ValidateSingleValue(prValueDgv, out int valuePr, out bool isErrorValue);
                // Check min value
                errorMess += ValidateSingleValue(prMinDgv, out int minPr, out bool isErrorMin);
                // Check max value
                errorMess += ValidateSingleValue(prMaxDgv, out int maxPr, out bool isErrorMax);

                // User input all value
                if (isInputValue && isInputMin && isInputMax)
                {
                    if (isErrorValue == false && isErrorMin == false && isErrorMax == false)
                    {
                        errorMess += CheckValueAndMin(valuePr, minPr);
                        errorMess += CheckValueAndMax(valuePr, maxPr);
                        errorMess += CheckMinAndMax(minPr, maxPr);
                    }

                    if (isErrorValue && isErrorMin == false && isErrorMax == false)
                        errorMess += CheckMinAndMax(minPr, maxPr);
                    else if (isErrorValue == false && isErrorMin == false && isErrorMax)
                        errorMess += CheckValueAndMin(valuePr, minPr);
                    else if (isErrorValue == false && isErrorMin && isErrorMax == false)
                        errorMess += CheckValueAndMax(valuePr, maxPr);
                }
                else if (isInputValue && isInputMin && isInputMax == false)        // Input value and min
                {
                    if (isErrorValue == false && isErrorMin == false)
                        errorMess += CheckValueAndMin(valuePr, minPr);
                }
                else if (isInputValue && isInputMin == false && isInputMax)         // Input value and max
                {
                    if (isErrorValue == false && isErrorMax == false)
                        errorMess += CheckValueAndMax(valuePr, maxPr);
                }
                else if (isInputValue == false && isInputMin && isInputMax)         // Input min and max
                {
                    if (isErrorMin == false && isErrorMax == false)
                        errorMess += CheckMinAndMax(minPr, maxPr);
                }
                else
                {
                    // input value and min
                    if (isInputValue && isInputMin && isInputMax == false)
                        errorMess += CheckValueAndMin(valuePr, minPr);

                    // input value and max
                    if (isInputValue && isInputMin == false && isInputMax)
                        errorMess += CheckValueAndMax(valuePr, maxPr);

                    // Input min and max
                    if (isInputValue == false && isInputMin && isInputMax)
                        errorMess += CheckMinAndMax(minPr, maxPr);
                }

                if (string.IsNullOrEmpty(errorMess) == false)
                {
                    isOk = false;
                    row.Cells["dgvParameter_Error"].ErrorText = errorMess;
                }
            }

            return isOk;
        }

        /// ================================================================================
        /// <summary>Check user input Min</summary>
        ///
        /// <param name="valuePr" >Current value</param>
        /// <param name="minPr" >Min value</param>
        ///
        /// <returns>Error mess</returns>
        ///
        /// <history>2022/01/26 Created Applied Technology</history>
        /// ================================================================================
        private string CheckValueAndMin(int valuePr, int minPr)
        {
            if (valuePr < minPr)
                return _CmpAttribute.ResourceText("IDS_ERR_LESSMIN") + "\n";
            else
                return string.Empty;
        }

        /// ================================================================================
        /// <summary>Check user input Max</summary>
        ///
        /// <param name="valuePr" >Current value</param>
        /// <param name="maxPr" >Max value</param>
        ///
        /// <returns>Error mess</returns>
        ///
        /// <history>2022/01/26 Created Applied Technology</history>
        /// ================================================================================
        private string CheckValueAndMax(int valuePr, int maxPr)
        {
            if (valuePr > maxPr)
                return _CmpAttribute.ResourceText("IDS_ERR_GREATERMAX") + "\n";
            else
                return string.Empty;
        }

        /// ================================================================================
        /// <summary>Check user input Min and Max</summary>
        ///
        /// <param name="minPr" >Min value</param>
        /// <param name="maxPr" >Min value</param>
        ///
        /// <returns>Error mess</returns>
        ///
        /// <history>2022/01/26 Created Applied Technology</history>
        /// ================================================================================
        private string CheckMinAndMax(int minPr, int maxPr)
        {
            if (minPr > maxPr)
                return _CmpAttribute.ResourceText("IDS_ERR_MINMAXVALUE") + "\n";
            else
                return string.Empty;
        }

        /// ================================================================================
        /// <summary>Check user input with white space</summary>
        ///
        /// <param name="objStr" >Object current value</param>
        ///
        /// <returns>True or false</returns>
        ///
        /// <history>2021/01/16 Created Applied Technology</history>
        /// ================================================================================
        private bool CheckWhiteSpace(object objStr)
        {
            if (objStr == null)
                return false;

            string strVal = objStr.ToString();

            return strVal.Length > 0 && strVal.Trim().Length == 0;
        }

        /// ================================================================================
        /// <summary>Check error User input number and greate than 0</summary>
        ///
        /// <param name="objStr" >Object current value</param>
        /// <param name="nVal" >Out value</param>
        ///
        /// <returns>Error mess</returns>
        ///
        /// <history>2021/01/16 Created Applied Technology</history>
        /// ================================================================================
        private string ValidateSingleValue(object valueObj, out int nVal, out bool isHasError)
        {
            string errorMess = string.Empty;
            nVal = 0;
            isHasError = true;

            if (valueObj == null || string.IsNullOrEmpty(valueObj.ToString()))
                return errorMess;

            string valueData = valueObj.ToString();

            // Check number
            if (int.TryParse(valueData, out nVal) == false)
                errorMess += _CmpAttribute.ResourceText("IDS_ERR_NUMBERONLY") + "\n";

            // Check >0
            if (nVal < 0)
                errorMess = errorMess + _CmpAttribute.ResourceText("IDS_ERR_GREATER0") + "\n";

            if (string.IsNullOrEmpty(errorMess))
                isHasError = false;

            return errorMess;
        }

        /// ================================================================================
        /// <summary>Set default group when element is don't have group length</summary>
        ///
        /// <param name="lstGroupAllProject" >List of all group</param>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetGroupNull(List<ObjectSelectGroup> lstGroupAllProject)
        {
            // Get selected group
            var lstGroupSelectedInProject = lstGroupAllProject.Where(x => x.IsSelected).ToList();

            List<ObjectLengthParameter> lstAllObjectLength = new List<ObjectLengthParameter>();

            // Get group need to add new to datagridview
            foreach (DataGridViewRow row in dgvParameter.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                bool isChecked = (bool)row.Cells[0].Value;
                if (isChecked == false)
                    continue;

                List<ObjectLengthParameter> lstObjElement = row.Tag as List<ObjectLengthParameter>;
                if (lstObjElement == null)
                    continue;

                lstAllObjectLength.AddRange(lstObjElement);
            }

            var objLengthElement = lstAllObjectLength.GroupBy(x => x.ElementCurrent.Id).ToList();

            // Find
            List<Revit.DB.Element> lstElementNeedAddNull = new List<Revit.DB.Element>();
            foreach (var objElement in objLengthElement)
            {
                if (objElement.Count() == 0)
                    continue;

                bool isFound = false;
                foreach (var objItem in objElement)
                {
                    if (objItem.ObjectGroupVal.GroupTypeId == new Revit.DB.ForgeTypeId(string.Empty))
                    {
                        isFound = false;
                        break;
                    }

                    if (lstGroupSelectedInProject.Any(x => x.GroupTypeId == objItem.ObjectGroupVal.GroupTypeId))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (isFound == false)
                    lstElementNeedAddNull.Add(objElement.FirstOrDefault().ElementCurrent);
            }

            // Get data of element
            var lstObjectElement = _CmpElements.GetDataElement(lstElementNeedAddNull);

            var groupElement = lstObjectElement.GroupBy(x => (x.CategoriesElement, x.TypeNameElement)).ToList();
            foreach (var objectElement in groupElement)
            {
                var indexAdded = dgvParameter.Rows.Add(true, objectElement.Key.CategoriesElement, objectElement.Key.TypeNameElement, "", "", "", "", "", "", objectElement.Count());
                dgvParameter.Rows[indexAdded].Cells["dgvParameter_Value"].ReadOnly = true;
                dgvParameter.Rows[indexAdded].Cells["dgvParameter_Min"].ReadOnly = true;
                dgvParameter.Rows[indexAdded].Cells["dgvParameter_Max"].ReadOnly = true;

                // Get object length
                // Add to tag
                List<ObjectLengthParameter> lstTemp = new List<ObjectLengthParameter>();
                foreach (var objCurrent in objectElement)
                {
                    objCurrent.GetLengthAndGroupParamter();
                    lstTemp.Add(objCurrent.ObjectLengths.FirstOrDefault());
                }

                dgvParameter.Rows[indexAdded].Tag = lstTemp;
                dgvParameter.Rows[indexAdded].Cells[0].Tag = Guid.NewGuid().ToString();
            }
        }

        /// ================================================================================
        /// <summary>Set status of select group</summary>
        ///
        /// <param name="lstSelectedGroup" >List of all group is selected</param>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetSelectedGroupParameter(List<ForgeTypeId> lstSelectedGroup)
        {
            foreach (var objElement in _ObjectElements)
            {
                foreach (var objLength in objElement.ObjectLengths)
                {
                    if (lstSelectedGroup.Any(x => x == objLength.ObjectGroupVal.GroupTypeId))
                        objLength.ObjectGroupVal.IsSelected = true;
                    else
                        objLength.ObjectGroupVal.IsSelected = false;
                }
            }
        }

        /// ================================================================================
        /// <summary>Set status of button previous and next</summary>
        ///
        /// <returns></returns>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        private void SetEnableNextAndPreviousButton()
        {
            int indexCurrentTab = tabParameterFilter.SelectedIndex;
            if (indexCurrentTab == 0)           // Tab category
            {
                btnPrevious.Enabled = false;
                btnNext.Enabled = true;
            }
            else if (indexCurrentTab == 3)      // Tab parameter
            {
                btnPrevious.Enabled = true;
                btnNext.Enabled = false;
            }
            else                                // Tab family and family type
            {
                btnPrevious.Enabled = true;
                btnNext.Enabled = true;
            }
        }

        /// ================================================================================
        /// <summary>Set text</summary>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_FILTERFORM");
            this.btnSettingParameterGroup.Text = _CmpAttribute.ResourceText("IDS_TXT_SETTINGPARAMETERGROUP");
            this.btnNext.Text = _CmpAttribute.ResourceText("IDS_TXT_NEXT");
            this.btnPrevious.Text = _CmpAttribute.ResourceText("IDS_TXT_PREVIOUS");
            this.cbkSelectConnect.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCONNECT");
            this.tabPageCategory.Text = _CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            this.lblCountTypeCategory.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            this.lblCountObjCate.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            this.lblCounterCategory.Text = "0";
            this.lblObjCounterCategory.Text = "0";
            this.btnSelectAllCategory.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            this.btnSelectClearCategory.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            this.btnPrewview.Text = _CmpAttribute.ResourceText("IDS_TXT_PNTCHECK");
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            this.tabPageFamily.Text = _CmpAttribute.ResourceText("IDS_TXT_FAMILY");
            this.lblCountTypeFamily.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            this.lblCountObjectFamily.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            this.lblTypeCounterFamily.Text = "0";
            this.lblObjectCounterFamily.Text = "0";
            this.btnSelectAllFamily.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            this.btnSelectClearFamily.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            this.tabPageFamilyType.Text = _CmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            this.lblCountTypeFamilyType.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            this.lblCountObjectFamilyType.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            this.lblTypeCounterFamilyType.Text = "0";
            this.lblObjectCounterFamilyType.Text = "0";
            this.btnSelectAllFamilyType.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            this.btnSelectClearFamilyType.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            this.tabPageParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
            this.lblCountTypeParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            this.lblCountObjectTypeParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            this.lblTypeCounterTypeParameter.Text = "0";
            this.lblObjectCounterTypeParameter.Text = "0";
            this.btnSelectAllParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            this.btnSelectClearParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");

            //Set header text
            dgvCategory.Columns[0].HeaderText = string.Empty;
            dgvCategory.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            dgvCategory.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");

            dgvFamily.Columns[0].HeaderText = string.Empty;
            dgvFamily.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            dgvFamily.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_FAMILY");
            dgvFamily.Columns[3].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");

            dgvFamilyType.Columns[0].HeaderText = string.Empty;
            dgvFamilyType.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            dgvFamilyType.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            dgvFamilyType.Columns[3].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");

            dgvParameter.Columns[0].HeaderText = string.Empty;
            dgvParameter.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            dgvParameter.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            dgvParameter.Columns[3].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
            dgvParameter.Columns[4].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_VALUEPARAMETER");
            dgvParameter.Columns[5].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_VALUEPARAMETERMIN");
            dgvParameter.Columns[7].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_VALUEPARAMETERMAX");
            dgvParameter.Columns[9].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");
        }

        /// ================================================================================
        /// <summary>Set Data</summary>
        ///
        /// <history>2021/11/11 Created Applied Technology</history>
        /// ================================================================================
        public void SetData()
        {
            // Order by category
            var lstCategory = _ObjectElements.GroupBy(x => x.CategoriesElement).ToList();

            // Add to datagridview
            foreach (var objCategory in lstCategory)
            {
                var indexAdded = dgvCategory.Rows.Add(false, objCategory.Key, objCategory.Count().ToString());
                dgvCategory.Rows[indexAdded].Tag = objCategory.ToList();
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

                var grids = new[] { dgvCategory, dgvFamily, dgvFamilyType, dgvParameter };
                foreach (var dgv in grids)
                {
                    dgv.RowTemplate.Height = rowHeight;
                    foreach (DataGridViewRow row in dgv.Rows)
                        row.Height = rowHeight;
                }

                dgvCategory.CellPainting += DgvCheckBox_CellPainting;
                dgvFamily.CellPainting += DgvCheckBox_CellPainting;
                dgvFamilyType.CellPainting += DgvCheckBox_CellPainting;

                int cbkColWidth = _checkBoxSize + 16;
                dgvCategory.Columns["dgvCategory_CbkCategory"].Width = cbkColWidth;
                dgvCategory.Columns["dgvCategory_CbkCategory"].MinimumWidth = cbkColWidth;
                dgvFamily.Columns["dgvFamily_CbkFamily"].Width = cbkColWidth;
                dgvFamily.Columns["dgvFamily_CbkFamily"].MinimumWidth = cbkColWidth;
                dgvFamilyType.Columns["dgvFamilyType_CbkFamilyType"].Width = cbkColWidth;
                dgvFamilyType.Columns["dgvFamilyType_CbkFamilyType"].MinimumWidth = cbkColWidth;
                dgvParameter.Columns["dgvParameter_CbkParameter"].Width = cbkColWidth;
                dgvParameter.Columns["dgvParameter_CbkParameter"].MinimumWidth = cbkColWidth;

                int countWidth = (int)Math.Ceiling(g.MeasureString("Number", this.tabParameterFilter.Font).Width) + 20;
                dgvCategory.Columns["dgvCategory_Count"].Width = countWidth;
                dgvFamily.Columns["dgvFamily_Count"].Width = countWidth;
                dgvFamilyType.Columns["dgvFamilyType_CountFamilyType"].Width = countWidth;
                dgvParameter.Columns["dgvParameter_CountParameter"].Width = countWidth;

                cellFont.Dispose();
            }
        }

        private void DgvCheckBox_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var dgv = (DataGridView)sender;
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;
            if (!(dgv.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            e.PaintBackground(e.ClipBounds, true);

            int size = _checkBoxSize;
            int x = e.CellBounds.Left + (e.CellBounds.Width - size) / 2;
            int y = e.CellBounds.Top + (e.CellBounds.Height - size) / 2;
            var boxRect = new System.Drawing.Rectangle(x, y, size, size);

            bool isChecked = false;
            if (e.Value != null && e.Value != DBNull.Value)
                isChecked = Convert.ToBoolean(e.Value);

            ControlPaint.DrawCheckBox(
                e.Graphics, boxRect,
                isChecked
                    ? ButtonState.Checked | ButtonState.Flat
                    : ButtonState.Normal | ButtonState.Flat);

            e.Handled = true;
        }

        #endregion Member Functions
    }
}