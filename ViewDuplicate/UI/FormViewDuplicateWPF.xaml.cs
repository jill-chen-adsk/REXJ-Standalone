using ADSK.ViewExtension.ViewDuplicate.DialogItem;
using ADSK.ViewExtension.ViewDuplicate.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static ADSK.ViewExtension.ViewDuplicate.Utils.DialogUtil;
using R = ADSK.ViewExtension.ViewDuplicate.Resources;
using RevitView = Autodesk.Revit.DB.View;

namespace ADSK.ViewExtension.ViewDuplicate.UI
{
    public partial class FormViewDuplicateWPF : Window, IWeaveChromeWindow
    {
        // Registry section and key names predate the WPF dialog; keep them so saved choices carry over.
        private const string SettingsSection = "DlgViewDuplicate";
        private const string KeyDiscipline = "CbxDicipline";
        private const string KeyViewType = "CbxViewType";
        private const string KeyViewFamilyType = "CbxViewFamilyType";
        private const string KeyViews = "LbxViews";
        private const string KeyDupMode = "CbxDupMode";

        private readonly Document _dbDoc;
        private readonly List<ElementId> _allViewIds = new List<ElementId>();
        private readonly List<RevitView> _allViewTemplates;
        private readonly ObservableCollection<ItmDuplicateRule> _rules = new ObservableCollection<ItmDuplicateRule>();
        private ViewType _currentViewType;

        public FormViewDuplicateWPF(ExternalCommandData cmdData)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");
            WeaveTheme.Apply(this, this, R.Text.CMD_VIEWDUPLICATE);
            WindowPositionMemory.Attach(this, SettingsSection);

            _dbDoc = cmdData.Application.ActiveUIDocument.Document;

            SetText();
            dgRules.ItemsSource = _rules;

            cbxDupMode.Items.Add(R.Text.DUP_MODE_DUPLICATE);
            cbxDupMode.Items.Add(R.Text.DUP_MODE_WITH_DETAILING);

            var disciplines = new List<ViewDiscipline>();
            foreach (RevitView view in new FilteredElementCollector(_dbDoc).OfClass(typeof(RevitView)).Cast<RevitView>())
            {
                if (view.IsTemplate
                    || !view.CanViewBeDuplicated(ViewDuplicateOption.Duplicate)
                    || !view.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing)
                    || !view.HasViewDiscipline())
                    continue;

                _allViewIds.Add(view.Id);
                if (!disciplines.Contains(view.Discipline))
                    disciplines.Add(view.Discipline);
            }

            _allViewTemplates = new FilteredElementCollector(_dbDoc)
                .OfClass(typeof(RevitView))
                .Cast<RevitView>()
                .Where(v => v.IsTemplate)
                .ToList();

            AddSorted(cbxDiscipline, disciplines.Select(d => new ItmViewDiscipline(d)));

            RestoreSelectedIndex(cbxDiscipline, SettingsSection, KeyDiscipline);
            RestoreSelectedIndex(cbxDupMode, SettingsSection, KeyDupMode);
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public System.Windows.Controls.Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private IntPtr Handle => new WindowInteropHelper(this).Handle;

        private void SetText()
        {
            lblDiscipline.Text = R.Text.LBL_DISCIPLINE;
            lblViewCategory.Text = R.Text.LBL_VIEW_CATEGORY;
            lblViewTypeFilter.Text = R.Text.LBL_VIEW_TYPE_FILTER;
            lblViews.Text = R.Text.LBL_VIEWS;
            lblDupMode.Text = R.Text.LBL_DUP_KIND;
            lblRules.Text = R.Text.LBL_RULES;
            colPosition.Header = R.Text.DGCOL_PREFIX_SUFFIX;
            colAddedText.Header = R.Text.DGCOL_ADDED_TEXT;
            colViewTemplate.Header = R.Text.DGCOL_VIEW_TEMPLATE;
            colViewType.Header = R.Text.DGCOL_VIEW_TYPE;
            btnAdd.Content = R.Text.BTN_ADD;
            btnDelete.Content = R.Text.BTN_DELETE;
            btnOK.Content = R.Text.BTN_OK;
            btnCancel.Content = R.Text.BTN_CANCEL;
        }

        private static void AddSorted(ItemsControl control, IEnumerable<object> items)
        {
            foreach (object item in items.OrderBy(i => i.ToString(), StringComparer.CurrentCultureIgnoreCase))
                control.Items.Add(item);
        }

