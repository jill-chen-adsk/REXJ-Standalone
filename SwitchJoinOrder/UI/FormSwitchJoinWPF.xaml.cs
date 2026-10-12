using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WpfGrid = System.Windows.Controls.Grid;
using ADSK.JExtRAC.SwitchJoinOrder.Entities;
using ADSK.JExtRAC.SwitchJoinOrder.Utils;
using Autodesk.Revit.DB;
using RvtExtApp = ADSK.JExtRAC.SwitchJoinOrder;

namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    public partial class FormSwitchJoinWPF : Window, IWeaveChromeWindow, ISwitchJoinHost
    {
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly Document _doc;
        private readonly CategoryItems _groupCategory;
        private bool _cbkChecked;

        public ObservableCollection<PriorityListRow> AvailableRows { get; } = new ObservableCollection<PriorityListRow>();
        public ObservableCollection<PriorityListRow> PriorityRows { get; } = new ObservableCollection<PriorityListRow>();

        public List<string> ListValueSetting { get; } = new List<string>();

        public FormSwitchJoinWPF(
            Document doc,
            RvtExtApp.Components.Attribute cmpAttribute,
            CategoryItems groupCategory)
        {
            _doc = doc;
            _cmpAttribute = cmpAttribute;
            _groupCategory = groupCategory;

            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            listAvailable.ItemsSource = AvailableRows;
            listPriority.ItemsSource = PriorityRows;

            SetText();
            SetData();
            UpdateDetailsEnabled();

            string title = _cmpAttribute.ResourceText("IDS_TXT_SWITCHJOINORDER")
                + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            WeaveTheme.Apply(this, this, title, () =>
            {
                DialogResult = false;
                Close();
            });
        }

        public bool GetChecked => _cbkChecked;

        public Border ChromeOuterBorder => chromeOuterBorder;
        public WpfGrid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        public void OpenFamilyDetail(CategoryItem categoryItem)
        {
            var detail = new FormSettingDetailWPF(this, _doc, _cmpAttribute, categoryItem)
            {
                Owner = this
            };
            WeaveDialogHost.ShowDialog(detail);
        }

        private void SetText()
        {
            lbLeft.Text = _cmpAttribute.ResourceText("IDS_TXT_LBLEFT");
            lbRight.Text = _cmpAttribute.ResourceText("IDS_TXT_LBRIGHT");
            lbCount.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNT");
            lbPriorityHint.Text = _cmpAttribute.ResourceText("IDS_TXT_PRIORITY_HINT");
            btnRm.Content = _cmpAttribute.ResourceText("IDS_TXT_TEXTLEFT");
            btnAdd.Content = _cmpAttribute.ResourceText("IDS_TXT_TEXTRIGHT");
            btnUp.Content = "\u2191";
            btnDown.Content = "\u2193";
            btnUp.ToolTip = _cmpAttribute.ResourceText("IDS_TXT_TOOLTIP_PRIORITY_UP");
            btnDown.ToolTip = _cmpAttribute.ResourceText("IDS_TXT_TOOLTIP_PRIORITY_DOWN");
            btnOK.Content = _cmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
            btnDetails.Content = _cmpAttribute.ResourceText("IDS_TXT_DETAILS");
            ckbGroup.Content = _cmpAttribute.ResourceText("IDS_CK");
        }

        private void SetData()
        {
            if (_groupCategory._categoryShow == null)
                return;

            var listAllName = new List<string>();
            string setting = SwitchJoinSettingsStore.Get(
                SwitchJoinSettingsStore.CategoryPrioritySection, SwitchJoinSettingsStore.PriorityKey, "");
            if (!string.IsNullOrEmpty(setting))
            {
                string[] strArray = setting.Split(',');
                listAllName.AddRange(strArray);
                if (strArray.Length >= 2)
                {
                    foreach (string nameCtg in strArray)
                    {
                        if (!_groupCategory.Contains(nameCtg))
                            continue;

                        int index = _groupCategory.Find(nameCtg);
                        if (index == -1)
                            continue;

                        if (_groupCategory._categoryShow.Count > 1)
                            _groupCategory.Add(_groupCategory.Remove(index));
                        else
                            _groupCategory.Add(_groupCategory.FindByName(nameCtg));
                    }
                }
            }

            foreach (var category in _groupCategory._categoryShow)
            {
                if (listAllName.Contains(category._name))
                    AddPriorityRow(category, true);
                else
                    AddAvailableRow(category);
            }
        }

        private static PriorityListRow RowFromCategory(CategoryItem category, bool withCount)
        {
            return new PriorityListRow
            {
                Tag = category,
                Name = category._name,
                Count = withCount ? "(" + (category._listElementId?.Count ?? 0) + ")" : string.Empty
            };
        }

        private void AddAvailableRow(CategoryItem category)
        {
            AvailableRows.Add(RowFromCategory(category, false));
        }

        private void AddPriorityRow(CategoryItem category, bool withCount)
        {
            PriorityRows.Add(RowFromCategory(category, withCount));
        }

        private void GetData()
        {
            _cbkChecked = ckbGroup.IsChecked == true;
            _groupCategory.ClearCtg();
            foreach (var row in PriorityRows)
            {
                if (row.Tag is CategoryItem objCata)
                    _groupCategory.Add(objCata);
            }
        }

        private CategoryItem SelectedPriorityCategory()
        {
            if (listPriority.SelectedItem is PriorityListRow row && row.Tag is CategoryItem category)
                return category;
            return null;
        }

        private void UpdateDetailsEnabled()
        {
            var category = SelectedPriorityCategory();
            btnDetails.IsEnabled = category?._listFamilyItem != null && category._listFamilyItem.Count > 0;
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            int idx = listPriority.SelectedIndex;
            if (idx <= 0)
                return;

            _groupCategory.SwapCtg(idx, idx - 1);
            PriorityRows.Move(idx, idx - 1);
            listPriority.SelectedIndex = idx - 1;
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            int idx = listPriority.SelectedIndex;
            if (idx < 0 || idx >= PriorityRows.Count - 1)
                return;

            _groupCategory.SwapCtg(idx + 1, idx);
            PriorityRows.Move(idx, idx + 1);
            listPriority.SelectedIndex = idx + 1;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            foreach (PriorityListRow row in listAvailable.SelectedItems.Cast<PriorityListRow>().ToList())
            {
                AvailableRows.Remove(row);
                if (row.Tag is CategoryItem category)
                    AddPriorityRow(category, true);
            }

            UpdateDetailsEnabled();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            foreach (PriorityListRow row in listPriority.SelectedItems.Cast<PriorityListRow>().ToList())
            {
                PriorityRows.Remove(row);
                if (row.Tag is CategoryItem category)
                    AddAvailableRow(category);
            }

            UpdateDetailsEnabled();
        }

        private void ListAvailable_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listAvailable.SelectedItem is PriorityListRow row)
            {
                AvailableRows.Remove(row);
                if (row.Tag is CategoryItem category)
                    AddPriorityRow(category, true);
                UpdateDetailsEnabled();
            }
        }

        private void ListPriority_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listPriority.SelectedItem is PriorityListRow row)
            {
                PriorityRows.Remove(row);
                if (row.Tag is CategoryItem category)
                    AddAvailableRow(category);
                UpdateDetailsEnabled();
            }
        }

        private void ListPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDetailsEnabled();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string retSave = string.Join(",", PriorityRows.Select(r => (r.Tag as CategoryItem)?._name).Where(n => !string.IsNullOrEmpty(n)));
            if (!string.IsNullOrEmpty(retSave))
                SwitchJoinSettingsStore.Save(
                    SwitchJoinSettingsStore.CategoryPrioritySection, SwitchJoinSettingsStore.PriorityKey, retSave);

            GetData();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var category = SelectedPriorityCategory();
            if (category != null)
                OpenFamilyDetail(category);
        }
    }
}
