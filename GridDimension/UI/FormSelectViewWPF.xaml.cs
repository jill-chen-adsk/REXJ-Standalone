using ADSK.JExtRAC.GridDimension.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;

namespace ADSK.JExtRAC.GridDimension.UI
{
    public partial class FormSelectViewWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly RvtExtApp.Entities.DtCmd _EntDtCmd;
        private readonly List<Revit.DB.View> _ViewList;
        private readonly Revit.DB.View _CurrentView;
        private readonly List<ViewSelectionItem> _items = new List<ViewSelectionItem>();
        private int _lastIndex;
        private bool _ignoreSelectAllEvents;

        public FormSelectViewWPF(
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Entities.DtCmd entDtCmd,
            List<Revit.DB.View> viewList,
            Revit.DB.View currentView)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _CmpAttribute = cmpAttribute;
            _EntDtCmd = entDtCmd;
            _ViewList = viewList;
            _CurrentView = currentView;

            string title = _CmpAttribute.ResourceText("IDS_TXT_SELECTVIEWS")
                + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            WeaveTheme.Apply(this, this, title);

            SetText();
            SetData();
            SetIcon();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(
                this,
                this,
                _CmpAttribute.ResourceText("IDS_TXT_SELECTVIEWS")
                    + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version));

            btnOk.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            cbkSelecAll.Content = _CmpAttribute.ResourceText("IDS_TXT_SELECTALLVIEW");
        }

        private void SetIcon()
        {
            if (_CmpAttribute.ResourceImage("IDI_SUBS_ICON") is not Icon icon)
                return;

            Icon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void SetData()
        {
            if (_ViewList == null || _ViewList.Count == 0)
                return;

            foreach (Revit.DB.View view in _ViewList)
            {
                _items.Add(new ViewSelectionItem
                {
                    View = view,
                    Title = view.Title,
                    IsChecked = view.Id == _CurrentView.Id
                });
            }

            lvViews.ItemsSource = _items;

            if (_ViewList.Count == 1)
                cbkSelecAll.IsChecked = true;
            else
                cbkSelecAll.IsChecked = null;
        }

        private void GetData()
        {
            _ViewList.Clear();

            foreach (ViewSelectionItem item in _items.Where(x => x.IsChecked))
            {
                if (item.View.Id == _CurrentView.Id)
                    _ViewList.Insert(0, item.View);
                else
                    _ViewList.Add(item.View);
            }
        }

        private void UpdateSelectAllState()
        {
            _ignoreSelectAllEvents = true;
            try
            {
                int countItem = _items.Count;
                int checkedItem = _items.Count(x => x.IsChecked);

                if (checkedItem != 0 && checkedItem != countItem)
                    cbkSelecAll.IsChecked = null;
                else if (checkedItem == countItem)
                    cbkSelecAll.IsChecked = true;
                else
                    cbkSelecAll.IsChecked = false;
            }
            finally
            {
                _ignoreSelectAllEvents = false;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            GetData();

            if (_ViewList.Count == 0)
            {
                WeaveDialogHost.ShowMessage(
                    new WindowInteropHelper(this).Handle,
                    _CmpAttribute.ResourceText("IDS_TXT_NOVIEWSELECT"),
                    _CmpAttribute.ResourceText("IDS_TXT_ERROR"),
                    _CmpAttribute.ResourceText("IDS_TXT_OK"));
                return;
            }

            DialogResult = true;
            Close();
        }

        private void LvViews_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Shift)
                return;

            ListViewItem listItem = FindListViewItem(e.OriginalSource as DependencyObject);
            if (listItem == null)
                return;

            int currentIndex = lvViews.ItemContainerGenerator.IndexFromContainer(listItem);
            if (currentIndex < 0)
                return;

            bool targetChecked = !_items[currentIndex].IsChecked;
            int upper = Math.Max(_lastIndex, currentIndex);
            int lower = Math.Min(_lastIndex, currentIndex);

            for (int i = lower; i <= upper; i++)
                _items[i].IsChecked = targetChecked;

            _lastIndex = currentIndex;
            UpdateSelectAllState();
            e.Handled = true;
        }

        private static ListViewItem FindListViewItem(DependencyObject source)
        {
            while (source != null)
            {
                if (source is ListViewItem item)
                    return item;

                source = VisualTreeHelper.GetParent(source);
            }

            return null;
        }

        private void ViewCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
                return;

            if (checkBox.DataContext is not ViewSelectionItem item)
                return;

            int currentIndex = _items.IndexOf(item);
            if (currentIndex >= 0)
                _lastIndex = currentIndex;

            UpdateSelectAllState();
        }

        private void CbkSelecAll_Changed(object sender, RoutedEventArgs e)
        {
            if (_ignoreSelectAllEvents || cbkSelecAll.IsChecked == null)
                return;

            bool isChecked = cbkSelecAll.IsChecked == true;
            foreach (ViewSelectionItem item in _items)
                item.IsChecked = isChecked;
        }
    }

    internal sealed class ViewSelectionItem : INotifyPropertyChanged
    {
        private bool _isChecked;

        public Revit.DB.View View { get; set; }
        public string Title { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;

                _isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
