using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.LevelFilter;
using System.Collections;
using System.Reflection;

namespace ADSK.JExtRAC.LevelFilter.UI
{
    public partial class FormLevelFilter : Form
    {
        // Member variable

        #region Memeber Variables

        /// <summary>DBDocument</summary>
        private Revit.DB.Document _RvtDbDoc;

        /// <summary>UIDocument</summary>
        private Revit.UI.UIDocument _RvtUIDoc;

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>element select</summary>
        private IList<Revit.DB.Element> _ElemSelect;

        /// <summary>part select</summary>
        private IList<Revit.DB.Part> _PartSelect;

        /// <summary>number tab</summary>
        private int _TabNum;

        /// <summary>selected id</summary>
        private IList<int> _SelectIdAry;

        /// <summary>dictionary category</summary>
        private Dictionary<Revit.DB.ElementId, IList<Revit.DB.ElementId>> _DicCat;

        /// <summary>dictionary family</summary>
        private Dictionary<string, IList<Revit.DB.ElementId>> _DicFam;

        /// <summary>dictionary family type</summary>
        private Dictionary<string, IList<Revit.DB.ElementId>> _DicFamType;

        /// <summary>dictionary part</summary>
        private Dictionary<Revit.DB.ElementId, IList<Revit.DB.ElementId>> _DicPart;

        /// <summary>Current selected</summary>
        private IList<Revit.DB.Element> _SelElems;

        /// <summary>Rule FilterElement</summary>
        private Dictionary<Revit.DB.ElementId, IList<Revit.DB.ElementId>> _DicFilter;

        #endregion Memeber Variables

        public
        IList<int> SelectIdAry
        {
            get
            {
                return this._SelectIdAry;
            }
        }

        public
        string SelectTabNum
        {
            get
            {
                return this._TabNum.ToString();
            }
        }

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="rvtUIDoc"  >UIdocument</param>
        /// <param name="cmpAttribute"  >Parameter</param>
        /// <param name="elemSet"  >list element</param>
        /// <param name="partSet"  >list part</param>
        /// <param name="dicCat"  >dictionary cat</param>
        /// <param name="dicFam"  >dictionary fam</param>
        /// <param name="dicFamType"  >dictionary if list fam type</param>
        /// <param name="dicPart"  >dictionary of list part</param>
        /// <param name="dicFilter"  >dictionary of list rule filter</param>
        /// <param name="tabNum"  >tab number</param>
        ///
        /// <history><p>2018/10/23 Created Applied Technology</p>
        ///         <p> 2021/11/10 Modified Applied Technology</p></history>
        /// ================================================================================
        public FormLevelFilter(Revit.UI.UIDocument rvtUIDoc, RvtExtApp.Components.Attribute cmpAttribute, IList<Revit.DB.Element> elemSet,
            IList<Revit.DB.Part> partSet, Dictionary<Revit.DB.ElementId, IList<Revit.DB.ElementId>> dicCat, Dictionary<string, IList<Revit.DB.ElementId>> dicFam,
            Dictionary<string, IList<Revit.DB.ElementId>> dicFamType, Dictionary<Revit.DB.ElementId, IList<Revit.DB.ElementId>> dicPart, Dictionary<Revit.DB.ElementId, IList<Revit.DB.ElementId>> dicFilter, IList<Revit.DB.Element> selElems, string tabNum)
        {
            this.InitializeComponent();
            AdjustGridSizesForDpi();

            this._CmpAttribute = cmpAttribute;
            this._ElemSelect = elemSet;
            this._PartSelect = partSet;
            this._SelectIdAry = (IList<int>)new List<int>();
            this._DicCat = dicCat;
            this._DicFam = dicFam;
            this._DicFamType = dicFamType;
            this._DicPart = dicPart;
            this._DicFilter = dicFilter;
            this._RvtDbDoc = rvtUIDoc.Document;
            _RvtUIDoc = rvtUIDoc;
            _SelElems = selElems;

            if (int.TryParse(tabNum, out this._TabNum))
            {
                if (this._TabNum < 0)
                    this._TabNum = 0;
            }
            else
                this._TabNum = 0;

            this.SetText();
            this.SetData();
        }

        #endregion Constructor

        // Member function

        #region Member Functions

        private int _checkBoxSize;

