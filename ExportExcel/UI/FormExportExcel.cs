using ADSK.JExtRAC.ExportExcel.Entities;
using ADSK.JExtRAC.ExportExcel.Utils;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using RvtExtApp = ADSK.JExtRAC.ExportExcel;

namespace ADSK.JExtRAC.ExportExcel.UI
{
    public partial class FormExportExcel : System.Windows.Forms.Form
    {
        private RvtExtApp.Components.Attribute _CmpAttribute;
        private Document _Doc = null;
        private List<Element> _ElementList = null;
        private List<CategoryItem> _Categories = null;

        public FormExportExcel(Document doc, RvtExtApp.Components.Attribute cmpAttribute, List<Element> elementList, List<CategoryItem> categories)
        {
            _Doc = doc;
            _ElementList = elementList;
            _Categories = categories;
            _CmpAttribute = cmpAttribute;

            InitializeComponent();
            SetLocalizedText();

            this.Text += string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            btnUp.Image = _CmpAttribute.ResourceImage("IDI_PIC_UP") as System.Drawing.Image;
            btnDown.Image = _CmpAttribute.ResourceImage("IDI_PIC_DOWN") as System.Drawing.Image;

            tvParameters.Sort();
        }

        private void SetLocalizedText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_FORM_TITLE");
            lblCategory.Text = _CmpAttribute.ResourceText("IDS_LBL_CATEGORY");
            lblParameter.Text = _CmpAttribute.ResourceText("IDS_LBL_PARAMETER");
            lblExport.Text = _CmpAttribute.ResourceText("IDS_LBL_OUTPUT");
            btnSearchCategory.Text = _CmpAttribute.ResourceText("IDS_BTN_SEARCH");
            btnSearchParameter.Text = _CmpAttribute.ResourceText("IDS_BTN_SEARCH");
            btnSearchOutput.Text = _CmpAttribute.ResourceText("IDS_BTN_SEARCH");
            btnRestore.Text = _CmpAttribute.ResourceText("IDS_BTN_RESTORE");
            btnLoad.Text = _CmpAttribute.ResourceText("IDS_BTN_LOAD");
            btnSave.Text = _CmpAttribute.ResourceText("IDS_BTN_SAVE");
            btnOK.Text = "OK";
            btnCancel.Text = _CmpAttribute.ResourceText("IDS_BTN_CANCEL");
        }

        private void DisplayCategories(string search = null)
        {
            clbCategories.Items.Clear();

            if (_Categories == null || _Categories.Count == 0)
                return;

            foreach (ObjectItem category in _Categories)
            {
                if (search != null && search != string.Empty)
                {
                    if (category.ToString().ToUpper().Contains(search.ToUpper()) == false)
                        continue;
                }

                clbCategories.Items.Add(category, false);
            }
        }

        private void DisplayExports(List<CategoryItem> categories)
        {
            categories.Sort(delegate (CategoryItem c1, CategoryItem c2)
            {
                var max1 = c1._Parameters.Max(item => item._IndexExport);
                var max2 = c2._Parameters.Max(item => item._IndexExport);
                return max1.CompareTo(max2);
            });

            tvExports.Nodes.Clear();

            foreach (CategoryItem categoryItem in categories)
            {
                DisplayExport(categoryItem);
            }
            tvExports.ExpandAll();
        }

        private void DisplayExport(CategoryItem categoryItem, string searching = null)
        {
            var exports = categoryItem._Parameters.Where(item => item._IndexExport != ParameterData._NotExport).ToList();

            if (exports.Count == 0)
                return;

            var groupNode = tvExports.Nodes.Add(categoryItem.ToString());
            groupNode.Tag = categoryItem;

            exports.Sort(delegate (ParameterData para1, ParameterData para2)
            {
                return para1._IndexExport.CompareTo(para2._IndexExport);
            });

            foreach (ParameterData parameter in exports)
            {
                if (searching != null && searching != string.Empty)
                {
                    if (parameter.ToString().ToUpper().Contains(searching.ToUpper()) == false)
                        continue;
                }

                var node = groupNode.Nodes.Add(parameter.ToString());
                node.Tag = parameter;
            }
            if (groupNode.Nodes.Count == 0)
            {
                tvExports.Nodes.Remove(groupNode);
                return;
            }
            else
                groupNode.ExpandAll();
        }

