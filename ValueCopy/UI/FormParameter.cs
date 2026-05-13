using ADSK.JExtRAC.ValueCopy.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using RvtExtApp = ADSK.JExtRAC.ValueCopy;

namespace ADSK.JExtRAC.ValueCopy.UI
{
    /// ================================================================================
    /// <summary>FormParameter</summary>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public partial class FormParameter : Form
    {
        // Member variable

        #region Member Variables

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>ObjectElement</summary>
        private ObjectElement _ObjElement = null;

        /// <summary>List ObjectIndexGroup </summary>
        private List<ObjectIndexGroup> _IndexGroups = null;

        /// <summary>Index row</summary>
        private int indexRow = -1;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute"  >Parameter</param>
        /// <param name="objElement"  >ObjectElement</param>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        public FormParameter(RvtExtApp.Components.Attribute cmpAttribute, ObjectElement objElement)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _ObjElement = objElement;
            _IndexGroups = new List<ObjectIndexGroup>();
        }

        #endregion Constructor

        // Member Functions

        #region Member Function

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private void InitText()
        {
            // Set icon
            this.Icon = Resources.Image.IDI_SUBS_ICON;

            this.Text = _CmpAttribute.ResourceText("IDS_TXT_COPYFORM");
            btApply.Text = _CmpAttribute.ResourceText("IDS_TXT_APPLY");
            btCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history>2022/01/10 Created Applied Technology</history>
        /// ================================================================================
        private void InitData()
        {
            if (_ObjElement == null)
                return;

            // Group by group
            var groups = _ObjElement.ObjectParameterData.GroupBy(x => x.ParameterGroupName).OrderBy(x => x.Key);
            foreach (var group in groups)
            {
                int index = dgvPropetives.Rows.Add(CheckState.Unchecked, false, string.Empty, string.Empty);
                dgvPropetives.Rows[index].DefaultCellStyle.BackColor = Color.LightGray;
                dgvPropetives.Rows[index].Cells[1] = new DataGridViewTextBoxCell { Value = string.Empty };

                dgvPropetives.Rows[index].Tag = group.FirstOrDefault().ElementIdGroup;

                ObjectIndexGroup objIndexGroup = new ObjectIndexGroup();
                objIndexGroup.ParameterGroupName = group.Key;
                objIndexGroup.IndexOnDatagridview = index;
                _IndexGroups.Add(objIndexGroup);

                foreach (var objParameter in group)
                {
                    index = dgvPropetives.Rows.Add(CheckState.Unchecked, false, objParameter.NameParameter, objParameter.ParameterValue);
                    dgvPropetives.Rows[index].Cells[0] = new DataGridViewTextBoxCell { Value = string.Empty };

                    dgvPropetives.Rows[index].Tag = objParameter;
                }
            }

            // Set enable checkbox
            foreach (DataGridViewRow row in dgvPropetives.Rows)
            {
                var dgvCbkGroup = row.Cells["dgvCbkGroup"] as DataGridViewCheckBoxCell;
                if (dgvCbkGroup != null)
                    dgvCbkGroup.ReadOnly = false;

                var dgvCbkParameter = row.Cells["dgvCbkParameter"] as DataGridViewCheckBoxCell;
                if (dgvCbkParameter != null)
                    dgvCbkParameter.ReadOnly = false;
            }
        }

        #endregion Member Function

        // Events

        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormParameter control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void FormParameter_Load(object sender, EventArgs e)
        {
            // Set text to dialog
            InitText();

            // Set data to dialog
            InitData();

            // Set event custom DatagridView
            this.dgvPropetives.CellPainting += new DataGridViewCellPaintingEventHandler(dgvPropetives_CellPainting);
            this.dgvPropetives.Paint += new PaintEventHandler(dgvPropetives_Paint);
            this.dgvPropetives.Scroll += new ScrollEventHandler(dgvPropetives_Scroll);
            this.dgvPropetives.ColumnWidthChanged += new DataGridViewColumnEventHandler(dgvPropetives_ColumnWidthChanged);
        }

        /// ================================================================================
        /// <summary>Handles the ColumnWidthChanged event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewColumnEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            foreach (var item in _IndexGroups)
            {
                Rectangle rtHeader = this.dgvPropetives.DisplayRectangle;
                rtHeader.Height = this.dgvPropetives.Rows[item.IndexOnDatagridview].Height;
                dgvPropetives.Invalidate(rtHeader);
            }
        }

        /// ================================================================================
        /// <summary>Handles the Scroll event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="ScrollEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_Scroll(object sender, ScrollEventArgs e)
        {
            foreach (var item in _IndexGroups)
            {
                Rectangle rtHeader = this.dgvPropetives.DisplayRectangle;
                rtHeader.Height = this.dgvPropetives.Rows[item.IndexOnDatagridview].Height;
                dgvPropetives.Invalidate(rtHeader);
            }
        }