        private void AdjustGridSizesForDpi()
        {
            using (var g = this.CreateGraphics())
            {
                var cellFont = new System.Drawing.Font("Segoe UI", 8.25F);
                int textHeight = (int)Math.Ceiling(g.MeasureString("Wg", cellFont).Height);
                int rowHeight = textHeight + 8;
                _checkBoxSize = Math.Max(13, textHeight);

                var grids = new[] { dgvCategory, dgvFamily, dgvFamilyType, dgvParts, dgvFilters };
                foreach (var dgv in grids)
                {
                    dgv.RowTemplate.Height = rowHeight;
                    foreach (DataGridViewRow row in dgv.Rows)
                        row.Height = rowHeight;
                    dgv.CellPainting += DgvCheckBox_CellPainting;
                }

                int cbkColWidth = _checkBoxSize + 16;
                dgvCategory.Columns["cbkCategory"].Width = cbkColWidth;
                dgvCategory.Columns["cbkCategory"].MinimumWidth = cbkColWidth;
                dgvFamily.Columns["cbkFamily"].Width = cbkColWidth;
                dgvFamily.Columns["cbkFamily"].MinimumWidth = cbkColWidth;
                dgvFamilyType.Columns["cbkFamilyType"].Width = cbkColWidth;
                dgvFamilyType.Columns["cbkFamilyType"].MinimumWidth = cbkColWidth;
                dgvParts.Columns["cbkParts"].Width = cbkColWidth;
                dgvParts.Columns["cbkParts"].MinimumWidth = cbkColWidth;
                dgvFilters.Columns["cbkFilters"].Width = cbkColWidth;
                dgvFilters.Columns["cbkFilters"].MinimumWidth = cbkColWidth;

                int countWidth = (int)Math.Ceiling(g.MeasureString("Number", this.tabLevelFilter.Font).Width) + 20;
                dgvCategory.Columns["countCategory"].Width = countWidth;
                dgvFamily.Columns["countFamily"].Width = countWidth;
                dgvFamilyType.Columns["countFamilyType"].Width = countWidth;
                dgvParts.Columns["countParts"].Width = countWidth;
                dgvFilters.Columns["countFilters"].Width = countWidth;

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

            System.Windows.Forms.ControlPaint.DrawCheckBox(
                e.Graphics, boxRect,
                isChecked
                    ? System.Windows.Forms.ButtonState.Checked | System.Windows.Forms.ButtonState.Flat
                    : System.Windows.Forms.ButtonState.Normal | System.Windows.Forms.ButtonState.Flat);

            e.Handled = true;
        }

        /// ================================================================================
        /// <summary>Set text</summary>
        ///
        /// <history><p>2018/10/23 Created Applied Technology</p>
        ///         <p>2021/11/10 Modified Applied Technology</p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_FILTERFORM") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
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
            this.tabPageParts.Text = _CmpAttribute.ResourceText("IDS_TXT_PARTS");
            this.lblCountTypeParts.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            this.lblCountObjectParts.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            this.lblTypeCounterParts.Text = "0";
            this.lblObjectCounterParts.Text = "0";
            this.btnSelectAllParts.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            this.btnSelectClearParts.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            //
            this.tabPageFilters.Text = _CmpAttribute.ResourceText("IDS_TXT_FILTERS");
            this.lblCountTypeFilters.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            this.lblCountObjectFilters.Text = _CmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            this.lblTypeCounterFilters.Text = "0";
            this.lblObjectCounterFilters.Text = "0";
            this.btnSelectAllFilters.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            this.btnSelectClearFilters.Text = _CmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");

            //Set header text
            dgvCategory.Columns[0].HeaderText = string.Empty;
            dgvCategory.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            dgvCategory.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");

            dgvFamily.Columns[0].HeaderText = string.Empty;
            dgvFamily.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_FAMILY");
            dgvFamily.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");

            dgvFamilyType.Columns[0].HeaderText = string.Empty;
            dgvFamilyType.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            dgvFamilyType.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            dgvFamilyType.Columns[3].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");

            dgvParts.Columns[0].HeaderText = string.Empty;
            dgvParts.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_MATERIAL");
            dgvParts.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");

            dgvFilters.Columns[0].HeaderText = string.Empty;
            dgvFilters.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_RULEFILTERS");
            dgvFilters.Columns[2].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_COUNT");
        }

