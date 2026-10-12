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
    public partial class FormSettingDetailWPF : Window, IWeaveChromeWindow
    {
        private readonly ISwitchJoinHost _host;
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly CategoryItem _categoryItem;

        public ObservableCollection<PriorityListRow> AvailableRows { get; } = new ObservableCollection<PriorityListRow>();
        public ObservableCollection<PriorityListRow> PriorityRows { get; } = new ObservableCollection<PriorityListRow>();

        public FormSettingDetailWPF(
            ISwitchJoinHost host,
            Document doc,
            RvtExtApp.Components.Attribute cmpAttribute,
            CategoryItem categoryItem)
        {
            _host = host;
            _cmpAttribute = cmpAttribute;
            _categoryItem = categoryItem;

            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            listAvailable.ItemsSource = AvailableRows;
            listPriority.ItemsSource = PriorityRows;

            SetText();
            SetData();
            btnApply.IsEnabled = false;

            string title = _cmpAttribute.ResourceText("IDS_TXT_SWITCHJOINORDER")
                + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            WeaveTheme.Apply(this, this, title, Close);
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public WpfGrid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void SetText()
        {
            lbLeft.Text = _categoryItem._name + _cmpAttribute.ResourceText("IDS_TXT_LIST_FAMILY");
            lbRight.Text = _cmpAttribute.ResourceText("IDS_TXT_LBRIGHT");
            lbCount.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNT");
            lbPriorityHint.Text = _cmpAttribute.ResourceText("IDS_TXT_PRIORITY_HINT");
            btnRm.Content = _cmpAttribute.ResourceText("IDS_TXT_TEXTLEFT");
            btnAdd.Content = _cmpAttribute.ResourceText("IDS_TXT_TEXTRIGHT");
            btnUp.Content = "\u2191";
            btnDown.Content = "\u2193";
            btnUp.ToolTip = _cmpAttribute.ResourceText("IDS_TXT_TOOLTIP_PRIORITY_UP");
            btnDown.ToolTip = _cmpAttribute.ResourceText("IDS_TXT_TOOLTIP_PRIORITY_DOWN");
            btnApply.Content = _cmpAttribute.ResourceText("IDS_TXT_APPLY");
            btnClose.Content = _cmpAttribute.ResourceText("IDS_TXT_CLOSELOG");
        }

        private void SetData()
        {
            if (_categoryItem?._listFamilyItem == null)
                return;

            var listAllName = new List<string>();
            string setting = SwitchJoinSettingsStore.Get(
                SwitchJoinSettingsStore.FamilyPrioritySection, SwitchJoinSettingsStore.PriorityKey, "");
            if (!string.IsNullOrEmpty(setting))
                listAllName.AddRange(setting.Split(','));

            var familyItems = new List<FamilyItem>();
            foreach (var nameFamily in listAllName)
            {
                FamilyItem familyItem = _categoryItem.FindFamilyByName(nameFamily);
                if (familyItem == null)
                    continue;

                if (listAllName.Contains(familyItem._nameFami) && familyItem._nameCate == _categoryItem._name)
                {
                    familyItems.Add(familyItem);
                    AddPriorityRow(familyItem);
                }
            }

            foreach (var famiItem in _categoryItem._listFamilyItem)
            {
                if (famiItem._nameCate == _categoryItem._name && !familyItems.Contains(famiItem))
                    AddAvailableRow(famiItem);
            }
        }

        private void AddAvailableRow(FamilyItem familyItem)
        {
            AvailableRows.Add(new PriorityListRow
            {
                Tag = familyItem,
                Name = familyItem._nameFami,
                Count = string.Empty
            });
        }

        private void AddPriorityRow(FamilyItem familyItem)
        {
            PriorityRows.Add(new PriorityListRow
            {
                Tag = familyItem,
                Name = familyItem._nameFami,
                Count = "(" + familyItem._listElementIdOfFamily.Count + ")"
            });
        }

        private void MarkDirty()
        {
            btnApply.IsEnabled = true;
        }

        private void GetData()
        {
            _categoryItem._listElementId.Clear();
            _categoryItem._listFamilyItem.Clear();
            _categoryItem._isJoinFami = true;

            int j = 0;
            foreach (var row in PriorityRows)
            {
                if (row.Tag is not FamilyItem objFami)
                    continue;

                objFami._indexFami = j;
                _categoryItem._listFamilyItem.Add(objFami);
                if (objFami._listElementIdOfFamily != null)
                {
                    foreach (var eleId in objFami._listElementIdOfFamily)
                        _categoryItem._listElementId.Add(eleId);
                }
                j++;
            }

            foreach (var row in AvailableRows)
            {
                if (row.Tag is not FamilyItem objFami)
                    continue;

                objFami._indexFami = j;
                _categoryItem._listFamilyItem.Add(objFami);
                if (objFami._listElementIdOfFamily != null)
                {
                    foreach (var eleId in objFami._listElementIdOfFamily)
                        _categoryItem._listElementId.Add(eleId);
                }
                j++;
            }
        }

        private void SaveSetting()
        {
            string setting = SwitchJoinSettingsStore.Get(
                SwitchJoinSettingsStore.FamilyPrioritySection, SwitchJoinSettingsStore.PriorityKey, "");
            if (!string.IsNullOrEmpty(setting))
            {
                foreach (var item in setting.Split(','))
                {
                    if (!_host.ListValueSetting.Contains(item))
                        _host.ListValueSetting.Add(item);
                }
            }

            foreach (var row in PriorityRows)
            {
                if (row.Tag is not FamilyItem fami)
                    continue;

                string nameFami = fami._nameFami;
                if (string.IsNullOrEmpty(nameFami))
                    continue;

                if (_host.ListValueSetting.Contains(nameFami))
                    _host.ListValueSetting.Remove(nameFami);
                _host.ListValueSetting.Add(nameFami);
            }

            foreach (var row in AvailableRows)
            {
                if (row.Tag is not FamilyItem fami)
                    continue;

                string nameFami = fami._nameFami;
                if (!string.IsNullOrEmpty(nameFami) && _host.ListValueSetting.Contains(nameFami))
                    _host.ListValueSetting.Remove(nameFami);
            }

            string retSave = _host.ListValueSetting.Count > 0
                ? string.Join(",", _host.ListValueSetting)
                : string.Empty;
            SwitchJoinSettingsStore.Save(
                SwitchJoinSettingsStore.FamilyPrioritySection, SwitchJoinSettingsStore.PriorityKey, retSave);
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            int idx = listPriority.SelectedIndex;
            if (idx <= 0)
                return;

            _categoryItem.SwapFml(idx, idx - 1);
            PriorityRows.Move(idx, idx - 1);
            listPriority.SelectedIndex = idx - 1;
            MarkDirty();
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            int idx = listPriority.SelectedIndex;
            if (idx < 0 || idx >= PriorityRows.Count - 1)
                return;

            _categoryItem.SwapFml(idx + 1, idx);
            PriorityRows.Move(idx, idx + 1);
            listPriority.SelectedIndex = idx + 1;
            MarkDirty();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            foreach (PriorityListRow row in listAvailable.SelectedItems.Cast<PriorityListRow>().ToList())
            {
                AvailableRows.Remove(row);
                if (row.Tag is FamilyItem familyItem)
                    AddPriorityRow(familyItem);
            }

            MarkDirty();
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            foreach (PriorityListRow row in listPriority.SelectedItems.Cast<PriorityListRow>().ToList())
            {
                PriorityRows.Remove(row);
                if (row.Tag is FamilyItem familyItem)
                    AddAvailableRow(familyItem);
            }

            MarkDirty();
        }

        private void ListAvailable_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listAvailable.SelectedItem is PriorityListRow row)
            {
                AvailableRows.Remove(row);
                if (row.Tag is FamilyItem familyItem)
                    AddPriorityRow(familyItem);
                MarkDirty();
            }
        }

        private void ListPriority_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listPriority.SelectedItem is PriorityListRow row)
            {
                PriorityRows.Remove(row);
                if (row.Tag is FamilyItem familyItem)
                    AddAvailableRow(familyItem);
                MarkDirty();
            }
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            SaveSetting();
            GetData();
            btnApply.IsEnabled = false;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
