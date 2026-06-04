using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RvtExtApp = ADSK.JExtRAC.LevelFilter;
using WpfDataGrid = System.Windows.Controls.DataGrid;
using WpfCheckBox = System.Windows.Controls.CheckBox;

namespace ADSK.JExtRAC.LevelFilter.UI
{
    public partial class LevelFilterWindow : Window
    {
        private readonly bool _isDarkTheme;
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly UIDocument _rvtUIDoc;
        private readonly Document _rvtDbDoc;
        private readonly IList<Element> _elemSelect;
        private readonly IList<Part> _partSelect;
        private readonly Dictionary<ElementId, IList<ElementId>> _dicCat;
        private readonly Dictionary<string, IList<ElementId>> _dicFam;
        private readonly Dictionary<string, IList<ElementId>> _dicFamType;
        private readonly Dictionary<ElementId, IList<ElementId>> _dicPart;
        private readonly Dictionary<ElementId, IList<ElementId>> _dicFilter;

        private readonly ObservableCollection<FilterRowItem> _catItems = new();
        private readonly ObservableCollection<FilterRowItem> _famItems = new();
        private readonly ObservableCollection<FilterRowItem> _famTypeItems = new();
        private readonly ObservableCollection<FilterRowItem> _partItems = new();
        private readonly ObservableCollection<FilterRowItem> _filterItems = new();

        private int _tabNum;

        public IList<int> SelectIdAry { get; private set; } = new List<int>();
        public string SelectTabNum => _tabNum.ToString();

        public LevelFilterWindow(
            UIDocument rvtUIDoc,
            RvtExtApp.Components.Attribute cmpAttribute,
            IList<Element> elemSet,
            IList<Part> partSet,
            Dictionary<ElementId, IList<ElementId>> dicCat,
            Dictionary<string, IList<ElementId>> dicFam,
            Dictionary<string, IList<ElementId>> dicFamType,
            Dictionary<ElementId, IList<ElementId>> dicPart,
            Dictionary<ElementId, IList<ElementId>> dicFilter,
            IList<Element> selElems,
            string tabNum)
        {
            InitializeComponent();

            _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;
            _cmpAttribute = cmpAttribute;
            _rvtUIDoc = rvtUIDoc;
            _rvtDbDoc = rvtUIDoc.Document;
            _elemSelect = elemSet;
            _partSelect = partSet;
            _dicCat = dicCat;
            _dicFam = dicFam;
            _dicFamType = dicFamType;
            _dicPart = dicPart;
            _dicFilter = dicFilter;

            if (int.TryParse(tabNum, out _tabNum))
            {
                if (_tabNum < 0) _tabNum = 0;
            }
            else _tabNum = 0;

            SetText();
            SetData();
            BindDataGrids();
            ApplyThemeColors();

            tabControl.SelectedIndex = _tabNum;
        }

        private void SetText()
        {
            titleText.Text = _cmpAttribute.ResourceText("IDS_TXT_FILTERFORM");
            tabCategory.Header = _cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
            tabFamily.Header = _cmpAttribute.ResourceText("IDS_TXT_FAMILY");
            tabFamilyType.Header = _cmpAttribute.ResourceText("IDS_TXT_FAMILYTYPE");
            tabParts.Header = _cmpAttribute.ResourceText("IDS_TXT_PARTS");
            tabFilters.Header = _cmpAttribute.ResourceText("IDS_TXT_FILTERS");

            btnOk.Content = _cmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
            btnApply.Content = _cmpAttribute.ResourceText("IDS_TXT_PNTCHECK");

            var selAll = _cmpAttribute.ResourceText("IDS_TXT_SELECTALL");
            var clr = _cmpAttribute.ResourceText("IDS_TXT_SELECTCLEAR");
            btnSelectAllCat.Content = selAll;
            btnClearCat.Content = clr;
            btnSelectAllFam.Content = selAll;
            btnClearFam.Content = clr;
            btnSelectAllFT.Content = selAll;
            btnClearFT.Content = clr;
            btnSelectAllParts.Content = selAll;
            btnClearParts.Content = clr;
            btnSelectAllFilters.Content = selAll;
            btnClearFilters.Content = clr;

            var countType = _cmpAttribute.ResourceText("IDS_TXT_COUNTTYPE");
            var countObj = _cmpAttribute.ResourceText("IDS_TXT_COUNTOBJECT");
            lblSelItemsCat.Text = countType;
            lblTotalSelCat.Text = countObj;
            lblSelItemsFam.Text = countType;
            lblTotalSelFam.Text = countObj;
            lblSelItemsFT.Text = countType;
            lblTotalSelFT.Text = countObj;
            lblSelItemsParts.Text = countType;
            lblTotalSelParts.Text = countObj;
            lblSelItemsFilters.Text = countType;
            lblTotalSelFilters.Text = countObj;
        }