        private void DisplayParameters(List<CategoryItem> categories, string searching = null)
        {
            tvParameters.Nodes.Clear();

            foreach (CategoryItem categoryItem in categories)
            {
                DisplayParameter(categoryItem, searching);
            }
            tvParameters.ExpandAll();
        }

        private void DisplayParameter(CategoryItem categoryItem, string searching = null)
        {
            var groupNode = tvParameters.Nodes.Add(categoryItem.ToString());
            groupNode.Tag = categoryItem;

            foreach (ParameterData parameter in categoryItem._Parameters)
            {
                if (parameter._IndexExport == ParameterData._NotExport)
                {
                    if (searching != null && searching != string.Empty)
                    {
                        if (parameter.ToString().ToUpper().Contains(searching.ToUpper()) == false)
                            continue;
                    }
                    var node = groupNode.Nodes.Add(parameter.ToString());
                    node.Tag = parameter;
                }
            }
            if (groupNode.Nodes.Count == 0)
            {
                tvParameters.Nodes.Remove(groupNode);
                return;
            }
            else
                groupNode.ExpandAll();
        }

        private void Searching(ListBox listBox, TextBox txtSearch)
        {
            var search = txtSearch.Text.Trim();
            DisplayCategories(search);

            if (search != null && search != string.Empty)
            {
                if (listBox.Items.Count == 0)
                {
                    MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERROR_SEARCH_NOT_FOUND"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    txtSearch.Focus();
                }
                else
                {
                    int index = -1;
                    int minLenght = int.MaxValue;

                    for (int i = 0; i < listBox.Items.Count; i++)
                    {
                        object item = listBox.Items[i];
                        var objItem = item as ObjectItem;
                        if (objItem == null) continue;

                        if (objItem.ToString().ToUpper() == search.ToUpper())
                        {
                            listBox.SelectedIndex = i;
                            return;
                        }
                        else
                        {
                            var text = objItem.ToString();
                            var result = text.ToUpper().Replace(search.ToUpper(), "");
                            if (result.Length < minLenght && text.Length > result.Length)
                            {
                                minLenght = result.Length;
                                index = i;
                            }
                        }
                    }

                    listBox.ClearSelected();
                    if (index != -1)
                    {
                        listBox.SelectedIndex = index;
                    }
                }
            }
        }

        private void Searching(TreeView treeView, TextBox txtSearch)
        {
            treeView.Nodes.Clear();
            var search = txtSearch.Text.Trim();

            for (int i = 0; i < clbCategories.Items.Count; i++)
            {
                object item = clbCategories.Items[i];
                var catetogyItem = item as CategoryItem;

                if (catetogyItem == null || catetogyItem._IsChecked == false)
                    continue;

                if (treeView == tvParameters)
                    DisplayParameter(catetogyItem, search);
                else
                    DisplayExport(catetogyItem, search);
            }
            if (search != null && search != string.Empty)
            {
                if (treeView.Nodes.Count == 0)
                {
                    MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERROR_SEARCH_NOT_FOUND"), _CmpAttribute.ResourceText("IDS_TXT_ERROR"));
                    txtSearch.Focus();
                }
                else
                {
                    TreeNode selectNode = null;
                    int minLenght = int.MaxValue;

                    for (int i = 0; i < treeView.Nodes.Count; i++)
                    {
                        TreeNode group = treeView.Nodes[i];
                        var categoryItem = group.Tag as CategoryItem;
                        if (categoryItem == null) continue;

                        foreach (TreeNode node in group.Nodes)
                        {
                            var objItem = node.Tag as ObjectItem;
                            if (objItem == null) continue;

                            if (objItem.ToString().ToUpper() == search.ToUpper())
                            {
                                treeView.SelectedNode = node;
                                return;
                            }
                            else
                            {
                                var text = objItem.ToString();
                                var result = text.ToUpper().Replace(search.ToUpper(), "");
                                if (result.Length < minLenght && text.Length > result.Length)
                                {
                                    minLenght = result.Length;
                                    selectNode = node;
                                }
                            }
                        }
                    }

                    treeView.HideSelection = true;
                    if (selectNode != null)
                    {
                        treeView.SelectedNode = selectNode;
                        treeView.Focus();
                    }
                }
            }
            else
            {
                if (treeView.Nodes.Count != 0)
                {
                    treeView.Nodes[0].EnsureVisible();
                }
            }
        }