        /// ================================================================================
        /// <summary>Handles the Paint event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_Paint(object sender, PaintEventArgs e)
        {
            foreach (var item in _IndexGroups)
            {
                Rectangle r1 = dgvPropetives.GetCellDisplayRectangle(1, item.IndexOnDatagridview, true);
                int w2 = dgvPropetives.GetCellDisplayRectangle(2, item.IndexOnDatagridview, true).Width;
                int w3 = dgvPropetives.GetCellDisplayRectangle(3, item.IndexOnDatagridview, true).Width;
                r1.Width = r1.Width + w2 + w3;

                Rectangle rec = dgvPropetives.DisplayRectangle;
                if (rec.IntersectsWith(r1) == false)
                    continue;

                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), r1);

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawString(item.ParameterGroupName, dgvPropetives.ColumnHeadersDefaultCellStyle.Font, new SolidBrush(dgvPropetives.ColumnHeadersDefaultCellStyle.ForeColor), r1, format);
            }
        }

        /// ================================================================================
        /// <summary>Handles the Cell Painting event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewCellPaintingEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (_IndexGroups.Any(x => x.IndexOnDatagridview == e.RowIndex) && e.ColumnIndex > -1)
            {
                Rectangle r2 = e.CellBounds;
                r2.Y += e.CellBounds.Height / 2;
                r2.Height = e.CellBounds.Height / 2;
                e.PaintBackground(r2, true);
                e.PaintContent(r2);
                e.Handled = true;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Cell Click event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            indexRow = e.RowIndex;
        }

        /// ================================================================================
        /// <summary>Handles the CellEndEdit event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
        }

        /// ================================================================================
        /// <summary>Handles the CellValueChanged event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (indexRow < 0)
                return;

            if (e.ColumnIndex == 0 && _IndexGroups.Any(x => x.IndexOnDatagridview == indexRow))                  // Set all inside
            {
                ((DataGridViewCheckBoxCell)dgvPropetives.Rows[e.RowIndex].Cells[e.ColumnIndex]).ThreeState = false;

                indexRow = -1;

                DataGridViewCheckBoxCell currentCheck = dgvPropetives.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
                if (currentCheck == null)
                    return;

                for (int i = e.RowIndex + 1; i < dgvPropetives.Rows.Count; i++)
                {
                    DataGridViewCheckBoxCell checkboxInside = dgvPropetives.Rows[i].Cells["dgvCbkParameter"] as DataGridViewCheckBoxCell;
                    if (checkboxInside == null)
                        break;

                    if (currentCheck.Value.Equals(CheckState.Checked))
                        checkboxInside.Value = true;
                    else if (currentCheck.Value.Equals(CheckState.Unchecked))
                        checkboxInside.Value = false;
                    else if (currentCheck.Value.Equals(true))
                        checkboxInside.Value = true;
                    else
                        checkboxInside.Value = false;
                }
            }
            else if (e.ColumnIndex == 1)
            {
                DataGridViewCheckBoxCell currentCheck = dgvPropetives.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
                if (currentCheck == null)
                    return;

                List<bool> lstStatus = new List<bool>();
                for (int i = e.RowIndex; i < dgvPropetives.Rows.Count; i++)
                {
                    DataGridViewCheckBoxCell checkboxInside = dgvPropetives.Rows[i].Cells["dgvCbkParameter"] as DataGridViewCheckBoxCell;
                    if (checkboxInside == null)
                        break;

                    lstStatus.Add((bool)checkboxInside.Value);
                }

                int indexRowHost = 0;
                for (int i = e.RowIndex; i >= 0; i--)
                {
                    DataGridViewCheckBoxCell checkboxInside = dgvPropetives.Rows[i].Cells["dgvCbkParameter"] as DataGridViewCheckBoxCell;
                    if (checkboxInside == null)
                    {
                        indexRowHost = i;
                        break;
                    }

                    lstStatus.Add((bool)checkboxInside.Value);
                }

                DataGridViewCheckBoxCell checkboxHost = dgvPropetives.Rows[indexRowHost].Cells["dgvCbkGroup"] as DataGridViewCheckBoxCell;
                if (checkboxHost == null)
                    return;

                checkboxHost.ThreeState = true;

                if (lstStatus.Any(x => x == true) && lstStatus.Any(x => x == false))
                    checkboxHost.Value = CheckState.Indeterminate;
                else if (lstStatus.Any(x => x == true))
                    checkboxHost.Value = CheckState.Checked;
                else
                    checkboxHost.Value = CheckState.Unchecked;
            }
        }

        /// ================================================================================
        /// <summary>Handles the CurrentCellDirtyStateChanged event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Commit edit change
            dgvPropetives.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btApply control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void btApply_Click(object sender, EventArgs e)
        {
            bool isHasCopy = false;

            // Set data checkbox to object
            foreach (DataGridViewRow row in dgvPropetives.Rows)
            {
                var objParameter = row.Tag as ObjectParameter;
                if (objParameter == null)
                    continue;

                DataGridViewCheckBoxCell checkboxInside = row.Cells["dgvCbkParameter"] as DataGridViewCheckBoxCell;
                if (checkboxInside == null)
                    continue;

                // Status of checkbox
                if (checkboxInside.Value.Equals(CheckState.Checked))
                {
                    objParameter.IsCopy = true;
                    isHasCopy = true;
                }
                else if (checkboxInside.Value.Equals(CheckState.Unchecked))
                    objParameter.IsCopy = false;
                else if (checkboxInside.Value.Equals(true))
                {
                    isHasCopy = true;
                    objParameter.IsCopy = true;
                }
                else
                    objParameter.IsCopy = false;
            }

            if (isHasCopy == false)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_INPUTVALUE"));
                this.DialogResult = DialogResult.None;
            }
        }

        /// ================================================================================
        /// <summary>Handles the CellDoubleClick event of the dgvPropetives control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="DataGridViewCellEventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2022/01/11 Created Applied Technology</history>
        /// ================================================================================
        private void dgvPropetives_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            indexRow = e.RowIndex;
        }

        #endregion Events
    }
}