        private void SetData()
        {
            foreach (ElementId key in _dicCat.Keys)
            {
                int count = _dicCat[key].Count;
                Category cate = Category.GetCategory(_rvtDbDoc, key);
                if (cate == null && _dicCat[key].Count != 0)
                {
                    var first = _dicCat[key].FirstOrDefault();
                    if (first != ElementId.InvalidElementId)
                    {
                        var element = _rvtDbDoc.GetElement(first);
                        cate = element?.Category;
                    }
                }

                if (cate != null)
                {
                    if (key.Value == (long)(int)BuiltInCategory.OST_Lines)
                    {
                        var listDetailId = new List<ElementId>();
                        var listModelId = new List<ElementId>();
                        string nameDetail = "", nameModel = "";
                        foreach (ElementId eleId in _dicCat[key])
                        {
                            Element eleFilter = _rvtDbDoc.GetElement(eleId);
                            if (eleFilter == null) continue;
                            if (eleFilter is DetailCurve) { nameDetail = eleFilter.Name; listDetailId.Add(eleFilter.Id); }
                            else if (eleFilter is ModelCurve) { nameModel = eleFilter.Name; listModelId.Add(eleFilter.Id); }
                        }
                        if (listDetailId.Count != 0)
                            _catItems.Add(new FilterRowItem { IsChecked = true, Name = nameDetail, Count = listDetailId.Count, Tag = listDetailId });
                        if (listModelId.Count != 0)
                            _catItems.Add(new FilterRowItem { IsChecked = true, Name = nameModel, Count = listModelId.Count, Tag = listModelId });
                    }
                    else
                    {
                        _catItems.Add(new FilterRowItem { IsChecked = true, Name = cate.Name, Count = count, Tag = _dicCat[key] });
                    }
                }
                else
                {
                    _catItems.Add(new FilterRowItem { IsChecked = true, Name = _cmpAttribute.ResourceText("IDS_TXT_OTHER"), Count = count, Tag = _dicCat[key] });
                }
            }

            foreach (string key in _dicFam.Keys)
            {
                int count = _dicFam[key].Count;
                string[] splitVal = key.Split(':');
                if (splitVal.Length == 2)
                    _famItems.Add(new FilterRowItem { IsChecked = true, Name = splitVal[1], Count = count, Tag = _dicFam[key] });
            }

            foreach (string key in _dicFamType.Keys)
            {
                int count = _dicFamType[key].Count;
                string[] splitVal = key.Split(':');
                if (splitVal.Length == 2)
                    _famTypeItems.Add(new FilterRowItem { IsChecked = true, Name = splitVal[1], SubName = splitVal[0], Count = count, Tag = _dicFamType[key] });
            }

            foreach (ElementId key in _dicPart.Keys)
            {
                int count = _dicPart[key].Count;
                Element ele = _rvtDbDoc.GetElement(key);
                if (ele != null)
                    _partItems.Add(new FilterRowItem { IsChecked = true, Name = ele.Name, Count = count, Tag = key });
            }

            foreach (ElementId key in _dicFilter.Keys)
            {
                int count = _dicFilter[key].Count;
                Element ele = _rvtDbDoc.GetElement(key);
                if (ele != null)
                    _filterItems.Add(new FilterRowItem { IsChecked = true, Name = ele.Name, Count = count, Tag = key });
            }
        }

