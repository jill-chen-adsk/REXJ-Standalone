using ADSK.JExtRAC.SwitchJoinOrder.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.SwitchJoinOrder;

namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    public partial class FormSwitchJoin : Form
    {
        public RvtExtApp.Components.Attribute _cmpAttribute;
        private CategoryItems _groupCategory;
        private bool _cbkChecked;
        private Autodesk.Revit.DB.Document _Doc;
        public List<string> listValueSetting = new List<string>();

        private static string RegistryBasePath => @"Software\VB and VBA Program Settings\" + Assembly.GetExecutingAssembly().GetName().Name;

        public FormSwitchJoin(Autodesk.Revit.DB.Document doc, RvtExtApp.Components.Attribute cmpAttribute, CategoryItems groupCategory)
        {
            _cmpAttribute = cmpAttribute;
            _Doc = doc;
            _groupCategory = groupCategory;

            InitializeComponent();

            this.TopMost = true;
            SetText();
            SetData();
        }

        private void SetText()
        {
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_SWITCHJOINORDER") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            this.lbLeft.Text = _cmpAttribute.ResourceText("IDS_TXT_LBLEFT");
            this.lbRight.Text = _cmpAttribute.ResourceText("IDS_TXT_LBRIGHT");
            this.lbCount.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNT");
            this.lbUp.Text = _cmpAttribute.ResourceText("IDS_TXT_LABEL_UP");
            this.lbDown.Text = _cmpAttribute.ResourceText("IDS_TXT_LABEL_DOWN");
            this.lbPriority.Text = _cmpAttribute.ResourceText("IDS_TXT_LABEL_PRIORITY");
            this.btnRm.Text = _cmpAttribute.ResourceText("IDS_TXT_TEXTLEFT");
            this.btnAdd.Text = _cmpAttribute.ResourceText("IDS_TXT_TEXTRIGHT");
            this.btnOK.Text = _cmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
            this.btnDetails.Text = _cmpAttribute.ResourceText("IDS_TXT_DETAILS");
            this.ckbGroup.Text = _cmpAttribute.ResourceText("IDS_CK");

            this.Icon = _cmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
            this.btnUp.Image = _cmpAttribute.ResourceImage("IDI_PIC_UP") as System.Drawing.Image;
            this.btnDown.Image = _cmpAttribute.ResourceImage("IDI_PIC_DOWN") as System.Drawing.Image;
        }

        private static string GetRegistrySetting(string section, string key, string defaultValue)
        {
            try
            {
                string path = RegistryBasePath + "\\" + section;
                object val = Registry.GetValue(@"HKEY_CURRENT_USER\" + path, key, defaultValue);
                return val?.ToString() ?? defaultValue;
            }
            catch { return defaultValue; }
        }

        private static void SaveRegistrySetting(string section, string key, string value)
        {
            try
            {
                string path = RegistryBasePath + "\\" + section;
                Registry.SetValue(@"HKEY_CURRENT_USER\" + path, key, value);
            }
            catch { }
        }

        private void SetData()
        {
            if (_groupCategory._categoryShow == null) return;

            List<string> listAllName = new List<string>();
            string setting = GetRegistrySetting(this.Name, this.dgvRight.Name, "");
            if (!string.IsNullOrEmpty(setting))
            {
                string[] strArray = setting.Split(',');
                listAllName.AddRange(strArray);
                if (strArray.Length >= 2)
                {
                    foreach (string nameCtg in strArray)
                    {
                        if (_groupCategory.Contains(nameCtg))
                        {
                            int index = _groupCategory.Find(nameCtg);
                            if (index == -1) continue;
                            if (_groupCategory._categoryShow.Count > 1)
                                _groupCategory.Add(_groupCategory.Remove(index));
                            else
                                _groupCategory.Add(_groupCategory.FindByName(nameCtg));
                        }
                    }
                }
            }

            foreach (var catagory in _groupCategory._categoryShow)
            {
                if (listAllName.Contains(catagory._name))
                    AddRowDataGrid(dgvRight, catagory, true);
                else
                    AddRowDataGrid(dgvLeft, catagory, false);
            }
        }

        public bool GetChecked => _cbkChecked;

        private void GetData()
        {
            _cbkChecked = ckbGroup.Checked;
            _groupCategory.ClearCtg();
            try
            {
                for (int i = 0; i < this.dgvRight.Rows.Count; i++)
                {
                    CategoryItem objCata = this.dgvRight.Rows[i].Tag as CategoryItem;
                    if (objCata == null) continue;
                    _groupCategory.Add(objCata);
                }
            }
            catch (Exception) { }
        }

        public static void ReNewDataGridRow(DataGridViewRow DgvRow, CategoryItem dlgItem)
        {
            try
            {
                DgvRow.Cells[0].Value = dlgItem;
                DgvRow.Cells[1].Value = "(" + dlgItem._listElementId.Count.ToString() + ")";
                DgvRow.Tag = dlgItem;
            }
            catch (Exception) { }
        }

        public static int AddRowDataGrid(DataGridView dtgview, CategoryItem cateItem, bool isDgvRight)
        {
            int addIdx = dtgview.Rows.Add();
            try
            {
                dtgview.Rows[addIdx].Cells[0].Value = cateItem;
                if (isDgvRight)
                    dtgview.Rows[addIdx].Cells[1].Value = "(" + cateItem._listElementId.Count.ToString() + ")";
                dtgview.Rows[addIdx].Tag = cateItem;
            }
            catch (Exception) { }
            return addIdx;
        }

        private void CheckEnable(CategoryItem categoryItem)
        {
            if (categoryItem == null) return;
            this.btnDetails.Enabled = categoryItem._listFamilyItem != null && categoryItem._listFamilyItem.Count > 0;
        }

        private void CheckEnable()
        {
            if (this.dgvRight.SelectedRows.Count == 0) { this.btnDetails.Enabled = true; return; }
            try
            {
                CategoryItem categoryItem = new CategoryItem();
                for (int i = 0; i <= this.dgvRight.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    categoryItem = (CategoryItem)this.dgvRight.Rows[idx].Tag;
                }
                CheckEnable(categoryItem);
            }
            catch (Exception) { }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0) return;
            try
            {
                for (int i = 0; i <= this.dgvRight.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    if (idx != 0)
                    {
                        _groupCategory.SwapCtg(idx, idx - 1);
                        CategoryItem clsCategoryItem = (CategoryItem)this.dgvRight.Rows[idx].Tag;
                        this.dgvRight.Rows.RemoveAt(idx);
                        this.dgvRight.Rows.Insert(idx - 1, (object)clsCategoryItem);
                        ReNewDataGridRow(this.dgvRight.Rows[idx - 1], clsCategoryItem);
                        this.dgvRight.Rows[idx - 1].Selected = true;
                    }
                }
            }
            catch (Exception) { }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0) return;
            try
            {
                for (int i = 0; i <= this.dgvRight.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    if (idx != this.dgvRight.Rows.Count - 1)
                    {
                        _groupCategory.SwapCtg(idx + 1, idx);
                        CategoryItem clsCategoryItem = (CategoryItem)this.dgvRight.Rows[idx].Tag;
                        this.dgvRight.Rows.RemoveAt(idx);
                        this.dgvRight.Rows.Insert(idx + 1, (object)clsCategoryItem);
                        ReNewDataGridRow(this.dgvRight.Rows[idx + 1], clsCategoryItem);
                        this.dgvRight.Rows[idx + 1].Selected = true;
                    }
                }
            }
            catch (Exception) { }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (this.dgvLeft.SelectedRows.Count == 0) return;
            try
            {
                for (int i = 0; i <= this.dgvLeft.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvLeft.SelectedRows[i].Index;
                    CategoryItem clsCategoryItem = (CategoryItem)this.dgvLeft.Rows[idx].Tag;
                    AddRowDataGrid(dgvRight, clsCategoryItem, true);
                    this.dgvLeft.Rows.RemoveAt(idx);
                    if (idx > 0) this.dgvRight.Rows[idx - 1].Selected = true;
                    else this.dgvRight.Rows[0].Selected = true;
                }
            }
            catch (Exception) { }
            CheckEnable();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0) return;
            try
            {
                for (int i = 0; i <= this.dgvRight.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    CategoryItem clsCategoryItem = (CategoryItem)this.dgvRight.Rows[idx].Tag;
                    AddRowDataGrid(dgvLeft, clsCategoryItem, false);
                    this.dgvRight.Rows.RemoveAt(idx);
                    if (idx > 0) this.dgvRight.Rows[idx - 1].Selected = true;
                    else if (this.dgvRight.Rows.Count > 0) this.dgvRight.Rows[0].Selected = true;
                }
            }
            catch (Exception) { }
            CheckEnable();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string retSave = "";
            for (int i = 0; i < this.dgvRight.Rows.Count; i++)
            {
                CategoryItem cate = this.dgvRight.Rows[i].Tag as CategoryItem;
                if (cate == null) continue;
                retSave = retSave + "," + cate._name;
            }
            if (retSave.Length > 1)
                SaveRegistrySetting(this.Name, this.dgvRight.Name, retSave.Substring(1));

            GetData();
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e) { this.Close(); }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0) return;
            try
            {
                CategoryItem categoryItem = new CategoryItem();
                for (int i = 0; i <= this.dgvRight.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    categoryItem = (CategoryItem)this.dgvRight.Rows[idx].Tag;
                }
                RvtExtApp.UI.FormSettingDetail form = new RvtExtApp.UI.FormSettingDetail(this, _Doc, _cmpAttribute, categoryItem);
                form.TopMost = true;
                form.ShowDialog();
            }
            catch (Exception) { }
        }

        private void dgvRight_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0) return;
            try
            {
                for (int i = 0; i <= this.dgvRight.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    CategoryItem clsCategoryItem = (CategoryItem)this.dgvRight.Rows[idx].Tag;
                    AddRowDataGrid(dgvLeft, clsCategoryItem, false);
                    this.dgvRight.Rows.RemoveAt(idx);
                }
            }
            catch (Exception) { }
            CheckEnable();
        }

        private void dgvLeft_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvLeft.SelectedRows.Count == 0) return;
            try
            {
                for (int i = 0; i <= this.dgvLeft.SelectedRows.Count - 1; i++)
                {
                    int idx = this.dgvLeft.SelectedRows[i].Index;
                    CategoryItem clsCategoryItem = (CategoryItem)this.dgvLeft.Rows[idx].Tag;
                    AddRowDataGrid(dgvRight, clsCategoryItem, true);
                    this.dgvLeft.Rows.RemoveAt(idx);
                    if (idx > 0) this.dgvRight.Rows[idx - 1].Selected = true;
                    else this.dgvRight.Rows[0].Selected = true;
                }
            }
            catch (Exception) { }
            CheckEnable();
        }

        private void dgvRight_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;
            if (dgvRight.Rows.Count == 0) return;
            CategoryItem categoryItem = (CategoryItem)this.dgvRight.Rows[e.RowIndex].Tag;
            CheckEnable(categoryItem);
        }

        private void FormSwitchJoin_Load(object sender, EventArgs e) { CheckEnable(); }
    }
}