        /// ================================================================================
        /// <summary>Set Data</summary>
        ///
        /// <history><p></para>2018/10/23 Created Applied Technology</p>
        ///         <p>2021/11/10 Modified Applied Technology</p></history>
        /// ================================================================================
        private
        void SetData()
        {
            int index = 0;
            // category
            foreach (Revit.DB.ElementId key in this._DicCat.Keys)
            {
                int count = this._DicCat[key].Count;

                // get name of category by ID
                Revit.DB.Category cate = Revit.DB.Category.GetCategory(_RvtDbDoc, key);
                if (cate == null && this._DicCat[key].Count != 0)
                {
                    var first = this._DicCat[key].FirstOrDefault();

                    if (first != Revit.DB.ElementId.InvalidElementId)
                    {
                        var element = _RvtDbDoc.GetElement(first);

                        cate = element.Category;
                    }
                }
                if (cate != null)
                {
                    // line
                    if (key.Value == (long)(int)Revit.DB.BuiltInCategory.OST_Lines)
                    {
                        System.Collections.Generic.List<Revit.DB.ElementId> listModelId = new List<Revit.DB.ElementId>();
                        System.Collections.Generic.List<Revit.DB.ElementId> listDetailId = new List<Revit.DB.ElementId>();
                        string nameDetailLine = "", nameModelLine = "";
                        foreach (Revit.DB.ElementId eleId in _DicCat[key])
                        {
                            Revit.DB.Element eleFilter = _RvtDbDoc.GetElement(eleId);

                            if (eleFilter == null)
                                continue;

                            if (eleFilter as Revit.DB.DetailCurve != null)
                            {
                                nameDetailLine = eleFilter.Name;
                                listDetailId.Add(eleFilter.Id);
                            }
                            else if (eleFilter as Revit.DB.ModelCurve != null)
                            {
                                nameModelLine = eleFilter.Name;
                                listModelId.Add(eleFilter.Id);
                            }
                        }

                        if (listDetailId.Count != 0)
                        {
                            dgvCategory.Rows.Add(false, nameDetailLine, listDetailId.Count);
                            dgvCategory.Rows[index].Tag = listDetailId;
                            index++;
                        }
                        if (listModelId.Count != 0)
                        {
                            dgvCategory.Rows.Add(false, nameModelLine, listModelId.Count);
                            dgvCategory.Rows[index].Tag = listModelId;
                            index++;
                        }
                    }
                    else
                    {
                        dgvCategory.Rows.Add(false, cate.Name, count);
                        dgvCategory.Rows[index].Tag = _DicCat[key];
                        index++;
                    }
                }
                else
                {
                    dgvCategory.Rows.Add(false, _CmpAttribute.ResourceText("IDS_TXT_OTHER"), count);
                    dgvCategory.Rows[index].Tag = _DicCat[key];
                    index++;
                }
            }

            // family
            index = 0;
            foreach (string key in this._DicFam.Keys)
            {
                int count = this._DicFam[key].Count;

                // get name of family
                string[] splitVal = key.Split(':');
                if (splitVal.Length == 2)
                {
                    dgvFamily.Rows.Add(false, splitVal[1], count);
                    dgvFamily.Rows[index].Tag = this._DicFam[key];
                    index++;
                }
            }

            // family type
            index = 0;
            foreach (string key in this._DicFamType.Keys)
            {
                int count = this._DicFamType[key].Count;

                // get name of family type, category by ID
                string[] splitVal = key.Split(':');
                if (splitVal.Length == 2)
                {
                    dgvFamilyType.Rows.Add(false, splitVal[0], splitVal[1], count);
                    dgvFamilyType.Rows[index].Tag = this._DicFamType[key];
                    index++;
                }
                else
                {
                }
            }

            // part
            index = 0;
            foreach (Revit.DB.ElementId key in this._DicPart.Keys)
            {
                int count = this._DicPart[key].Count;

                // get name of part by ID
                Revit.DB.Element ele = _RvtDbDoc.GetElement(key);
                if (ele != null)
                {
                    dgvParts.Rows.Add(false, ele.Name, count);
                    dgvParts.Rows[index].Tag = key;
                    index++;
                }
            }

            // Filter
            index = 0;
            foreach (Revit.DB.ElementId key in this._DicFilter.Keys)
            {
                int count = this._DicFilter[key].Count;
                // get name of rule filter by ID
                Revit.DB.Element ele = _RvtDbDoc.GetElement(key);
                if (ele != null)
                {
                    dgvFilters.Rows.Add(false, ele.Name, count);
                    dgvFilters.Rows[index].Tag = key;
                    index++;
                }
            }
        }

        #endregion Member Functions

        // Events

        #region Events