        private List<RevitView> CollectViews(Func<RevitView, bool> predicate)
        {
            if (_allViewIds.Count == 0)
                return new List<RevitView>();

            return new FilteredElementCollector(_dbDoc, _allViewIds)
                .OfClass(typeof(RevitView))
                .Cast<RevitView>()
                .Where(predicate)
                .ToList();
        }

        private void CbxDiscipline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxViewType.Items.Clear();
            if (cbxDiscipline.SelectedItem is not ItmViewDiscipline disciplineItem)
                return;

            ViewDiscipline discipline = disciplineItem.Discipline;
            List<ViewType> viewTypes = CollectViews(v => v.Discipline == discipline)
                .Select(v => v.ViewType)
                .Distinct()
                .ToList();

            AddSorted(cbxViewType, viewTypes.Select(vt => new ItmViewType(vt)));
            RestoreSelectedIndex(cbxViewType, SettingsSection, KeyViewType);
        }

        private void CbxViewType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxViewFamilyType.Items.Clear();
            if (cbxViewType.SelectedItem is not ItmViewType viewTypeItem
                || cbxDiscipline.SelectedItem is not ItmViewDiscipline disciplineItem)
                return;

            _currentViewType = viewTypeItem.ViewType;
            ViewDiscipline discipline = disciplineItem.Discipline;

            var familyTypes = new List<object> { new ItmViewFamilyType() };
            var seen = new HashSet<ElementId>();
            foreach (RevitView view in CollectViews(v => v.Discipline == discipline && v.ViewType == _currentViewType))
            {
                if (_dbDoc.GetElement(view.GetTypeId()) is ViewFamilyType vft && seen.Add(vft.Id))
                    familyTypes.Add(new ItmViewFamilyType(vft));
            }

            AddSorted(cbxViewFamilyType, familyTypes);
            RestoreSelectedIndex(cbxViewFamilyType, SettingsSection, KeyViewFamilyType);
        }

        private void CbxViewFamilyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbxViews.Items.Clear();
            if (cbxViewType.SelectedItem is not ItmViewType viewTypeItem
                || cbxDiscipline.SelectedItem is not ItmViewDiscipline disciplineItem
                || cbxViewFamilyType.SelectedItem is not ItmViewFamilyType vftItem)
                return;

            ViewType viewType = viewTypeItem.ViewType;
            ViewDiscipline discipline = disciplineItem.Discipline;
            ViewFamilyType vft = vftItem.ViewfamilyType;

            List<RevitView> views = CollectViews(v =>
                v.ViewType == viewType
                && v.Discipline == discipline
                && (vft == null || v.GetTypeId().Equals(vft.Id)));

            AddSorted(lbxViews, views.Select(v => new ItmView(v)));
            RestoreSelectedIndex(lbxViews, SettingsSection, KeyViews);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ViewFamily viewFamily = cbxViewFamilyType.Items
                .OfType<ItmViewFamilyType>()
                .Select(i => i.ViewfamilyType)
                .FirstOrDefault(vft => vft != null)
                ?.ViewFamily ?? ViewFamily.Invalid;

            var dialog = new FormViewDuplicateItemWPF(_dbDoc, viewFamily, _currentViewType, _allViewTemplates)
            {
                Owner = this,
            };
            if (dialog.ShowDialog() == true && dialog.Rule != null)
                _rules.Add(dialog.Rule);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgRules.SelectedItems.OfType<ItmDuplicateRule>().ToList();
            if (selected.Count == 0)
            {
                ShowMessage(R.Text.ERR_SELECTROW);
                return;
            }

            foreach (ItmDuplicateRule rule in selected)
                _rules.Remove(rule);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            dgRules.CommitEdit(DataGridEditingUnit.Row, true);

            SaveSelectedIndex(cbxDiscipline, SettingsSection, KeyDiscipline);
            SaveSelectedIndex(cbxViewType, SettingsSection, KeyViewType);
            SaveSelectedIndex(cbxViewFamilyType, SettingsSection, KeyViewFamilyType);
            SaveSelectedIndex(lbxViews, SettingsSection, KeyViews);
            SaveSelectedIndex(cbxDupMode, SettingsSection, KeyDupMode);

            List<RevitView> targetViews = lbxViews.SelectedItems.OfType<ItmView>().Select(i => i.View).ToList();
            if (targetViews.Count == 0)
            {
                ShowMessage(R.Text.ERR_SELECTTARGETVIEW);
                return;
            }
            if (_rules.Count == 0)
            {
                ShowMessage(R.Text.ERR_INPUTCOPYRULE);
                return;
            }

            var errors = new List<string>();
            if (_rules.Any(r => string.IsNullOrEmpty(r.AddedText)))
                errors.Add(string.Format(R.Text.ERR_FIELD_EMPTY, R.Text.DGCOL_ADDED_TEXT));

            foreach (RevitView targetView in targetViews)
            {
                var targetVft = (ViewFamilyType)_dbDoc.GetElement(targetView.GetTypeId());
                foreach (ItmDuplicateRule rule in _rules)
                {
                    if (targetVft.ViewFamily != rule.ViewFamilyTypeItem.ViewfamilyType.ViewFamily)
                        errors.Add(string.Format(R.Text.ERR_CANNOTCOPY, targetView.Name, rule.ViewFamilyTypeText));
                }
            }

            if (errors.Count > 0)
            {
                ShowMessage(R.Text.TXT_CANNOTCOPYVIEW + Environment.NewLine + string.Join(Environment.NewLine, errors));
                return;
            }

            ViewDuplicateOption dupMode = cbxDupMode.SelectedIndex == 1
                ? ViewDuplicateOption.WithDetailing
                : ViewDuplicateOption.Duplicate;

            var namesByViewType = new Dictionary<ViewType, HashSet<string>>();
            foreach (RevitView view in new FilteredElementCollector(_dbDoc).OfClass(typeof(RevitView)).Cast<RevitView>())
            {
                if (!namesByViewType.TryGetValue(view.ViewType, out HashSet<string> names))
                {
                    names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    namesByViewType.Add(view.ViewType, names);
                }
                names.Add(view.Name);
            }

            var failedViews = new List<string>();
            foreach (RevitView targetView in targetViews)
            {
                if (!namesByViewType.TryGetValue(targetView.ViewType, out HashSet<string> existingNames))
                {
                    existingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    namesByViewType.Add(targetView.ViewType, existingNames);
                }

                foreach (ItmDuplicateRule rule in _rules)
                {
                    if (!TryDuplicate(targetView, rule, dupMode, existingNames))
                        failedViews.Add(targetView.Name);
                }
            }

            if (failedViews.Count > 0)
                ShowMessage(R.Text.TXT_CANNOTCOPYVIEW + Environment.NewLine + string.Join(Environment.NewLine, failedViews.Distinct()));

            DialogResult = true;
        }

        private bool TryDuplicate(RevitView targetView, ItmDuplicateRule rule, ViewDuplicateOption dupMode, HashSet<string> existingNames)
        {
            using var subTransaction = new SubTransaction(_dbDoc);
            subTransaction.Start();
            try
            {
                ViewFamilyType vft = rule.ViewFamilyTypeItem.ViewfamilyType;
                var newView = (RevitView)_dbDoc.GetElement(targetView.Duplicate(dupMode));

                string baseName = rule.IsPrefix
                    ? rule.AddedText + targetView.Name
                    : targetView.Name + rule.AddedText;
                string newName = NewSafeName(baseName, existingNames);
                newView.Name = newName;
                newView.ChangeTypeId(vft.Id);

                Parameter assignOnCreation = vft.get_Parameter(BuiltInParameter.ASSIGN_TEMPLATE_ON_VIEW_CREATION);
                bool typeAssignsTemplate = assignOnCreation != null && assignOnCreation.AsInteger() != 0;
                if (!typeAssignsTemplate && rule.ViewTemplateItem != null)
                    newView.ViewTemplateId = rule.ViewTemplateItem.View.Id;

                subTransaction.Commit();
                existingNames.Add(newName);
                return true;
            }
            catch (Exception)
            {
                subTransaction.RollBack();
                return false;
            }
        }

        private static string NewSafeName(string newName, HashSet<string> existingNames)
        {
            if (!existingNames.Contains(newName))
                return newName;

            int i = 0;
            string candidate;
            do
            {
                i += 1;
                candidate = string.Concat(newName, R.Text.TXT_LEFTBRACE, i, R.Text.TXT_RIGHTBRACE);
            }
            while (existingNames.Contains(candidate));

            return candidate;
        }

        private void ShowMessage(string message) =>
            WeaveDialogHost.ShowMessage(Handle, message, R.Text.CMD_VIEWDUPLICATE);
    }
}