        private void BindDataGrids()
        {
            dgCategory.ItemsSource = _catItems;
            dgFamily.ItemsSource = _famItems;
            dgFamilyType.ItemsSource = _famTypeItems;
            dgParts.ItemsSource = _partItems;
            dgFilters.ItemsSource = _filterItems;

            UpdateCounters(_catItems, lblSelItemsCountCat, lblTotalSelCountCat);
            UpdateCounters(_famItems, lblSelItemsCountFam, lblTotalSelCountFam);
            UpdateCounters(_famTypeItems, lblSelItemsCountFT, lblTotalSelCountFT);
            UpdateCounters(_partItems, lblSelItemsCountParts, lblTotalSelCountParts);
            UpdateCounters(_filterItems, lblSelItemsCountFilters, lblTotalSelCountFilters);
        }

        private void UpdateCounters(ObservableCollection<FilterRowItem> items, TextBlock selCount, TextBlock totalCount)
        {
            var checkedItems = items.Where(i => i.IsChecked).ToList();
            selCount.Text = checkedItems.Count.ToString();
            totalCount.Text = checkedItems.Sum(i => i.Count).ToString();
        }

        private (ObservableCollection<FilterRowItem> items, TextBlock selCount, TextBlock totalCount) GetActiveTab()
        {
            int idx = tabControl.SelectedIndex;
            return idx switch
            {
                0 => (_catItems, lblSelItemsCountCat, lblTotalSelCountCat),
                1 => (_famItems, lblSelItemsCountFam, lblTotalSelCountFam),
                2 => (_famTypeItems, lblSelItemsCountFT, lblTotalSelCountFT),
                3 => (_partItems, lblSelItemsCountParts, lblTotalSelCountParts),
                4 => (_filterItems, lblSelItemsCountFilters, lblTotalSelCountFilters),
                _ => (_catItems, lblSelItemsCountCat, lblTotalSelCountCat)
            };
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var (items, selCount, totalCount) = GetActiveTab();
            UpdateCounters(items, selCount, totalCount);
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            var (items, selCount, totalCount) = GetActiveTab();
            foreach (var item in items) item.IsChecked = true;
            UpdateCounters(items, selCount, totalCount);
            RefreshActiveGrid();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            var (items, selCount, totalCount) = GetActiveTab();
            foreach (var item in items) item.IsChecked = false;
            UpdateCounters(items, selCount, totalCount);
            RefreshActiveGrid();
        }

        private void RefreshActiveGrid()
        {
            int idx = tabControl.SelectedIndex;
            WpfDataGrid dg = idx switch
            {
                0 => dgCategory,
                1 => dgFamily,
                2 => dgFamilyType,
                3 => dgParts,
                4 => dgFilters,
                _ => dgCategory
            };
            dg.Items.Refresh();
        }

