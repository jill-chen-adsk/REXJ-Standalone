using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ADSK.JExtRAC.ParameterFilter.Components;
using ADSK.JExtRAC.ParameterFilter.Entities;
using ADSK.JExtRAC.ParameterFilter.Utils;
using Autodesk.Revit.UI;
using RvtExtApp = ADSK.JExtRAC.ParameterFilter;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    public partial class FormParameterFilterWPF : Window, IWeaveChromeWindow
    {
        private readonly ParameterFilterEngine _engine;
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private bool _suppressTabChange;

        public FormParameterFilterWPF(
            UIDocument rvtUIDoc,
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Components.Elements cmpElements,
            List<ObjectElement> objectElements,
            IntPtr ownerHandle)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _cmpAttribute = cmpAttribute;
            _engine = new ParameterFilterEngine(rvtUIDoc, cmpAttribute, cmpElements, objectElements)
            {
                OwnerHandle = ownerHandle
            };

            string title = cmpAttribute.ResourceText("IDS_TXT_FILTERFORM");
            WeaveTheme.Apply(this, this, title, CancelDialog);

            SetText();
            BindGrids();

            _engine.TabChanged += (_, __) => SyncFooterState();
            _engine.StateChanged += (_, __) => RefreshAllTabState();

            _engine.CompleteInitialLoad();
            tabParameterFilter.SelectedIndex = 0;
            RefreshAllTabState();
            SyncFooterState();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        void SetText()
        {
            btnSettingParameterGroup.Content = _cmpAttribute.ResourceText("IDS_TXT_SETTINGPARAMETERGROUP");
            btnNext.Content = _cmpAttribute.ResourceText("IDS_TXT_NEXT");
            btnPrevious.Content = _cmpAttribute.ResourceText("IDS_TXT_PREVIOUS");
            cbkSelectConnect.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTCONNECT");
            tabPageCategory.Header = _cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            lblCountTypeCategory.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            lblCountObjCate.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            btnSelectAllCategory.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            btnSelectClearCategory.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            btnPreview.Content = _cmpAttribute.ResourceText("IDS_TXT_PNTCHECK");
            btnOk.Content = _cmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
            tabPageFamily.Header = _cmpAttribute.ResourceText("IDS_TXT_FAMILY");
            lblCountTypeFamily.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            lblCountObjectFamily.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            btnSelectAllFamily.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            btnSelectClearFamily.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            tabPageFamilyType.Header = _cmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            lblCountTypeFamilyType.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            lblCountObjectFamilyType.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            btnSelectAllFamilyType.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            btnSelectClearFamilyType.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            tabPageParameter.Header = _cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
            lblCountTypeParameter.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            lblCountObjectTypeParameter.Text = _cmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            btnSelectAllParameter.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            btnSelectClearParameter.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");

            colCategoryName.Header = _cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            colCategoryCount.Header = _cmpAttribute.ResourceText("IDS_TXT_COUNT");
            colFamilyCategory.Header = _cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            colFamilyName.Header = _cmpAttribute.ResourceText("IDS_TXT_FAMILY");
            colFamilyCount.Header = _cmpAttribute.ResourceText("IDS_TXT_COUNT");
            colFtCategory.Header = _cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            colFtName.Header = _cmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            colFtCount.Header = _cmpAttribute.ResourceText("IDS_TXT_COUNT");
            colParamCategory.Header = _cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            colParamFamilyType.Header = _cmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            colParamName.Header = _cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
            colParamValue.Header = _cmpAttribute.ResourceText("IDS_TXT_VALUEPARAMETER");
            colParamMin.Header = _cmpAttribute.ResourceText("IDS_TXT_VALUEPARAMETERMIN");
            colParamMax.Header = _cmpAttribute.ResourceText("IDS_TXT_VALUEPARAMETERMAX");
            colParamCount.Header = _cmpAttribute.ResourceText("IDS_TXT_COUNT");
        }

        void BindGrids()
        {
            dgvCategory.ItemsSource = _engine.CategoryItems;
            dgvFamily.ItemsSource = _engine.FamilyItems;
            dgvFamilyType.ItemsSource = _engine.FamilyTypeItems;
            dgvParameter.ItemsSource = _engine.ParameterRows;

            cbkSelectConnect.Checked += (_, __) => _engine.SelectConnectChecked = true;
            cbkSelectConnect.Unchecked += (_, __) => _engine.SelectConnectChecked = false;
        }

        void RefreshAllTabState()
        {
            ApplyTabState(_engine.GetCategoryTabState(), lblCounterCategory, lblObjCounterCategory, btnSelectAllCategory, btnSelectClearCategory);
            ApplyTabState(_engine.GetFamilyTabState(), lblTypeCounterFamily, lblObjectCounterFamily, btnSelectAllFamily, btnSelectClearFamily);
            ApplyTabState(_engine.GetFamilyTypeTabState(), lblTypeCounterFamilyType, lblObjectCounterFamilyType, btnSelectAllFamilyType, btnSelectClearFamilyType);
            ApplyTabState(_engine.GetParameterTabState(), lblTypeCounterTypeParameter, lblObjectCounterTypeParameter, btnSelectAllParameter, btnSelectClearParameter);
        }

        static void ApplyTabState(ParameterFilterTabState state, TextBlock types, TextBlock objects, Button selectAll, Button clear)
        {
            types.Text = state.SelectedTypes.ToString();
            objects.Text = state.SelectedObjects.ToString();
            selectAll.IsEnabled = state.SelectAllEnabled;
            clear.IsEnabled = state.ClearEnabled;
        }

        void SyncFooterState()
        {
            btnPrevious.IsEnabled = _engine.CanGoPrevious;
            btnNext.IsEnabled = _engine.CanGoNext;

            _suppressTabChange = true;
            tabParameterFilter.SelectedIndex = _engine.CurrentTabIndex;
            _suppressTabChange = false;
        }

        void TabParameterFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is TabItem tabItem && tabItem != tabParameterFilter.SelectedItem)
                e.Handled = true;
        }

        void TabParameterFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressTabChange)
                return;
            if (tabParameterFilter.SelectedIndex != _engine.CurrentTabIndex)
                tabParameterFilter.SelectedIndex = _engine.CurrentTabIndex;
        }

        void FilterRowCheckBox_Click(object sender, RoutedEventArgs e)
        {
            int tab = tabParameterFilter.SelectedIndex;
            if (tab == 0)
                _engine.OnCategoryCheckChanged();
            else if (tab == 1)
                _engine.OnFamilyCheckChanged();
            else if (tab == 2)
                _engine.OnFamilyTypeCheckChanged();
        }

        void ParameterRowCheckBox_Click(object sender, RoutedEventArgs e) => _engine.OnParameterCheckChanged();

        void BtnSelectAllCategory_Click(object sender, RoutedEventArgs e) => _engine.SelectAllCategory();
        void BtnSelectClearCategory_Click(object sender, RoutedEventArgs e) => _engine.ClearCategory();
        void BtnSelectAllFamily_Click(object sender, RoutedEventArgs e) => _engine.SelectAllFamily();
        void BtnSelectClearFamily_Click(object sender, RoutedEventArgs e) => _engine.ClearFamily();
        void BtnSelectAllFamilyType_Click(object sender, RoutedEventArgs e) => _engine.SelectAllFamilyType();
        void BtnSelectClearFamilyType_Click(object sender, RoutedEventArgs e) => _engine.ClearFamilyType();
        void BtnSelectAllParameter_Click(object sender, RoutedEventArgs e) => _engine.SelectAllParameter();
        void BtnSelectClearParameter_Click(object sender, RoutedEventArgs e) => _engine.ClearParameter();

        void BtnSettingParameterGroup_Click(object sender, RoutedEventArgs e) => _engine.OpenParameterGroupDialog(this);

        void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _engine.GoPrevious();
            SyncFooterState();
            RefreshAllTabState();
        }

        void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            _engine.GoNext();
            SyncFooterState();
            RefreshAllTabState();
        }

        void BtnPreview_Click(object sender, RoutedEventArgs e) => _engine.UpdateSelection();

        void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_engine.UpdateSelection())
                DialogResult = true;
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e) => CancelDialog();

        void CancelDialog()
        {
            DialogResult = false;
            Close();
        }
    }
}