        /// ================================================================================
        /// <summary>Load form</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2018/10/23 Created Applied Technology</p>
        ///         <p>2021/11/10 Modified Applied Technology</p></history>
        /// ================================================================================
        private void FormLevelFilter_Load(object sender, EventArgs e)
        {
            //this.SetText();
            this.dgvCategory.Sort(dgvCategory.Columns["dataCategory"], System.ComponentModel.ListSortDirection.Ascending);
            this.dgvFamily.Sort(dgvFamily.Columns["dataFamily"], System.ComponentModel.ListSortDirection.Ascending);
            this.dgvFamilyType.Sort(dgvFamilyType.Columns["dataFamilyTypeA"], System.ComponentModel.ListSortDirection.Ascending);
            this.dgvParts.Sort(dgvParts.Columns["dataParts"], System.ComponentModel.ListSortDirection.Ascending);
            this.dgvFilters.Sort(dgvFilters.Columns["cbkFilters"], System.ComponentModel.ListSortDirection.Ascending);
            // set button selectall and clear all category
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

            // set button selectall and clear all family
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

            // set button selectall and clear all family type
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

            // set button selectall and clear all part
            if (dgvParts.Rows.Count < 1)
            {
                this.btnSelectAllParts.Enabled = false;
                this.btnSelectClearParts.Enabled = false;
            }
            else
            {
                this.btnSelectAllParts.Enabled = false;
                this.btnSelectClearParts.Enabled = true;
            }
            // set button selectall and clear all filter
            if (dgvFilters.Rows.Count < 1)
            {
                this.btnSelectAllFilters.Enabled = false;
                this.btnSelectClearFilters.Enabled = false;
            }
            else
            {
                this.btnSelectAllFilters.Enabled = false;
                this.btnSelectClearFilters.Enabled = true;
            }
            // set label number selected
            this.lblCounterCategory.Text = Convert.ToString(dgvCategory.Rows.Count);
            int num1 = 0;
            foreach (DataGridViewRow row in dgvCategory.Rows)
            {
                row.Cells["cbkCategory"].Value = true;
                string valueNum = row.Cells["countCategory"].Value.ToString();
                num1 += int.Parse(valueNum);
            }
            this.lblObjCounterCategory.Text = Convert.ToString(num1);

            this.lblTypeCounterFamily.Text = Convert.ToString(dgvFamily.Rows.Count);
            int num2 = 0;
            foreach (DataGridViewRow row in dgvFamily.Rows)
            {
                row.Cells["cbkFamily"].Value = true;
                string valueNum = row.Cells["countFamily"].Value.ToString();
                num2 += int.Parse(valueNum);
            }
            this.lblObjectCounterFamily.Text = Convert.ToString(num2);

            this.lblTypeCounterFamilyType.Text = Convert.ToString(dgvFamilyType.Rows.Count);
            int num3 = 0;
            foreach (DataGridViewRow row in dgvFamilyType.Rows)
            {
                row.Cells["cbkFamilyType"].Value = true;
                string valueNum = row.Cells["countFamilyType"].Value.ToString();
                num3 += int.Parse(valueNum);
            }
            this.lblObjectCounterFamilyType.Text = Convert.ToString(num3);

            this.lblTypeCounterParts.Text = Convert.ToString(dgvParts.Rows.Count);

            int num4 = 0;
            foreach (DataGridViewRow row in dgvParts.Rows)
            {
                row.Cells["cbkParts"].Value = true;
                string valueNum = row.Cells["countParts"].Value.ToString();
                num4 += int.Parse(valueNum);
            }
            this.lblObjectCounterParts.Text = Convert.ToString(num4);

            this.lblTypeCounterFilters.Text = Convert.ToString(dgvFilters.Rows.Count);

            int num5 = 0;
            foreach (DataGridViewRow row in dgvFilters.Rows)
            {
                row.Cells["cbkFilters"].Value = true;
                string valueNum = row.Cells["countFilters"].Value.ToString();
                num5 += int.Parse(valueNum);
            }
            this.lblObjectCounterFilters.Text = Convert.ToString(num5);

            this.tabLevelFilter.SelectedIndex = this._TabNum;
            if (this.tabLevelFilter.SelectedTab == this.tabPageCategory)
                this.dgvCategory.Select();
            else if (this.tabLevelFilter.SelectedTab == this.tabPageFamily)
                this.dgvFamily.Select();
            else if (this.tabLevelFilter.SelectedTab == this.tabPageFamilyType)
            {
                this.dgvFamilyType.Select();
            }
            else if (this.tabLevelFilter.SelectedTab != this.tabPageParts)
            {
                this.dgvParts.Select();
            }
            else
            {
                if (this.tabLevelFilter.SelectedTab != this.tabPageFilters)
                    return;
                this.dgvFilters.Select();
            }
        }