        private void UpdateSelection()
        {
            SelectIdAry.Clear();
            int idx = tabControl.SelectedIndex;
            _tabNum = idx;

            switch (idx)
            {
                case 0:
                case 1:
                case 2:
                {
                    var items = idx == 0 ? _catItems : idx == 1 ? _famItems : _famTypeItems;
                    foreach (var item in items.Where(i => i.IsChecked))
                    {
                        if (item.Tag is IList<ElementId> idList)
                        {
                            foreach (ElementId eleId in idList)
                                SelectIdAry.Add((int)eleId.Value);
                        }
                        else if (item.Tag is List<ElementId> list)
                        {
                            foreach (ElementId eleId in list)
                                SelectIdAry.Add((int)eleId.Value);
                        }
                    }
                    break;
                }
                case 3:
                {
                    foreach (var item in _partItems.Where(i => i.IsChecked))
                    {
                        var materialId = item.Tag as ElementId;
                        if (materialId == null || materialId == ElementId.InvalidElementId) continue;
                        foreach (Part part in _partSelect)
                        {
                            ICollection<ElementId> materialIds = part.GetMaterialIds(false);
                            if (materialIds.Count == 0) continue;
                            var find = materialIds.ToList().Find(m => m.Equals(materialId));
                            if (find != null) SelectIdAry.Add((int)part.Id.Value);
                        }
                    }
                    break;
                }
                case 4:
                {
                    foreach (var item in _filterItems.Where(i => i.IsChecked))
                    {
                        var elementId = item.Tag as ElementId;
                        if (elementId == null || elementId == ElementId.InvalidElementId) continue;
                        foreach (var pair in _dicFilter)
                        {
                            if (pair.Key == null || pair.Key == ElementId.InvalidElementId) continue;
                            if (pair.Key == elementId)
                            {
                                if (pair.Value != null && pair.Value.Count != 0)
                                {
                                    foreach (var id in pair.Value)
                                        SelectIdAry.Add((int)id.Value);
                                }
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            ICollection<ElementId> elementIds = new List<ElementId>();
            foreach (int id in SelectIdAry)
                elementIds.Add(new ElementId((long)id));

            _rvtUIDoc.Selection.SetElementIds(elementIds);
            _rvtUIDoc.RefreshActiveView();
        }

        private static SolidColorBrush Brush(string hex)
        {
            var b = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(hex));
            b.Freeze();
            return b;
        }

        private void ApplyThemeColors()
        {
            var accent = Brush("#0696d7");
            var font = new FontFamily("ArtifaktElement, Segoe UI, sans-serif");

            if (_isDarkTheme)
            {
                var bg = Brush("#2e3c48");
                var panelBg = Brush("#232f3b");
                var divider = Brush("#3d4f5c");
                var textColor = Brush("#e8ecf0");
                var subtextColor = Brush("#99aab8");
                var gridBg = Brush("#1c2830");
                var gridHeaderBg = Brush("#263340");
                var gridAltRow = Brush("#212d38");
                var gridLine = Brush("#2e3e4c");
                var gridBorder = Brush("#3d4f5c");
                var btnBg = Brush("#34444f");
                var btnHover = Brush("#3e5060");

                outerBorder.Background = bg;
                titleBarGrid.Background = bg;
                dividerBorder.Background = divider;
                titleText.Foreground = textColor;
                btnClose.Foreground = Brush("#8fa4b8");
                footerBorder.Background = Brushes.Transparent;

                // TabControl: transparent bg, no border (border is handled by individual tab underlines)
                var tabControlStyle = new Style(typeof(TabControl));
                tabControlStyle.Setters.Add(new Setter(TabControl.BackgroundProperty, Brushes.Transparent));
                tabControlStyle.Setters.Add(new Setter(TabControl.BorderBrushProperty, divider));
                tabControlStyle.Setters.Add(new Setter(TabControl.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
                this.Resources[typeof(TabControl)] = tabControlStyle;

                // TabItem with bottom underline indicator
                var tabItemStyle = new Style(typeof(TabItem));
                var tabTemplate = new ControlTemplate(typeof(TabItem));
                var borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.Name = "tabBorder";
                borderFactory.SetValue(Border.BackgroundProperty, Brushes.Transparent);
                borderFactory.SetValue(Border.PaddingProperty, new Thickness(14, 8, 14, 8));
                borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 0, 2));
                borderFactory.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
                var cpFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                cpFactory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
                cpFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                cpFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
                borderFactory.AppendChild(cpFactory);
                tabTemplate.VisualTree = borderFactory;

                var tabSelectedTrig = new Trigger { Property = TabItem.IsSelectedProperty, Value = true };
                tabSelectedTrig.Setters.Add(new Setter(TabItem.ForegroundProperty, accent));
                tabSelectedTrig.Setters.Add(new Setter(TabItem.FontWeightProperty, FontWeights.SemiBold));
                tabSelectedTrig.Setters.Add(new Setter(Border.BorderBrushProperty, accent, "tabBorder"));
                tabTemplate.Triggers.Add(tabSelectedTrig);

                var tabHoverTrig = new MultiTrigger();
                tabHoverTrig.Conditions.Add(new Condition(TabItem.IsMouseOverProperty, true));
                tabHoverTrig.Conditions.Add(new Condition(TabItem.IsSelectedProperty, false));
                tabHoverTrig.Setters.Add(new Setter(TabItem.ForegroundProperty, textColor));
                tabTemplate.Triggers.Add(tabHoverTrig);

                tabItemStyle.Setters.Add(new Setter(TabItem.TemplateProperty, tabTemplate));
                tabItemStyle.Setters.Add(new Setter(TabItem.ForegroundProperty, subtextColor));
                tabItemStyle.Setters.Add(new Setter(TabItem.FontFamilyProperty, font));
                tabItemStyle.Setters.Add(new Setter(TabItem.FontSizeProperty, 12.5));
                tabItemStyle.Setters.Add(new Setter(TabItem.CursorProperty, Cursors.Hand));
                this.Resources[typeof(TabItem)] = tabItemStyle;

                // DataGrid
                var dgStyle = new Style(typeof(WpfDataGrid));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.BackgroundProperty, gridBg));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.ForegroundProperty, textColor));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.BorderBrushProperty, gridBorder));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.BorderThicknessProperty, new Thickness(1)));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.RowBackgroundProperty, gridBg));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.AlternatingRowBackgroundProperty, gridAltRow));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.HorizontalGridLinesBrushProperty, gridLine));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.VerticalGridLinesBrushProperty, Brushes.Transparent));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.SelectionUnitProperty, DataGridSelectionUnit.FullRow));
                this.Resources[typeof(WpfDataGrid)] = dgStyle;

                // DataGridRow
                var dgRowStyle = new Style(typeof(DataGridRow));
                dgRowStyle.Setters.Add(new Setter(DataGridRow.ForegroundProperty, textColor));
                dgRowStyle.Setters.Add(new Setter(DataGridRow.MinHeightProperty, 34.0));
                var hoverTrigger = new Trigger { Property = DataGridRow.IsMouseOverProperty, Value = true };
                hoverTrigger.Setters.Add(new Setter(DataGridRow.BackgroundProperty, Brush("#2a3d4e")));
                dgRowStyle.Triggers.Add(hoverTrigger);
                var selTrigger = new Trigger { Property = DataGridRow.IsSelectedProperty, Value = true };
                selTrigger.Setters.Add(new Setter(DataGridRow.BackgroundProperty, Brush("#1a4a6a")));
                selTrigger.Setters.Add(new Setter(DataGridRow.ForegroundProperty, textColor));
                dgRowStyle.Triggers.Add(selTrigger);
                this.Resources[typeof(DataGridRow)] = dgRowStyle;

                // DataGridColumnHeader
                var dgHeaderStyle = new Style(typeof(DataGridColumnHeader));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, gridHeaderBg));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, subtextColor));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, gridLine));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BorderThicknessProperty, new Thickness(0, 0, 1, 1)));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.PaddingProperty, new Thickness(10, 8, 10, 8)));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontFamilyProperty, font));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontSizeProperty, 11.5));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.Medium));
                this.Resources[typeof(DataGridColumnHeader)] = dgHeaderStyle;

                // DataGridCell
                var dgCellStyle = new Style(typeof(DataGridCell));
                dgCellStyle.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.Transparent));
                dgCellStyle.Setters.Add(new Setter(DataGridCell.PaddingProperty, new Thickness(8, 6, 8, 6)));
                dgCellStyle.Setters.Add(new Setter(DataGridCell.FocusVisualStyleProperty, null));
                var cellSelTrigger = new Trigger { Property = DataGridCell.IsSelectedProperty, Value = true };
                cellSelTrigger.Setters.Add(new Setter(DataGridCell.BackgroundProperty, Brushes.Transparent));
                cellSelTrigger.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.Transparent));
                dgCellStyle.Triggers.Add(cellSelTrigger);
                this.Resources[typeof(DataGridCell)] = dgCellStyle;

                // TextBlock default styling
                titleText.Foreground = textColor;
                lblSelItemsCat.Foreground = subtextColor;
                lblTotalSelCat.Foreground = subtextColor;
                lblSelItemsFam.Foreground = subtextColor;
                lblTotalSelFam.Foreground = subtextColor;
                lblSelItemsFT.Foreground = subtextColor;
                lblTotalSelFT.Foreground = subtextColor;
                lblSelItemsParts.Foreground = subtextColor;
                lblTotalSelParts.Foreground = subtextColor;
                lblSelItemsFilters.Foreground = subtextColor;
                lblTotalSelFilters.Foreground = subtextColor;
                lblSelItemsCountCat.Foreground = textColor;
                lblTotalSelCountCat.Foreground = textColor;
                lblSelItemsCountFam.Foreground = textColor;
                lblTotalSelCountFam.Foreground = textColor;
                lblSelItemsCountFT.Foreground = textColor;
                lblTotalSelCountFT.Foreground = textColor;
                lblSelItemsCountParts.Foreground = textColor;
                lblTotalSelCountParts.Foreground = textColor;
                lblSelItemsCountFilters.Foreground = textColor;
                lblTotalSelCountFilters.Foreground = textColor;

                btnOk.Foreground = Brushes.White;
                btnCancel.Foreground = textColor;
                btnApply.Foreground = textColor;
            }
            else
            {
                var bg = Brush("#ffffff");
                var panelBg = Brush("#f8f9fa");
                var divider = Brush("#e0e0e0");
                var textColor = Brush("#1e1e1e");
                var subtextColor = Brush("#5c5c5c");
                var gridBorder = Brush("#d4d4d4");
                var gridLine = Brush("#ebebeb");
                var gridHeaderBg = Brush("#f5f5f5");

                outerBorder.Background = bg;
                titleBarGrid.Background = panelBg;
                dividerBorder.Background = divider;
                titleText.Foreground = textColor;
                btnClose.Foreground = Brush("#5c5c5c");
                footerBorder.Background = Brushes.Transparent;

                // TabControl
                var tabControlStyle = new Style(typeof(TabControl));
                tabControlStyle.Setters.Add(new Setter(TabControl.BackgroundProperty, Brushes.Transparent));
                tabControlStyle.Setters.Add(new Setter(TabControl.BorderBrushProperty, divider));
                tabControlStyle.Setters.Add(new Setter(TabControl.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
                this.Resources[typeof(TabControl)] = tabControlStyle;

                // TabItem with underline
                var tabItemStyle = new Style(typeof(TabItem));
                var tabTemplate = new ControlTemplate(typeof(TabItem));
                var borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.Name = "tabBorder";
                borderFactory.SetValue(Border.BackgroundProperty, Brushes.Transparent);
                borderFactory.SetValue(Border.PaddingProperty, new Thickness(14, 8, 14, 8));
                borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 0, 2));
                borderFactory.SetValue(Border.BorderBrushProperty, Brushes.Transparent);
                var cpFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                cpFactory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
                cpFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                cpFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
                borderFactory.AppendChild(cpFactory);
                tabTemplate.VisualTree = borderFactory;

                var tabSelectedTrig = new Trigger { Property = TabItem.IsSelectedProperty, Value = true };
                tabSelectedTrig.Setters.Add(new Setter(TabItem.ForegroundProperty, accent));
                tabSelectedTrig.Setters.Add(new Setter(TabItem.FontWeightProperty, FontWeights.SemiBold));
                tabSelectedTrig.Setters.Add(new Setter(Border.BorderBrushProperty, accent, "tabBorder"));
                tabTemplate.Triggers.Add(tabSelectedTrig);

                var tabHoverTrig = new MultiTrigger();
                tabHoverTrig.Conditions.Add(new Condition(TabItem.IsMouseOverProperty, true));
                tabHoverTrig.Conditions.Add(new Condition(TabItem.IsSelectedProperty, false));
                tabHoverTrig.Setters.Add(new Setter(TabItem.ForegroundProperty, textColor));
                tabTemplate.Triggers.Add(tabHoverTrig);

                tabItemStyle.Setters.Add(new Setter(TabItem.TemplateProperty, tabTemplate));
                tabItemStyle.Setters.Add(new Setter(TabItem.ForegroundProperty, subtextColor));
                tabItemStyle.Setters.Add(new Setter(TabItem.FontFamilyProperty, font));
                tabItemStyle.Setters.Add(new Setter(TabItem.FontSizeProperty, 12.5));
                tabItemStyle.Setters.Add(new Setter(TabItem.CursorProperty, Cursors.Hand));
                this.Resources[typeof(TabItem)] = tabItemStyle;

                // DataGrid
                var dgStyle = new Style(typeof(WpfDataGrid));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.BackgroundProperty, Brushes.White));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.ForegroundProperty, textColor));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.BorderBrushProperty, gridBorder));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.BorderThicknessProperty, new Thickness(1)));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.RowBackgroundProperty, Brushes.White));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.AlternatingRowBackgroundProperty, Brush("#fafbfc")));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.HorizontalGridLinesBrushProperty, gridLine));
                dgStyle.Setters.Add(new Setter(WpfDataGrid.VerticalGridLinesBrushProperty, Brushes.Transparent));
                this.Resources[typeof(WpfDataGrid)] = dgStyle;

                // DataGridRow
                var dgRowStyle = new Style(typeof(DataGridRow));
                dgRowStyle.Setters.Add(new Setter(DataGridRow.ForegroundProperty, textColor));
                dgRowStyle.Setters.Add(new Setter(DataGridRow.MinHeightProperty, 34.0));
                var hoverTrigger = new Trigger { Property = DataGridRow.IsMouseOverProperty, Value = true };
                hoverTrigger.Setters.Add(new Setter(DataGridRow.BackgroundProperty, Brush("#e8f4fa")));
                dgRowStyle.Triggers.Add(hoverTrigger);
                this.Resources[typeof(DataGridRow)] = dgRowStyle;

                // DataGridColumnHeader
                var dgHeaderStyle = new Style(typeof(DataGridColumnHeader));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, gridHeaderBg));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, subtextColor));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BorderBrushProperty, gridLine));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.BorderThicknessProperty, new Thickness(0, 0, 1, 1)));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.PaddingProperty, new Thickness(10, 8, 10, 8)));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontFamilyProperty, font));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontSizeProperty, 11.5));
                dgHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.FontWeightProperty, FontWeights.Medium));
                this.Resources[typeof(DataGridColumnHeader)] = dgHeaderStyle;

                // DataGridCell
                var dgCellStyle = new Style(typeof(DataGridCell));
                dgCellStyle.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, Brushes.Transparent));
                dgCellStyle.Setters.Add(new Setter(DataGridCell.PaddingProperty, new Thickness(8, 6, 8, 6)));
                dgCellStyle.Setters.Add(new Setter(DataGridCell.FocusVisualStyleProperty, null));
                this.Resources[typeof(DataGridCell)] = dgCellStyle;

                btnOk.Foreground = Brushes.White;
                btnCancel.Foreground = textColor;
                btnApply.Foreground = textColor;
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            SetButtonBackgrounds();
        }

        private void SetButtonBackgrounds()
        {
            var accent = Brush("#0696d7");
            var secondaryBg = _isDarkTheme ? Brush("#34444f") : Brush("#ffffff");
            var secondaryBorder = _isDarkTheme ? Brush("#4a6070") : Brush("#d0d0d0");
            var secondaryFg = _isDarkTheme ? Brush("#c8d8e4") : Brush("#3c3c3c");

            ApplyButtonBorder(btnOk, accent, Brushes.Transparent);
            ApplyButtonBorder(btnCancel, secondaryBg, secondaryBorder);
            ApplyButtonBorder(btnApply, secondaryBg, secondaryBorder);

            var secButtons = new[] {
                btnSelectAllCat, btnClearCat,
                btnSelectAllFam, btnClearFam,
                btnSelectAllFT, btnClearFT,
                btnSelectAllParts, btnClearParts,
                btnSelectAllFilters, btnClearFilters
            };
            foreach (var btn in secButtons)
            {
                btn.Foreground = secondaryFg;
                ApplyButtonBorder(btn, secondaryBg, secondaryBorder);
            }
        }

        private void ApplyButtonBorder(Button btn, SolidColorBrush bg, SolidColorBrush border)
        {
            btn.ApplyTemplate();
            if (VisualTreeHelper.GetChildrenCount(btn) > 0)
            {
                var bd = VisualTreeHelper.GetChild(btn, 0) as Border;
                if (bd != null) { bd.Background = bg; bd.BorderBrush = border; }
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelection();
            DialogResult = true;
            Close();
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelection();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
                _tabNum = tabControl.SelectedIndex;
        }
    }
}
