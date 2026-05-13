using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using RvtExtApp = ADSK.JExtRAC.SwitchJoinOrder;
using ADSK.JExtRAC.SwitchJoinOrder.Entities;

namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    public partial class FormSettingDetail : Form
    {
        // Member variable

        #region Member Variables

        /// <summary>FormSwitchJoin</summary>
        private RvtExtApp.UI.FormSwitchJoin _form;

        /// <summary>Document</summary>
        private Autodesk.Revit.DB.Document _Doc;

        /// <summary>Attributes</summary>
        private RvtExtApp.Components.Attribute _cmpAttribute;

        /// <summary>Category Item</summary>
        private CategoryItem _categoryItem;

        private static string RegistryBasePath => @"Software\VB and VBA Program Settings\" + Assembly.GetExecutingAssembly().GetName().Name;

        #endregion Member Variables

        // Member constructor

        #region Member Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="cmpAttribute">attribute</param>
        /// <param name="categoryItem">category item</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        public FormSettingDetail(RvtExtApp.UI.FormSwitchJoin form, Autodesk.Revit.DB.Document doc, RvtExtApp.Components.Attribute cmpAttribute, CategoryItem categoryItem)
        {
            InitializeComponent();
            _form = form;
            _Doc = doc;
            _cmpAttribute = cmpAttribute;
            _categoryItem = categoryItem;
            SetText();
            SetData();
            this.btnApply.Enabled = false;
        }

        #endregion Member Constructor

        // Member functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Form character setting</summary>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _cmpAttribute.ResourceText("IDS_TXT_SWITCHJOINORDER") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.lbLeft.Text = _categoryItem._name + _cmpAttribute.ResourceText("IDS_TXT_LIST_FAMILY");
            this.lbRight.Text = _cmpAttribute.ResourceText("IDS_TXT_LBRIGHT");
            this.lbCount.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNT");

            this.lbUp.Text = _cmpAttribute.ResourceText("IDS_TXT_LABEL_UP");
            this.lbDown.Text = _cmpAttribute.ResourceText("IDS_TXT_LABEL_DOWN");
            this.lbPriority.Text = _cmpAttribute.ResourceText("IDS_TXT_LABEL_PRIORITY");

            this.btnRm.Text = _cmpAttribute.ResourceText("IDS_TXT_TEXTLEFT");
            this.btnAdd.Text = _cmpAttribute.ResourceText("IDS_TXT_TEXTRIGHT");

            this.btnApply.Text = _cmpAttribute.ResourceText("IDS_TXT_APPLY");
            this.btnClose.Text = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");

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

        /// ================================================================================
        /// <summary>Form data setting</summary>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================

        private void SetData()
        {
            if (_categoryItem == null || _categoryItem._listFamilyItem == null)
                return;
            //Get setting
            List<string> listAllName = new List<string>();
            string setting = GetRegistrySetting(this.Name, this.dgvRight.Name, "");
            if (!string.IsNullOrEmpty(setting))
            {
                string[] strArray = setting.Split(',');
                listAllName.AddRange(strArray);
            }

            // List family item
            List<FamilyItem> familyItems = new List<FamilyItem>();

            // Add to datagridview
            foreach (var nameFamily in listAllName)
            {
                // Get family
                FamilyItem clsFamilyItem = _categoryItem.FindFamilyByName(nameFamily);
                if (clsFamilyItem == null)
                    continue;

                if (listAllName.Contains(clsFamilyItem._nameFami) == true && clsFamilyItem._nameCate == _categoryItem._name)
                {
                    familyItems.Add(clsFamilyItem);

                    //Add data to dgvRight
                    AddRowDataGrid(this.dgvRight, clsFamilyItem, true);
                }
            }
            foreach (var famiItem in _categoryItem._listFamilyItem)
            {
                if (famiItem._nameCate == _categoryItem._name)
                {
                    if (!familyItems.Contains(famiItem))
                        AddRowDataGrid(this.dgvLeft, famiItem, false);  // Add data to dgvLeft
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(System.Runtime.InteropServices.HandleRef hWnd);

        // ================================================================================
        /// <summary>Get data</summary>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void GetData()
        {
            _categoryItem._listElementId.Clear();
            _categoryItem._listFamilyItem.Clear();
            _categoryItem._isJoinFami = true;

            try
            {
                int j = 0;
                for (int i = 0; i < this.dgvRight.Rows.Count; i++)
                {
                    //Get family item
                    FamilyItem objFami = this.dgvRight.Rows[i].Tag as FamilyItem;
                    if (objFami == null)
                        continue;
                    objFami._indexFami = j;
                    _categoryItem._listFamilyItem.Add(objFami);

                    if (objFami._listElementIdOfFamily != null && objFami._listElementIdOfFamily.Count > 0)
                    {
                        foreach (var eleId in objFami._listElementIdOfFamily)
                            _categoryItem._listElementId.Add(eleId);
                    }
                    j++;
                }

                for (int i = 0; i < this.dgvLeft.Rows.Count; i++)
                {
                    //Get family item
                    FamilyItem objFami = this.dgvLeft.Rows[i].Tag as FamilyItem;
                    if (objFami == null)
                        continue;

                    objFami._indexFami = j;
                    _categoryItem._listFamilyItem.Add(objFami);

                    if (objFami._listElementIdOfFamily != null && objFami._listElementIdOfFamily.Count > 0)
                    {
                        foreach (var eleId in objFami._listElementIdOfFamily)
                            _categoryItem._listElementId.Add(eleId);
                    }
                    j++;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary> Reset data  grid view</summary>
        ///
        /// <param name="DgvRow">DataGridViewRow</param>
        /// <param name="faItem">FamilyItem</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        public static void ReNewDataGridRow(DataGridViewRow DgvRow, FamilyItem faItem)
        {
            DgvRow.Cells[0].Value = faItem;
            DgvRow.Cells[1].Value = "(" + faItem._listElementIdOfFamily.Count.ToString() + ")";
            DgvRow.Tag = faItem;
        }

        /// ================================================================================
        /// <summary> Add data datagridview</summary>
        ///
        /// <param name="dtgview">DataGridView</param>
        /// <param name="famiItem">FamilyItem</param>
        /// <param name="isDgvRight"></param>
        /// <returns></returns>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        public static int AddRowDataGrid(DataGridView dtgview, FamilyItem famiItem, bool isDgvRight)
        {
            int addIdx = dtgview.Rows.Add();
            dtgview.Rows[addIdx].Cells[0].Value = famiItem;

            if (isDgvRight)
                dtgview.Rows[addIdx].Cells[1].Value = "(" + famiItem._listElementIdOfFamily.Count.ToString() + ")";

            dtgview.Rows[addIdx].Tag = famiItem;
            return addIdx;
        }

        /// ================================================================================
        /// <summary> Save settings</summary>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void SaveSetting()
        {
            try
            {
                string retSave = "";
                string setting = GetRegistrySetting(this.Name, this.dgvRight.Name, "");
                if (!string.IsNullOrEmpty(setting))
                {
                    string[] strArray = setting.Split(',');
                    foreach (var item in strArray)
                    {
                        if (!_form.listValueSetting.Contains(item))
                            _form.listValueSetting.Add(item);
                    }
                }

                for (int i = 0; i < this.dgvRight.Rows.Count; i++)
                {
                    //Get family item
                    FamilyItem fami = this.dgvRight.Rows[i].Tag as FamilyItem;
                    if (fami == null)
                        continue;
                    string nameFami = fami._nameFami;
                    if (string.IsNullOrEmpty(nameFami))
                        continue;
                    if (_form.listValueSetting.Contains(nameFami))
                        _form.listValueSetting.Remove(nameFami);
                    _form.listValueSetting.Add(nameFami);
                }
                for (int i = 0; i < this.dgvLeft.Rows.Count; i++)
                {
                    //Get family item
                    FamilyItem fami = this.dgvLeft.Rows[i].Tag as FamilyItem;
                    if (fami == null)
                        continue;
                    string nameFami = fami._nameFami;

                    if (_form.listValueSetting.Contains(nameFami))
                        _form.listValueSetting.Remove(nameFami);
                }
                if (_form.listValueSetting != null && _form.listValueSetting.Count > 0)
                {
                    foreach (var item in _form.listValueSetting)
                        retSave = retSave + "," + item;
                }
                SaveRegistrySetting(this.Name, this.dgvRight.Name, retSave.Length > 1 ? retSave.Substring(1) : "");
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        #endregion Member Functions

        // Event

        #region Event

        /// ================================================================================
        /// <summary>Handles the Click event of the btnReturn control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            // save setting
            SaveSetting();

            // Get data
            GetData();

            this.btnApply.Enabled = false;
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnAdd control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this.dgvLeft.SelectedRows.Count == 0)
                return;
            this.btnApply.Enabled = true;
            try
            {
                for (int i = 0, loopTo = this.dgvLeft.SelectedRows.Count - 1; i <= loopTo; i++)
                {
                    int idx = this.dgvLeft.SelectedRows[i].Index;
                    //Get family item
                    FamilyItem famiItem = (FamilyItem)this.dgvLeft.Rows[idx].Tag;

                    // Add row
                    AddRowDataGrid(this.dgvRight, famiItem, true);

                    // Remove row
                    this.dgvLeft.Rows.RemoveAt(idx);

                    // Select row
                    if (idx > 0)
                        this.dgvRight.Rows[idx - 1].Selected = true;
                    else
                        this.dgvRight.Rows[0].Selected = true;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnRm control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void btnRm_Click(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0)
                return;
            this.btnApply.Enabled = true;
            try
            {
                for (int i = 0, loopTo = this.dgvRight.SelectedRows.Count - 1; i <= loopTo; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;

                    //Get family item
                    FamilyItem famiItem = (FamilyItem)this.dgvRight.Rows[idx].Tag;

                    // Add row
                    AddRowDataGrid(this.dgvLeft, famiItem, false);

                    // Remove row
                    this.dgvRight.Rows.RemoveAt(idx);

                    // Select row
                    if (idx > 0)
                        this.dgvRight.Rows[idx - 1].Selected = true;
                    else
                    {
                        if (this.dgvRight.Rows.Count > 0)
                            this.dgvRight.Rows[0].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnUp control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0)
                return;
            this.btnApply.Enabled = true;
            try
            {
                for (int i = 0, loopTo = this.dgvRight.SelectedRows.Count - 1; i <= loopTo; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    if (idx != 0)
                    {
                        // Sort item
                        _categoryItem.SwapFml(idx, idx - 1);

                        //Update form

                        //Get family item
                        FamilyItem famiItem = (FamilyItem)this.dgvRight.Rows[idx].Tag;

                        // Remove row
                        this.dgvRight.Rows.RemoveAt(idx);

                        // Insert row
                        this.dgvRight.Rows.Insert(checked(idx - 1), (object)famiItem);

                        // Renew row
                        ReNewDataGridRow(this.dgvRight.Rows[idx - 1], famiItem);

                        //Select row
                        this.dgvRight.Rows[idx - 1].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnDown control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0)
                return;
            this.btnApply.Enabled = true;
            try
            {
                for (int i = 0, loopTo = this.dgvRight.SelectedRows.Count - 1; i <= loopTo; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;
                    if (idx != checked(this.dgvRight.Rows.Count - 1))
                    {
                        // Sort item
                        _categoryItem.SwapFml(idx + 1, idx);

                        //Update form

                        //Get family item
                        FamilyItem famiItem = (FamilyItem)this.dgvRight.Rows[idx].Tag;

                        // Remove row
                        this.dgvRight.Rows.RemoveAt(idx);

                        // Insert row
                        this.dgvRight.Rows.Insert(checked(idx + 1), (object)famiItem);

                        // Renew row
                        ReNewDataGridRow(this.dgvRight.Rows[idx + 1], famiItem);

                        // Select row
                        this.dgvRight.Rows[idx + 1].Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Handles the dgvLef double click</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void dgvLeft_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvLeft.SelectedRows.Count == 0)
                return;

            this.btnApply.Enabled = true;
            try
            {
                for (int i = 0, loopTo = this.dgvLeft.SelectedRows.Count - 1; i <= loopTo; i++)
                {
                    int idx = this.dgvLeft.SelectedRows[i].Index;

                    //Get family item
                    FamilyItem famiItem = (FamilyItem)this.dgvLeft.Rows[idx].Tag;

                    // Add row
                    AddRowDataGrid(this.dgvRight, famiItem, true);

                    // Remove row
                    this.dgvLeft.Rows.RemoveAt(idx);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        /// ================================================================================
        /// <summary>Handles the dgvRight double click</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/12/28 Created Applied Technology</history>
        /// ================================================================================
        private void dgvRight_DoubleClick(object sender, EventArgs e)
        {
            if (this.dgvRight.SelectedRows.Count == 0)
                return;
            this.btnApply.Enabled = true;
            try
            {
                for (int i = 0, loopTo = this.dgvRight.SelectedRows.Count - 1; i <= loopTo; i++)
                {
                    int idx = this.dgvRight.SelectedRows[i].Index;

                    // Get family item
                    FamilyItem famiItem = (FamilyItem)this.dgvRight.Rows[idx].Tag;

                    // Add row
                    AddRowDataGrid(this.dgvLeft, famiItem, false);

                    // Remove row
                    this.dgvRight.Rows.RemoveAt(idx);
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        #endregion Event
    }
}