        /// ================================================================================
        /// <summary>Forcus gridview when user change tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2018/10/23 Created Applied Technology</p>
        ///         <p>2021/11/10 Modified Applied Technology</p></history>
        /// ================================================================================
        private void tabLevelFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabLevelFilter.SelectedTab == this.tabPageCategory)
                this.dgvCategory.Select();
            else if (this.tabLevelFilter.SelectedTab == this.tabPageFamily)
                this.dgvFamily.Select();
            else if (this.tabLevelFilter.SelectedTab == this.tabPageFamilyType)
            {
                dgvFamilyType.Select();
            }
            else if (this.tabLevelFilter.SelectedTab == this.tabPageParts)
            {
                dgvParts.Select();
            }
            else
            {
                if (this.tabLevelFilter.SelectedTab != this.tabPageFilters)
                    return;
                this.dgvFilters.Select();
            }
        }

        /// ================================================================================
        /// <summary>Select all in category tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllCategory_Click(object sender, EventArgs e)
        {
            // checkbox = true
            foreach (DataGridViewRow dgvRow in dgvCategory.Rows)
                dgvRow.Cells["cbkCategory"].Value = true;

            this.btnSelectAllCategory.Enabled = false;
            this.btnSelectClearCategory.Enabled = true;

            // update counter
            this.lblCounterCategory.Text = Convert.ToString(dgvCategory.Rows.Count);
            int num = 0;
            foreach (DataGridViewRow row in dgvCategory.Rows)
            {
                string valueNum = row.Cells["countCategory"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjCounterCategory.Text = Convert.ToString(num);
        }

        /// ================================================================================
        /// <summary>Clear select category</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearCategory_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvCategory.Rows
                                where Convert.ToBoolean(r.Cells["cbkCategory"].Value) == false
                                select r;
            if (unCheckedRows.Count() == dgvCategory.Rows.Count)
                return;
            // checkbox = false
            foreach (DataGridViewRow dgvRow in dgvCategory.Rows)
                dgvRow.Cells["cbkCategory"].Value = false;

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
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnPrewview_Click(object sender, EventArgs e)
        {
            UpdateSelection();
        }

        /// ================================================================================
        /// <summary>Update selection</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2018/10/23 Created Applied Technology</p>
        ///         <p>2021/11/10 Modified Applied Technology</p></history>
        /// ================================================================================
        private void UpdateSelection()
        {
            switch (this.tabLevelFilter.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                    {
                        this._SelectIdAry.Clear();
                        DataGridView dgv = null;
                        if (this.tabLevelFilter.SelectedIndex == 0)     //tab Category is selected
                            dgv = dgvCategory;
                        else if (this.tabLevelFilter.SelectedIndex == 1)     //tab family is selected
                            dgv = dgvFamily;
                        else if (this.tabLevelFilter.SelectedIndex == 2)        //tab family type is selected
                            dgv = dgvFamilyType;

                        if (dgv == null)
                            return;

                        var col_checkbox = 0;
                        var checkedRows = from DataGridViewRow r in dgv.Rows
                                          where Convert.ToBoolean(r.Cells[col_checkbox].Value) == true
                                          select r;

                        foreach (var row in checkedRows)
                        {
                            try
                            {
                                List<Revit.DB.ElementId> list_ElementId = row.Tag as List<Revit.DB.ElementId>;
                                if (list_ElementId == null || list_ElementId.Count == 0)
                                    continue;

                                foreach (Revit.DB.ElementId eleId in list_ElementId)
                                {
                                    this._SelectIdAry.Add((int)eleId.Value);
                                }
                            }
                            catch { }
                        }
                        this._TabNum = this.tabLevelFilter.SelectedIndex;
                        break;
                    }

                //tab parts is selected
                case 3:
                    {
                        this._SelectIdAry.Clear();

                        var checkedRows = from DataGridViewRow r in dgvParts.Rows
                                          where Convert.ToBoolean(r.Cells["cbkParts"].Value) == true
                                          select r;

                        //Iterator all part row is checked
                        foreach (var row in checkedRows)
                        {
                            try
                            {
                                var materialId = row.Tag as Revit.DB.ElementId;
                                if (materialId == Revit.DB.ElementId.InvalidElementId)
                                    continue;

                                //iterator all part selected list
                                foreach (Revit.DB.Part part in _PartSelect)
                                {
                                    ICollection<Revit.DB.ElementId> materialIds = part.GetMaterialIds(false);
                                    if (materialIds.Count == 0)
                                        continue;

                                    var find = materialIds.ToList().Find(item => item.Equals(materialId));

                                    if (find != null)
                                    {
                                        this._SelectIdAry.Add((int)part.Id.Value);
                                    }
                                }
                            }
                            catch { }
                        }

                        this._TabNum = this.tabLevelFilter.SelectedIndex;
                    }
                    break;
                //tab filters is selected
                case 4:
                    {
                        this._SelectIdAry.Clear();

                        var checkedRows = from DataGridViewRow r in dgvFilters.Rows
                                          where Convert.ToBoolean(r.Cells["cbkFilters"].Value) == true
                                          select r;

                        //Iterator all part row is checked
                        foreach (var row in checkedRows)
                        {
                            try
                            {
                                var elementId = row.Tag as Revit.DB.ElementId;
                                if (elementId == Revit.DB.ElementId.InvalidElementId)
                                    continue;

                                //iterator all filter selected list
                                foreach (var pair in _DicFilter)
                                {
                                    if (pair.Key == null || pair.Key == Revit.DB.ElementId.InvalidElementId)
                                        continue;
                                    if (pair.Key == elementId)
                                    {
                                        var find = pair.Value;

                                        if (find != null && find.Count != 0)
                                        {
                                            foreach (var item in find)
                                                this._SelectIdAry.Add((int)item.Value);
                                        }
                                        break;
                                    }
                                }
                            }
                            catch { }
                        }

                        this._TabNum = this.tabLevelFilter.SelectedIndex;
                    }
                    break;
            }
            // user press preview
            ICollection<Revit.DB.ElementId> elementIds = (ICollection<Revit.DB.ElementId>)new List<Revit.DB.ElementId>();
            if (SelectIdAry.Count > 0)
            {
                try
                {
                    for (int index = 0; index < SelectIdAry.Count; ++index)
                    {
                        Revit.DB.ElementId elementId = new Revit.DB.ElementId((long)SelectIdAry[index]);
                        elementIds.Add(elementId);
                    }
                }
                catch
                {
                }
            }

            _RvtUIDoc.Selection.SetElementIds(elementIds);
            _RvtUIDoc.RefreshActiveView();
        }

        /// ================================================================================
        /// <summary>Button OK</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            UpdateSelection();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// ================================================================================
        /// <summary>Button cancel</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
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
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllFamily_Click(object sender, EventArgs e)
        {
            // checkbox = true
            foreach (DataGridViewRow dgvRow in dgvFamily.Rows)
                dgvRow.Cells["cbkFamily"].Value = true;

            this.btnSelectAllFamily.Enabled = false;
            this.btnSelectClearFamily.Enabled = true;

            // update counter
            this.lblTypeCounterFamily.Text = Convert.ToString(dgvFamily.Rows.Count);
            int num = 0;
            foreach (DataGridViewRow row in dgvFamily.Rows)
            {
                string valueNum = row.Cells["countFamily"].Value.ToString();
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
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearFamily_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvFamily.Rows
                                where Convert.ToBoolean(r.Cells["cbkFamily"].Value) == false
                                select r;
            if (unCheckedRows.Count() == dgvFamily.Rows.Count)
                return;
            // checkbox = false
            foreach (DataGridViewRow dgvRow in dgvFamily.Rows)
                dgvRow.Cells["cbkFamily"].Value = false;

            this.btnSelectAllFamily.Enabled = true;
            this.btnSelectClearFamily.Enabled = false;

            // update counter
            this.lblTypeCounterFamily.Text = "0";
            this.lblObjectCounterFamily.Text = "0";
        }

        /// ================================================================================
        /// <summary>Select all in family type tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllFamilyType_Click(object sender, EventArgs e)
        {
            // checkbox = true
            foreach (DataGridViewRow dgvRow in dgvFamilyType.Rows)
                dgvRow.Cells["cbkFamilyType"].Value = true;

            this.btnSelectAllFamilyType.Enabled = false;
            this.btnSelectClearFamilyType.Enabled = true;

            // update counter
            this.lblTypeCounterFamilyType.Text = Convert.ToString(dgvFamilyType.Rows.Count);
            int num = 0;
            foreach (DataGridViewRow row in dgvFamilyType.Rows)
            {
                string valueNum = row.Cells["countFamilyType"].Value.ToString();
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
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearFamilyType_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvFamilyType.Rows
                                where Convert.ToBoolean(r.Cells["cbkFamilyType"].Value) == false
                                select r;
            if (unCheckedRows.Count() == dgvFamilyType.Rows.Count)
                return;
            // checkbox = false
            foreach (DataGridViewRow dgvRow in dgvFamilyType.Rows)
                dgvRow.Cells["cbkFamilyType"].Value = false;

            this.btnSelectAllFamilyType.Enabled = true;
            this.btnSelectClearFamilyType.Enabled = false;

            // update counter
            this.lblTypeCounterFamilyType.Text = "0";
            this.lblObjectCounterFamilyType.Text = "0";
        }

        /// ================================================================================
        /// <summary>Select all in parts tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllParts_Click(object sender, EventArgs e)
        {
            // checkbox = true
            foreach (DataGridViewRow dgvRow in dgvParts.Rows)
                dgvRow.Cells["cbkParts"].Value = true;

            this.btnSelectAllParts.Enabled = false;
            this.btnSelectClearParts.Enabled = true;

            // update counter
            this.lblTypeCounterParts.Text = Convert.ToString(dgvParts.Rows.Count);
            int num = 0;
            foreach (DataGridViewRow row in dgvParts.Rows)
            {
                string valueNum = row.Cells["countParts"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterParts.Text = Convert.ToString(num);
        }

        /// ================================================================================
        /// <summary>Clear select in parts tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearParts_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvParts.Rows
                                where Convert.ToBoolean(r.Cells["cbkParts"].Value) == false
                                select r;

            if (unCheckedRows.Count() == dgvParts.Rows.Count)
                return;
            // checkbox = false
            foreach (DataGridViewRow dgvRow in dgvParts.Rows)
                dgvRow.Cells["cbkParts"].Value = false;

            this.btnSelectAllParts.Enabled = true;
            this.btnSelectClearParts.Enabled = false;

            // update counter
            this.lblTypeCounterParts.Text = "0";
            this.lblObjectCounterParts.Text = "0";
        }

        /// ================================================================================
        /// <summary>Update form information</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void dgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            // check value
            if (dgvCategory.Rows.Count == 0)
                return;
            if ((bool)dgvCategory.Rows[e.RowIndex].Cells["cbkCategory"].Value)
                dgvCategory.Rows[e.RowIndex].Cells["cbkCategory"].Value = false;
            else
                dgvCategory.Rows[e.RowIndex].Cells["cbkCategory"].Value = true;

            var checkedRows = from DataGridViewRow r in dgvCategory.Rows
                              where Convert.ToBoolean(r.Cells["cbkCategory"].Value) == true
                              select r;

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
            this.lblCounterCategory.Text = Convert.ToString(checkedRows.Count());

            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["countCategory"].Value.ToString();
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
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void dgvFamily_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            // check value
            if (dgvFamily.Rows.Count == 0)
                return;
            if ((bool)dgvFamily.Rows[e.RowIndex].Cells["cbkFamily"].Value)
                dgvFamily.Rows[e.RowIndex].Cells["cbkFamily"].Value = false;
            else
                dgvFamily.Rows[e.RowIndex].Cells["cbkFamily"].Value = true;

            // control

            var checkedRows = from DataGridViewRow r in dgvFamily.Rows
                              where Convert.ToBoolean(r.Cells["cbkFamily"].Value) == true
                              select r;

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
            this.lblTypeCounterFamily.Text = Convert.ToString(checkedRows.Count());

            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["countFamily"].Value.ToString();
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
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void dgvFamilyType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            // check value
            if (dgvFamilyType.Rows.Count == 0)
                return;
            if ((bool)dgvFamilyType.Rows[e.RowIndex].Cells["cbkFamilyType"].Value)
                dgvFamilyType.Rows[e.RowIndex].Cells["cbkFamilyType"].Value = false;
            else
                dgvFamilyType.Rows[e.RowIndex].Cells["cbkFamilyType"].Value = true;

            // control

            var checkedRows = from DataGridViewRow r in dgvFamilyType.Rows
                              where Convert.ToBoolean(r.Cells["cbkFamilyType"].Value) == true
                              select r;

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
            this.lblTypeCounterFamilyType.Text = Convert.ToString(checkedRows.Count());

            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["countFamilyType"].Value.ToString();
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
        /// <history>2018/10/23 Created Applied Technology</history>
        /// ================================================================================
        private void dgvParts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            // check value
            if (dgvParts.Rows.Count == 0)
                return;
            if ((bool)dgvParts.Rows[e.RowIndex].Cells["cbkParts"].Value)
                dgvParts.Rows[e.RowIndex].Cells["cbkParts"].Value = false;
            else
                dgvParts.Rows[e.RowIndex].Cells["cbkParts"].Value = true;

            // control

            var checkedRows = from DataGridViewRow r in dgvParts.Rows
                              where Convert.ToBoolean(r.Cells["cbkParts"].Value) == true
                              select r;

            if (checkedRows.Count() > 0 && checkedRows.Count() != dgvParts.Rows.Count)
            {
                this.btnSelectAllParts.Enabled = true;
                this.btnSelectClearParts.Enabled = true;
            }

            if (checkedRows.Count() == 0)
            {
                this.btnSelectAllParts.Enabled = true;
                this.btnSelectClearParts.Enabled = false;
            }
            if (checkedRows.Count() == dgvParts.Rows.Count)
            {
                this.btnSelectAllParts.Enabled = false;
                this.btnSelectClearParts.Enabled = true;
            }
            this.lblTypeCounterParts.Text = Convert.ToString(checkedRows.Count());

            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["countParts"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterParts.Text = Convert.ToString(num);

            dgvParts.RefreshEdit();
        }

        /// ================================================================================
        /// <summary>Update form information</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/10 Created Applied Technology</history>
        /// ================================================================================

        private void dgvFilters_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // User click header
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;

            // check value
            if (dgvFilters.Rows.Count == 0)
                return;
            if ((bool)dgvFilters.Rows[e.RowIndex].Cells["cbkFilters"].Value)
                dgvFilters.Rows[e.RowIndex].Cells["cbkFilters"].Value = false;
            else
                dgvFilters.Rows[e.RowIndex].Cells["cbkFilters"].Value = true;

            // control

            var checkedRows = from DataGridViewRow r in dgvFilters.Rows
                              where Convert.ToBoolean(r.Cells["cbkFilters"].Value) == true
                              select r;

            if (checkedRows.Count() > 0 && checkedRows.Count() != dgvFilters.Rows.Count)
            {
                this.btnSelectAllFilters.Enabled = true;
                this.btnSelectClearFilters.Enabled = true;
            }

            if (checkedRows.Count() == 0)
            {
                this.btnSelectAllFilters.Enabled = true;
                this.btnSelectClearFilters.Enabled = false;
            }
            if (checkedRows.Count() == dgvFilters.Rows.Count)
            {
                this.btnSelectAllFilters.Enabled = false;
                this.btnSelectClearFilters.Enabled = true;
            }
            this.lblTypeCounterFilters.Text = Convert.ToString(checkedRows.Count());

            int num = 0;
            foreach (var row in checkedRows)
            {
                string valueNum = row.Cells["countFilters"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterFilters.Text = Convert.ToString(num);

            dgvFilters.RefreshEdit();
        }

        /// ================================================================================
        /// <summary> Select all in filters tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/10 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectAllFilters_Click(object sender, EventArgs e)
        {
            // checkbox = true
            foreach (DataGridViewRow dgvRow in dgvFilters.Rows)
                dgvRow.Cells["cbkFilters"].Value = true;

            this.btnSelectAllFilters.Enabled = false;
            this.btnSelectClearFilters.Enabled = true;

            // update counter
            this.lblTypeCounterFilters.Text = Convert.ToString(dgvFilters.Rows.Count);
            int num = 0;
            foreach (DataGridViewRow row in dgvFilters.Rows)
            {
                string valueNum = row.Cells["countFilters"].Value.ToString();
                num += int.Parse(valueNum);
            }
            this.lblObjectCounterFilters.Text = Convert.ToString(num);
        }

        /// ================================================================================
        /// <summary>Clear select in filters tab</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/10 Created Applied Technology</history>
        /// ================================================================================
        private void btnSelectClearFilters_Click(object sender, EventArgs e)
        {
            var unCheckedRows = from DataGridViewRow r in dgvFilters.Rows
                                where Convert.ToBoolean(r.Cells["cbkFilters"].Value) == false
                                select r;

            if (unCheckedRows.Count() == dgvFilters.Rows.Count)
                return;
            // checkbox = false
            foreach (DataGridViewRow dgvRow in dgvFilters.Rows)
                dgvRow.Cells["cbkFilters"].Value = false;

            this.btnSelectAllFilters.Enabled = true;
            this.btnSelectClearFilters.Enabled = false;

            // update counter
            this.lblTypeCounterFilters.Text = "0";
            this.lblObjectCounterFilters.Text = "0";
        }

        #endregion Events
    }
}