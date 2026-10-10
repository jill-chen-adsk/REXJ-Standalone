using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ADSK.JExtRAC.ParameterFilter.Entities;
using ADSK.JExtRAC.ParameterFilter.Utils;
using RvtExtApp = ADSK.JExtRAC.ParameterFilter;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    public class ParameterGroupPairRow : INotifyPropertyChanged
    {
        private bool _isChecked1;
        private bool _isChecked2;

        public ObjectSelectGroup Group1 { get; set; }
        public ObjectSelectGroup Group2 { get; set; }

        public bool HasGroup1 => Group1 != null;
        public bool HasGroup2 => Group2 != null && !string.IsNullOrEmpty(GroupName2);

        public bool IsChecked1
        {
            get => _isChecked1;
            set
            {
                if (_isChecked1 == value)
                    return;
                _isChecked1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked1)));
            }
        }

        public bool IsChecked2
        {
            get => _isChecked2;
            set
            {
                if (_isChecked2 == value)
                    return;
                _isChecked2 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked2)));
            }
        }

        public string GroupName1 => Group1?.ParameterGroupVal ?? string.Empty;
        public string GroupName2 => Group2?.ParameterGroupVal ?? string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class FormParameterGroupWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly ObservableCollection<ParameterGroupPairRow> _rows = new();

        public List<ObjectSelectGroup> LstGroupAllProject { get; private set; }

        public FormParameterGroupWPF(RvtExtApp.Components.Attribute cmpAttribute, List<ObjectSelectGroup> lstGroupAllProject)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _cmpAttribute = cmpAttribute;
            LstGroupAllProject = lstGroupAllProject;

            string title = _cmpAttribute.ResourceText("IDS_TXT_PARAMETERGROUP");
            WeaveTheme.Apply(this, this, title, () => { DialogResult = false; Close(); });

            SetText();
            LoadRows();
            dgGroups.ItemsSource = _rows;
            UpdateSelectAllButtons();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        void SetText()
        {
            lblSection.Text = _cmpAttribute.ResourceText("IDS_TXT_DGVPARAMETERGROUP");
            btnCheckAll.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            btnUnCheck.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
            btnApply.Content = _cmpAttribute.ResourceText("IDS_TXT_PNTCHECK");
        }

        void LoadRows()
        {
            _rows.Clear();
            for (int i = 0; i < LstGroupAllProject.Count; i += 2)
            {
                var currentGroup = LstGroupAllProject[i];
                ObjectSelectGroup nextGroup = i + 1 < LstGroupAllProject.Count ? LstGroupAllProject[i + 1] : null;

                _rows.Add(new ParameterGroupPairRow
                {
                    Group1 = currentGroup,
                    Group2 = nextGroup,
                    IsChecked1 = currentGroup.IsSelected,
                    IsChecked2 = nextGroup?.IsSelected ?? false
                });
            }
        }

        void ParserData()
        {
            LstGroupAllProject.Clear();
            foreach (var row in _rows)
            {
                if (row.Group1 != null)
                {
                    row.Group1.IsSelected = row.IsChecked1;
                    LstGroupAllProject.Add(row.Group1);
                }

                if (row.Group2 != null && !string.IsNullOrEmpty(row.GroupName2))
                {
                    row.Group2.IsSelected = row.IsChecked2;
                    LstGroupAllProject.Add(row.Group2);
                }
            }
        }

        void UpdateSelectAllButtons()
        {
            int total = _rows.Sum(row =>
                (row.HasGroup1 ? 1 : 0) + (row.HasGroup2 ? 1 : 0));
            int selected = _rows.Sum(row =>
                (row.HasGroup1 && row.IsChecked1 ? 1 : 0) + (row.HasGroup2 && row.IsChecked2 ? 1 : 0));

            if (selected == 0)
            {
                btnCheckAll.IsEnabled = true;
                btnUnCheck.IsEnabled = false;
            }
            else if (selected == total)
            {
                btnCheckAll.IsEnabled = false;
                btnUnCheck.IsEnabled = true;
            }
            else
            {
                btnCheckAll.IsEnabled = true;
                btnUnCheck.IsEnabled = true;
            }
        }

        void RefreshButtonState() => UpdateSelectAllButtons();

        void GroupCheckBox_Click(object sender, RoutedEventArgs e) => RefreshButtonState();

        void BtnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in _rows)
            {
                if (row.HasGroup1)
                    row.IsChecked1 = true;
                if (row.HasGroup2)
                    row.IsChecked2 = true;
            }

            RefreshButtonState();
        }

        void BtnUnCheck_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in _rows)
            {
                if (row.HasGroup1)
                    row.IsChecked1 = false;
                if (row.HasGroup2)
                    row.IsChecked2 = false;
            }

            RefreshButtonState();
        }

        void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            ParserData();
            DialogResult = true;
            Close();
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
}