        private void ExportPrameter(TreeView tvRemove, TreeView tvAdd, bool isExport)
        {
            if (tvRemove.Nodes.Count == 0 || tvRemove.SelectedNode == null)
                return;

            ParameterData parameterData = tvRemove.SelectedNode.Tag as ParameterData;
            if (parameterData == null) return;

            var parent = tvRemove.SelectedNode.Parent;
            if (parent == null || parent.Tag == null) return;

            tvRemove.Nodes.Remove(tvRemove.SelectedNode);

            int index = -1;
            var founds = tvAdd.Nodes.Cast<TreeNode>()
                                    .Where(r => r.Text == parent.Text)
                                    .ToList();

            if (founds.Count != 0)
            {
                var groupNode = founds.First();
                var node = groupNode.Nodes.Add(parameterData.ToString());
                node.Tag = parameterData;
                index = node.Index;
                tvAdd.SelectedNode = node;
                groupNode.ExpandAll();
            }
            else
            {
                var groupNode = tvAdd.Nodes.Add(parent.Text);
                groupNode.Tag = parent.Tag;
                var node = groupNode.Nodes.Add(parameterData.ToString());
                node.Tag = parameterData;
                index = node.Index;
                tvAdd.SelectedNode = node;
                groupNode.ExpandAll();
            }

            if (isExport == true)
                parameterData._IndexExport = index;
            else
                parameterData._IndexExport = ParameterData._NotExport;

            tvRemove.Focus();
            tvAdd.Focus();

            ResetExportIndex();
        }

        private void ResetExportIndex()
        {
            int index = 0;
            for (int i = 0; i < tvExports.Nodes.Count; i++)
            {
                var groupNode = tvExports.Nodes[i] as TreeNode;
                foreach (TreeNode node in groupNode.Nodes)
                {
                    var parameterData = node.Tag as ParameterData;
                    if (parameterData == null) continue;
                    parameterData._IndexExport = index++;
                }
            }
        }

        private void SetData(List<CategoryItem> loaded_categories)
        {
            var list_category = new List<CategoryItem>();

            foreach (CategoryItem loaded_cate in loaded_categories)
            {
                var categoryItem = _Categories.Find(item => item.ToString() == loaded_cate.ToString());
                if (categoryItem != null)
                {
                    list_category.Add(categoryItem);
                    GetData.GetParameters(_Doc, categoryItem, _ElementList);

                    foreach (ParameterData parameterData in categoryItem._Parameters)
                    {
                        parameterData._IndexExport = ParameterData._NotExport;
                    }

                    foreach (ParameterData loaded_para in loaded_cate._Parameters)
                    {
                        var parameterData = categoryItem._Parameters.Find(item => item.ToString() == loaded_para.ToString());
                        if (parameterData != null)
                            parameterData._IndexExport = loaded_para._IndexExport;
                    }
                }
            }

            foreach (CategoryItem categoryItem in _Categories)
            {
                var find = list_category.Find(item => item == categoryItem);
                if (find == null)
                    categoryItem._IsChecked = false;
                else
                    categoryItem._IsChecked = true;
            }

            clbCategories.SelectedItem = list_category.FirstOrDefault();
            RefreshListBoxs();
        }

        private void RefreshListBoxs()
        {
            List<CategoryItem> checkList = new List<CategoryItem>();
            for (int i = 0; i < clbCategories.Items.Count; i++)
            {
                object item = clbCategories.Items[i];
                var catetogyItem = item as CategoryItem;
                if (catetogyItem == null) continue;

                clbCategories.SetItemChecked(i, catetogyItem._IsChecked);
                if (catetogyItem._IsChecked)
                    checkList.Add(catetogyItem);
            }

            DisplayParameters(checkList);
            DisplayExports(checkList);
        }

