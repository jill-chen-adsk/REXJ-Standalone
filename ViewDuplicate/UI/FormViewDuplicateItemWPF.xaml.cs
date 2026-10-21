using ADSK.ViewExtension.ViewDuplicate.DialogItem;
using ADSK.ViewExtension.ViewDuplicate.Utils;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static ADSK.ViewExtension.ViewDuplicate.Utils.DialogUtil;
using R = ADSK.ViewExtension.ViewDuplicate.Resources;
using RevitView = Autodesk.Revit.DB.View;

namespace ADSK.ViewExtension.ViewDuplicate.UI
{
    public partial class FormViewDuplicateItemWPF : Window, IWeaveChromeWindow
    {
        private const string SettingsSection = "DlgViewDuplicateItem";
        private const string KeyViewFamilyType = "CbxViewFamilyType";
        private const string KeyPrefix = "RbnPrefix";

        private readonly Document _dbDoc;
        private readonly ViewType _currentViewType;
        private readonly List<RevitView> _viewTemplates;

        public FormViewDuplicateItemWPF(Document dbDoc, ViewFamily viewFamily, ViewType currentViewType, List<RevitView> viewTemplates)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");
            WeaveTheme.Apply(this, this, R.Text.CMD_VIEWDUPLICATE);
            WindowPositionMemory.Attach(this, SettingsSection);

            _dbDoc = dbDoc;
            _currentViewType = currentViewType;
            _viewTemplates = viewTemplates ?? new List<RevitView>();

            SetText();

            IEnumerable<ItmViewFamilyType> familyTypes = new FilteredElementCollector(_dbDoc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .Where(vft => vft.ViewFamily == viewFamily)
                .Select(vft => new ItmViewFamilyType(vft))
                .OrderBy(i => i.ToString(), StringComparer.CurrentCultureIgnoreCase);
            foreach (ItmViewFamilyType item in familyTypes)
                cbxViewFamilyType.Items.Add(item);

            RestoreSelectedIndex(cbxViewFamilyType, SettingsSection, KeyViewFamilyType);

            bool isPrefix = GetBool(SettingsSection, KeyPrefix, true);
            rbnPrefix.IsChecked = isPrefix;
            rbnSuffix.IsChecked = !isPrefix;
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public System.Windows.Controls.Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        public ItmDuplicateRule Rule { get; private set; }

        private void SetText()
        {
            lblViewFamilyType.Text = R.Text.LBL_ITEM_VIEW_TYPE;
            lblViewTemplate.Text = R.Text.LBL_ITEM_VIEW_TEMPLATE;
            lblAddText.Text = R.Text.GP_ADD_STRING;
            rbnPrefix.Content = R.Text.TXT_PREFIX;
            rbnSuffix.Content = R.Text.TXT_SUFFIX;
            btnOK.Content = R.Text.BTN_OK;
            btnCancel.Content = R.Text.BTN_CANCEL;
        }

        private void CbxViewFamilyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxViewTemplate.Items.Clear();
            if (cbxViewFamilyType.SelectedItem is not ItmViewFamilyType vftItem)
                return;

            ElementId defaultTemplateId = vftItem.AppliedViewTemplateId;
            if (defaultTemplateId.Equals(ElementId.InvalidElementId))
            {
                IEnumerable<ItmView> templates = _viewTemplates
                    .Select(vt => new ItmView(vt))
                    .Where(i => i.ViewType == _currentViewType)
                    .OrderBy(i => i.ToString(), StringComparer.CurrentCultureIgnoreCase);
                foreach (ItmView item in templates)
                    cbxViewTemplate.Items.Add(item);
                cbxViewTemplate.IsEnabled = true;
            }
            else
            {
                cbxViewTemplate.Items.Add(new ItmView((RevitView)_dbDoc.GetElement(defaultTemplateId)));
                cbxViewTemplate.IsEnabled = false;
            }

            if (cbxViewTemplate.Items.Count > 0)
                cbxViewTemplate.SelectedIndex = 0;
        }

        private void CbxViewTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbxAddText.Text = cbxViewTemplate.SelectedItem?.ToString() ?? string.Empty;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbxViewFamilyType.SelectedItem is not ItmViewFamilyType vftItem)
            {
                ShowMessage(string.Format(R.Text.ERR_FIELD_EMPTY, R.Text.LBL_ITEM_VIEW_TYPE));
                return;
            }
            if (string.IsNullOrEmpty(tbxAddText.Text))
            {
                ShowMessage(string.Format(R.Text.ERR_FIELD_EMPTY, R.Text.GP_ADD_STRING));
                return;
            }

            bool isPrefix = rbnPrefix.IsChecked == true;
            SaveSelectedIndex(cbxViewFamilyType, SettingsSection, KeyViewFamilyType);
            SaveBool(SettingsSection, KeyPrefix, isPrefix);

            Rule = new ItmDuplicateRule(isPrefix, tbxAddText.Text, cbxViewTemplate.SelectedItem as ItmView, vftItem);
            DialogResult = true;
        }

        private void ShowMessage(string message) =>
            WeaveDialogHost.ShowMessage(new WindowInteropHelper(this).Handle, message, R.Text.CMD_VIEWDUPLICATE);
    }
}