        public Dictionary<CategoryItem, List<ParameterData>> GetExportData()
        {
            List<CategoryItem> export_categories = new List<CategoryItem>();
            foreach (TreeNode groupNode in tvExports.Nodes)
            {
                var categoryItem = groupNode.Tag as CategoryItem;
                if (categoryItem == null || categoryItem._IsChecked == false)
                    continue;
                export_categories.Add(categoryItem);
            }
            if (export_categories.Count == 0)
                return null;

            Dictionary<CategoryItem, List<ParameterData>> data = new Dictionary<CategoryItem, List<ParameterData>>();

            foreach (CategoryItem categoryItem in export_categories)
            {
                var export_parameters = categoryItem._Parameters.Where(item => item._IndexExport != ParameterData._NotExport).ToList();
                if (export_parameters.Count == 0) continue;

                export_parameters.Sort(delegate (ParameterData p1, ParameterData p2)
                {
                    return p1._IndexExport.CompareTo(p2._IndexExport);
                });

                if (data.ContainsKey(categoryItem) == false)
                    data.Add(categoryItem, new List<ParameterData>());

                foreach (ParameterData parameterData in export_parameters)
                {
                    data[categoryItem].Add(parameterData);
                }

                if (data[categoryItem].Count == 0)
                    data.Remove(categoryItem);
            }

            if (data.Count == 0) return null;
            return data;
        }

        private void FrmExportExcel_Load(object sender, EventArgs e) { DisplayCategories(); }

        private void clbCategories_SelectedIndexChanged(object sender, EventArgs e) { }

        private void clbCategories_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == -1) return;

            var item = clbCategories.Items[e.Index];
            if (item == null || item is CategoryItem == false) return;

            var categoryItem = item as CategoryItem;
            categoryItem._IsChecked = e.NewValue == CheckState.Checked ? true : false;

            if (categoryItem._IsChecked == true)
            {
                GetData.GetParameters(_Doc, categoryItem, _ElementList);
            }

            var found1s = tvParameters.Nodes.Cast<TreeNode>().Where(r => r.Text == categoryItem.ToString()).ToList();
            var found2s = tvExports.Nodes.Cast<TreeNode>().Where(r => r.Text == categoryItem.ToString()).ToList();

            if (categoryItem._IsChecked == true)
            {
                if (found1s.Count == 0) { DisplayParameter(categoryItem); }
                if (found2s.Count == 0) { DisplayExport(categoryItem); }
            }
            else
            {
                if (found1s.Count != 0) tvParameters.Nodes.Remove(found1s[0]);
                if (found2s.Count != 0) tvExports.Nodes.Remove(found2s[0]);
            }
        }

        private void btnSearchCategory_Click(object sender, EventArgs e) { Searching(clbCategories, txtSearchCategory); }
        private void btnSearchParameter_Click(object sender, EventArgs e) { Searching(tvParameters, txtSearchParameter); }
        private void btnSearchOutput_Click(object sender, EventArgs e) { Searching(tvExports, txtSearchExport); }
        private void btnExport_Click(object sender, EventArgs e) { ExportPrameter(tvParameters, tvExports, true); }
        private void btnUnExport_Click(object sender, EventArgs e) { ExportPrameter(tvExports, tvParameters, false); }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (tvExports.SelectedNode == null) return;
            var node = tvExports.SelectedNode;
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index > 0)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index - 1, node);
                    tvExports.SelectedNode = node;
                }
            }
            else if (node.TreeView.Nodes.Contains(node))
            {
                int index = view.Nodes.IndexOf(node);
                if (index > 0)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index - 1, node);
                    tvExports.SelectedNode = node;
                }
            }
            tvExports.Focus();
            ResetExportIndex();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (tvExports.SelectedNode == null) return;
            var node = tvExports.SelectedNode;
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index < parent.Nodes.Count - 1)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index + 1, node);
                    tvExports.SelectedNode = node;
                }
            }
            else if (view != null && view.Nodes.Contains(node))
            {
                int index = view.Nodes.IndexOf(node);
                if (index < view.Nodes.Count - 1)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index + 1, node);
                    tvExports.SelectedNode = node;
                }
            }
            tvExports.Focus();
            ResetExportIndex();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var config = Setting.GetDataConfig(_Categories);
            if (config == null)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDS_INFO_SELECT_PARAMETER_TO_EXPORT"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = Resources.Text.IDS_SAVE_SETTINGS_TITLE;
            saveFileDialog.Filter = Resources.Text.IDS_SETTINGS_FILE_FILTER;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.AutoUpgradeEnabled = true;

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            string fileName = saveFileDialog.FileName;
            Setting.SaveSettingFile(fileName, config);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Resources.Text.IDS_LOAD_SETTINGS_TITLE;
            openFileDialog.Filter = Resources.Text.IDS_SETTINGS_FILE_FILTER;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.AutoUpgradeEnabled = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            string fileName = openFileDialog.FileName;
            var loaded_categories = Setting.ReadSettingFile(_Doc, fileName);

            if (loaded_categories != null && loaded_categories.Count != 0)
            {
                SetData(loaded_categories);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            string value = null;
            if (Setting.LoadSetting(this.Name, Setting._ExportSetting, ref value) == false) return;

            var loaded_categories = Setting.ReadSettingText(_Doc, value);

            if (loaded_categories != null && loaded_categories.Count != 0)
            {
                SetData(loaded_categories);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (GetExportData() == null)
            {
                MessageBox.Show(_CmpAttribute.ResourceText("IDI_EXPORT_DATA_EMPTY"), _CmpAttribute.ResourceText("IDS_TXT_INFORMATION"));
                return;
            }

            try
            {
                var config = Setting.GetDataConfig(_Categories);
                if (config != null)
                    Setting.SaveSetting(this.Name, Setting._ExportSetting, config);
            }
            catch (Exception ex)
            {
                string errMsg = ex.Message;
            }

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; }

        private void tvParameters_DoubleClick(object sender, EventArgs e) { ExportPrameter(tvParameters, tvExports, true); }
        private void tvExports_DoubleClick(object sender, EventArgs e) { ExportPrameter(tvExports, tvParameters, false); }

        private void txtSearchCategory_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { Searching(clbCategories, txtSearchCategory); txtSearchCategory.Focus(); } }
        private void txtSearchParameter_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { Searching(tvParameters, txtSearchParameter); txtSearchParameter.Focus(); } }
        private void txtSearchExport_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { Searching(tvExports, txtSearchExport); txtSearchExport.Focus(); } }

        private void tvParameters_BeforeSelect(object sender, TreeViewCancelEventArgs e) { if (tvParameters.SelectedNode != null) { tvParameters.SelectedNode.BackColor = System.Drawing.SystemColors.Window; tvParameters.SelectedNode.ForeColor = System.Drawing.SystemColors.WindowText; } }
        private void tvParameters_AfterSelect(object sender, TreeViewEventArgs e) { tvParameters.SelectedNode.BackColor = System.Drawing.SystemColors.Highlight; tvParameters.SelectedNode.ForeColor = System.Drawing.SystemColors.HighlightText; }
        private void tvParameters_MouseDown(object sender, MouseEventArgs e) { var node = tvParameters.GetNodeAt(e.X, e.Y); if (node != null) { tvParameters.SelectedNode = node; } }
        private void tvExports_BeforeSelect(object sender, TreeViewCancelEventArgs e) { if (tvExports.SelectedNode != null) { tvExports.SelectedNode.BackColor = System.Drawing.SystemColors.Window; tvExports.SelectedNode.ForeColor = System.Drawing.SystemColors.WindowText; } }
        private void tvExports_MouseDown(object sender, MouseEventArgs e) { var node = tvExports.GetNodeAt(e.X, e.Y); if (node != null) { tvExports.SelectedNode = node; } }
        private void tvExports_AfterSelect(object sender, TreeViewEventArgs e) { tvExports.SelectedNode.BackColor = System.Drawing.SystemColors.Highlight; tvExports.SelectedNode.ForeColor = System.Drawing.SystemColors.HighlightText; }
    }
}
