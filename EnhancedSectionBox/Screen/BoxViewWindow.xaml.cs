using ADSK.JExtRAC.EnhancedSectionBox.Common;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfTextBox = System.Windows.Controls.TextBox;
using WpfComboBox = System.Windows.Controls.ComboBox;
using WpfControl = System.Windows.Controls.Control;
using WpfGrid = System.Windows.Controls.Grid;

namespace ADSK.JExtRAC.EnhancedSectionBox.Screen
{
    public partial class BoxViewWindow : Window
    {
        public static ExternalCommandData CommandData;
        public static UIApplication UiApp;
        public static Autodesk.Revit.ApplicationServices.Application App;
        public static Document Doc;
        public static UIDocument UiDoc;
        public static Element selectionElement;
        public static List<Element> selectionElementList = new List<Element>();
        public static List<Level> levels = new List<Level>();
        public PickedBox pickedBox;

        public static double topValue;
        public static double lowerValue;

        public bool _isSelectObject;
        public bool _isObject;
        public bool _isLink;
        public bool _isRegion;
        private bool _returnFlag;

        public static View3D newView3d;
        private List<Element> _elements = new List<Element>();
        public static List<Element> mainElementList = new List<Element>();
        public static List<Element> linkElementList = new List<Element>();
        public static IList<Reference> linkRefList = new List<Reference>();

        private View _baseView;
        private readonly Components.Attribute _res = new Components.Attribute();
        private readonly bool _isDarkTheme;
        private HashSet<string> _levelSet = new HashSet<string>();

        public bool ResultOK { get; set; }
        public bool ResultExecuted { get; private set; }

        public BoxViewWindow(ExternalCommandData commandData)
        {
            InitializeComponent();

            UiApp = commandData.Application;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            App = UiApp.Application;
            CommandData = commandData;

            _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;
            ApplyThemeColors();
            SetText();
            PopulateData();

            _isSelectObject = false;
            _isObject = false;
            _isLink = false;
            _isRegion = false;
            ResultOK = false;
            ResultExecuted = false;
        }

        private void SetText()
        {
            titleText.Text = _res.ResourceText("IDS_FORM_BOXVIEW_TITLE");
            grpXYPlane.Header = _res.ResourceText("IDS_GRP_XYPLANE");
            radioByObject.Content = _res.ResourceText("IDS_RADIO_OBJECT_SPEC");
            radioPickRange.Content = _res.ResourceText("IDS_BTN_RANGE_SPEC");
            radioByLevel.Content = _res.ResourceText("IDS_RADIO_LEVEL_SPEC");
            btnSelectMain.Content = _res.ResourceText("IDS_BTN_SELECT_MAIN");
            btnSelectLink.Content = _res.ResourceText("IDS_BTN_SELECT_LINK");
            btnPickRange.Content = _res.ResourceText("IDS_BTN_RANGE_SPEC");
            lblRegion.Text = _res.ResourceText("IDS_LBL_NOT_SPECIFIED");
            lblMainCount.Text = _res.ResourceText("IDS_LBL_SELCOUNT_0");
            lblLinkCount.Text = _res.ResourceText("IDS_LBL_SELCOUNT_0");
            chkAlignAngle.Content = _res.ResourceText("IDS_CHK_ALIGN_ANGLE");
            grpLevelOffset.Header = _res.ResourceText("IDS_GRP_LEVEL_OFFSET");
            grpHideObjects.Header = _res.ResourceText("IDS_GRP_HIDE_UPPER_OBJECTS");
            chkFloors.Content = _res.ResourceText("IDS_CHK_FLOOR");
            chkBeams.Content = _res.ResourceText("IDS_CHK_BEAM");
            btnOK.Content = _res.ResourceText("IDS_BTN_OK");
            btnCancel.Content = _res.ResourceText("IDS_BTN_CANCEL");
        }

        private void PopulateData()
        {
            mainElementList = new List<Element>();
            linkElementList = new List<Element>();
            linkRefList = new List<Reference>();
            selectionElement = null;
            selectionElementList = new List<Element>();
            levels = new List<Level>();

            levels = GetElements<Level>(Doc);
            levels.Sort((a, b) => Math.Sign(a.Elevation - b.Elevation));
            foreach (Level level in levels)
            {
                _levelSet.Add(level.Name);
            }
            foreach (string name in _levelSet)
            {
                lstLevels.Items.Add(name);
                cmbTopLevel.Items.Add(name);
                cmbBottomLevel.Items.Add(name);
            }
            if (cmbTopLevel.Items.Count > 0)
                cmbTopLevel.SelectedIndex = cmbTopLevel.Items.Count - 1;
            if (cmbBottomLevel.Items.Count > 0)
                cmbBottomLevel.SelectedIndex = 0;

            List<View> views = GetElements<View>(Doc);
            List<string> viewNameList = new List<string>();
            foreach (View view in views)
            {
                if (view.ViewType == ViewType.ThreeD)
                {
                    if (view.IsTemplate == false && view.Id != view.Document.ActiveView.Id)
                    {
                        viewNameList.Add(view.Name);
                    }
                    if (view.Name == "{3D}")
                    {
                        _baseView = view;
                    }
                }
            }
            viewNameList.Sort();
            foreach (string name in viewNameList)
            {
                cmbViewName.Items.Add(name);
            }

            if (UiDoc.ActiveView.ViewType == ViewType.ThreeD
                || UiDoc.ActiveView.ViewType == ViewType.Elevation
                || UiDoc.ActiveView.ViewType == ViewType.Section)
            {
                radioPickRange.IsEnabled = false;
            }
        }

        private static SolidColorBrush Brush(string hex)
        {
            var b = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(hex));
            b.Freeze();
            return b;
        }

        private void ApplyThemeColors()
        {
            if (_isDarkTheme)
            {
                var surface = Brush("#263545");
                var border = Brush("#3a4f63");
                var textPrimary = Brush("#e0e8f0");
                var textSecondary = Brush("#8fa4b8");
                var inputBg = Brush("#1a2633");
                var inputBorder = Brush("#3a4f63");
                var btnBg = Brush("#2d4050");
                var btnBorder = Brush("#4a6070");
                var btnHover = Brush("#3a5568");
                var accent = Brush("#0696d7");
                var accentHover = Brush("#07a8ed");

                outerBorder.Background = surface;
                titleBarGrid.Background = surface;
                titleText.Foreground = textPrimary;
                dividerBorder.Background = border;
                btnClose.Foreground = textSecondary;

                var res = this.Resources;

                // Override system colors so all standard control internals use dark theme
                res[SystemColors.WindowBrushKey] = inputBg;
                res[SystemColors.WindowTextBrushKey] = textPrimary;
                res[SystemColors.ControlBrushKey] = inputBg;
                res[SystemColors.ControlTextBrushKey] = textPrimary;
                res[SystemColors.HighlightBrushKey] = accent;
                res[SystemColors.HighlightTextBrushKey] = Brush("#ffffff");
                res[SystemColors.InactiveSelectionHighlightBrushKey] = btnBg;
                res[SystemColors.InactiveSelectionHighlightTextBrushKey] = textPrimary;

                // Button style with custom template for dark theme
                var btnStyle = CreateDarkButtonStyle(btnBg, btnBorder, btnHover, textPrimary);
                res[typeof(Button)] = btnStyle;

                // Accent button style for OK
                var accentBtnStyle = CreateDarkButtonStyle(accent, accent, accentHover, Brush("#ffffff"));
                btnOK.Style = accentBtnStyle;

                // TextBox style
                var tbStyle = new Style(typeof(WpfTextBox));
                tbStyle.Setters.Add(new Setter(WpfTextBox.BackgroundProperty, inputBg));
                tbStyle.Setters.Add(new Setter(WpfTextBox.ForegroundProperty, textPrimary));
                tbStyle.Setters.Add(new Setter(WpfTextBox.BorderBrushProperty, inputBorder));
                tbStyle.Setters.Add(new Setter(WpfTextBox.CaretBrushProperty, textPrimary));
                res[typeof(WpfTextBox)] = tbStyle;

                // ComboBox style with custom template
                var cbStyle = new Style(typeof(WpfComboBox));
                cbStyle.Setters.Add(new Setter(WpfComboBox.BackgroundProperty, inputBg));
                cbStyle.Setters.Add(new Setter(WpfComboBox.ForegroundProperty, textPrimary));
                cbStyle.Setters.Add(new Setter(WpfComboBox.BorderBrushProperty, inputBorder));
                cbStyle.Setters.Add(new Setter(WpfComboBox.TemplateProperty, CreateComboBoxTemplate(inputBg, inputBorder, textPrimary, btnHover)));
                res[typeof(WpfComboBox)] = cbStyle;

                // ComboBoxItem style for dropdown items
                var cbiStyle = new Style(typeof(ComboBoxItem));
                cbiStyle.Setters.Add(new Setter(ComboBoxItem.BackgroundProperty, inputBg));
                cbiStyle.Setters.Add(new Setter(ComboBoxItem.ForegroundProperty, textPrimary));
                cbiStyle.Setters.Add(new Setter(ComboBoxItem.BorderBrushProperty, Brushes.Transparent));
                var cbiHoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
                cbiHoverTrigger.Setters.Add(new Setter(ComboBoxItem.BackgroundProperty, btnHover));
                cbiHoverTrigger.Setters.Add(new Setter(ComboBoxItem.ForegroundProperty, Brush("#ffffff")));
                cbiStyle.Triggers.Add(cbiHoverTrigger);
                var cbiSelTrigger = new Trigger { Property = ComboBoxItem.IsSelectedProperty, Value = true };
                cbiSelTrigger.Setters.Add(new Setter(ComboBoxItem.BackgroundProperty, accent));
                cbiSelTrigger.Setters.Add(new Setter(ComboBoxItem.ForegroundProperty, Brush("#ffffff")));
                cbiStyle.Triggers.Add(cbiSelTrigger);
                res[typeof(ComboBoxItem)] = cbiStyle;

                // ListBox style with custom template
                var lbStyle = new Style(typeof(ListBox));
                lbStyle.Setters.Add(new Setter(ListBox.BackgroundProperty, inputBg));
                lbStyle.Setters.Add(new Setter(ListBox.ForegroundProperty, textPrimary));
                lbStyle.Setters.Add(new Setter(ListBox.BorderBrushProperty, inputBorder));
                lbStyle.Setters.Add(new Setter(ListBox.TemplateProperty, CreateListBoxTemplate(inputBg, inputBorder)));
                res[typeof(ListBox)] = lbStyle;

                // ListBoxItem style with custom template
                var lbiStyle = new Style(typeof(ListBoxItem));
                lbiStyle.Setters.Add(new Setter(ListBoxItem.ForegroundProperty, textPrimary));
                lbiStyle.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.Transparent));
                lbiStyle.Setters.Add(new Setter(ListBoxItem.TemplateProperty, CreateListBoxItemTemplate(textPrimary, accent, btnHover)));
                res[typeof(ListBoxItem)] = lbiStyle;

                // GroupBox style with custom template
                var gbStyle = new Style(typeof(GroupBox));
                gbStyle.Setters.Add(new Setter(GroupBox.ForegroundProperty, textPrimary));
                gbStyle.Setters.Add(new Setter(GroupBox.TemplateProperty, CreateGroupBoxTemplate(border, textPrimary)));
                res[typeof(GroupBox)] = gbStyle;

                // RadioButton style
                var rbStyle = new Style(typeof(RadioButton));
                rbStyle.Setters.Add(new Setter(RadioButton.ForegroundProperty, textPrimary));
                res[typeof(RadioButton)] = rbStyle;

                // CheckBox style with custom template for proper alignment
                var ckStyle = new Style(typeof(CheckBox));
                ckStyle.Setters.Add(new Setter(CheckBox.ForegroundProperty, textPrimary));
                ckStyle.Setters.Add(new Setter(CheckBox.TemplateProperty, CreateCheckBoxTemplate(textPrimary, inputBorder, accent)));
                res[typeof(CheckBox)] = ckStyle;

                // TextBlock style
                var tblkStyle = new Style(typeof(TextBlock));
                tblkStyle.Setters.Add(new Setter(TextBlock.ForegroundProperty, textPrimary));
                res[typeof(TextBlock)] = tblkStyle;

                // Secondary labels
                lblMainCount.Foreground = textSecondary;
                lblLinkCount.Foreground = textSecondary;
                lblRegion.Foreground = textSecondary;
                lblUnitTop.Foreground = textSecondary;
                lblUnitBottom.Foreground = textSecondary;
            }
            else
            {
                outerBorder.Background = Brush("#f0f0f0");
                titleBarGrid.Background = Brush("#f0f0f0");
                titleText.Foreground = Brush("#1a1a1a");
                dividerBorder.Background = Brush("#d0d0d0");
                btnClose.Foreground = Brush("#555555");
            }
        }

        private Style CreateDarkButtonStyle(SolidColorBrush bg, SolidColorBrush borderBrush,
            SolidColorBrush hoverBg, SolidColorBrush fg)
        {
            var style = new Style(typeof(Button));

            var template = new ControlTemplate(typeof(Button));
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "border";
            borderFactory.SetValue(Border.BackgroundProperty, bg);
            borderFactory.SetValue(Border.BorderBrushProperty, borderBrush);
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            borderFactory.SetValue(Border.PaddingProperty, new Thickness(6, 2, 6, 2));

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            borderFactory.AppendChild(contentFactory);

            template.VisualTree = borderFactory;

            var hoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, hoverBg, "border"));
            template.Triggers.Add(hoverTrigger);

            var disabledTrigger = new Trigger { Property = UIElement.IsEnabledProperty, Value = false };
            disabledTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.5));
            template.Triggers.Add(disabledTrigger);

            style.Setters.Add(new Setter(WpfControl.TemplateProperty, template));
            style.Setters.Add(new Setter(WpfControl.ForegroundProperty, fg));
            style.Setters.Add(new Setter(WpfControl.CursorProperty, Cursors.Hand));

            return style;
        }

        private ControlTemplate CreateGroupBoxTemplate(SolidColorBrush borderBrush, SolidColorBrush headerFg)
        {
            var template = new ControlTemplate(typeof(GroupBox));

            var gridFactory = new FrameworkElementFactory(typeof(WpfGrid));

            var outerBorderF = new FrameworkElementFactory(typeof(Border));
            outerBorderF.SetValue(Border.BorderBrushProperty, borderBrush);
            outerBorderF.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            outerBorderF.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            outerBorderF.SetValue(Border.PaddingProperty, new Thickness(6, 14, 6, 6));
            outerBorderF.SetValue(Border.MarginProperty, new Thickness(0, 8, 0, 0));

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.ContentSourceProperty, "Content");
            outerBorderF.AppendChild(contentPresenter);

            var headerBorder = new FrameworkElementFactory(typeof(Border));
            headerBorder.SetValue(Border.BackgroundProperty, Brush("#263545"));
            headerBorder.SetValue(Border.PaddingProperty, new Thickness(4, 0, 4, 0));
            headerBorder.SetValue(Border.MarginProperty, new Thickness(10, 0, 0, 0));
            headerBorder.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            headerBorder.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);

            var headerContent = new FrameworkElementFactory(typeof(ContentPresenter));
            headerContent.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            headerContent.SetValue(TextBlock.ForegroundProperty, headerFg);
            headerContent.SetValue(TextBlock.FontSizeProperty, 12.0);
            headerBorder.AppendChild(headerContent);

            gridFactory.AppendChild(outerBorderF);
            gridFactory.AppendChild(headerBorder);

            template.VisualTree = gridFactory;
            return template;
        }

        private ControlTemplate CreateListBoxTemplate(SolidColorBrush bg, SolidColorBrush borderBrush)
        {
            var template = new ControlTemplate(typeof(ListBox));
            var borderF = new FrameworkElementFactory(typeof(Border));
            borderF.SetValue(Border.BackgroundProperty, bg);
            borderF.SetValue(Border.BorderBrushProperty, borderBrush);
            borderF.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            borderF.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));

            var scrollViewer = new FrameworkElementFactory(typeof(ScrollViewer));
            scrollViewer.SetValue(ScrollViewer.PaddingProperty, new Thickness(2));

            var itemsPresenter = new FrameworkElementFactory(typeof(ItemsPresenter));
            scrollViewer.AppendChild(itemsPresenter);
            borderF.AppendChild(scrollViewer);

            template.VisualTree = borderF;
            return template;
        }

        private ControlTemplate CreateListBoxItemTemplate(SolidColorBrush fg, SolidColorBrush selectBg, SolidColorBrush hoverBg)
        {
            var template = new ControlTemplate(typeof(ListBoxItem));
            var borderF = new FrameworkElementFactory(typeof(Border));
            borderF.Name = "Bd";
            borderF.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            borderF.SetValue(Border.PaddingProperty, new Thickness(4, 2, 4, 2));

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            borderF.AppendChild(contentPresenter);

            template.VisualTree = borderF;

            var hoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, hoverBg, "Bd"));
            template.Triggers.Add(hoverTrigger);

            var selTrigger = new Trigger { Property = ListBoxItem.IsSelectedProperty, Value = true };
            selTrigger.Setters.Add(new Setter(Border.BackgroundProperty, selectBg, "Bd"));
            selTrigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brush("#ffffff")));
            template.Triggers.Add(selTrigger);

            return template;
        }

        private ControlTemplate CreateComboBoxTemplate(SolidColorBrush bg, SolidColorBrush borderBrush,
            SolidColorBrush fg, SolidColorBrush hoverBg)
        {
            var template = new ControlTemplate(typeof(WpfComboBox));
            var tpRel = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent);

            var gridF = new FrameworkElementFactory(typeof(WpfGrid));

            var col0 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col0.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            var col1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col1.SetValue(ColumnDefinition.WidthProperty, new GridLength(20, GridUnitType.Pixel));
            gridF.AppendChild(col0);
            gridF.AppendChild(col1);

            // Main border
            var borderF = new FrameworkElementFactory(typeof(Border));
            borderF.Name = "Border";
            borderF.SetValue(Border.BackgroundProperty, bg);
            borderF.SetValue(Border.BorderBrushProperty, borderBrush);
            borderF.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            borderF.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            borderF.SetValue(WpfGrid.ColumnSpanProperty, 2);
            gridF.AppendChild(borderF);

            // Toggle button (invisible trigger for dropdown)
            var toggleBtn = new FrameworkElementFactory(typeof(System.Windows.Controls.Primitives.ToggleButton));
            toggleBtn.Name = "ToggleButton";
            toggleBtn.SetValue(System.Windows.Controls.Primitives.ToggleButton.FocusableProperty, false);
            toggleBtn.SetValue(UIElement.OpacityProperty, 0.0);
            toggleBtn.SetValue(WpfGrid.ColumnSpanProperty, 2);
            toggleBtn.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                new System.Windows.Data.Binding("IsDropDownOpen") { RelativeSource = tpRel, Mode = System.Windows.Data.BindingMode.TwoWay });
            gridF.AppendChild(toggleBtn);

            // Arrow
            var arrowPath = new FrameworkElementFactory(typeof(System.Windows.Shapes.Path));
            arrowPath.SetValue(System.Windows.Shapes.Path.DataProperty,
                System.Windows.Media.Geometry.Parse("M 0,0 L 4,4 L 8,0"));
            arrowPath.SetValue(System.Windows.Shapes.Path.StrokeProperty, fg);
            arrowPath.SetValue(System.Windows.Shapes.Path.StrokeThicknessProperty, 1.5);
            arrowPath.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            arrowPath.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            arrowPath.SetValue(WpfGrid.ColumnProperty, 1);
            arrowPath.SetValue(UIElement.IsHitTestVisibleProperty, false);
            gridF.AppendChild(arrowPath);

            // Content presenter (shows selected item for non-editable mode)
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.Name = "ContentSite";
            contentPresenter.SetBinding(ContentPresenter.ContentProperty,
                new System.Windows.Data.Binding("SelectionBoxItem") { RelativeSource = tpRel });
            contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty,
                new System.Windows.Data.Binding("SelectionBoxItemTemplate") { RelativeSource = tpRel });
            contentPresenter.SetValue(FrameworkElement.MarginProperty, new Thickness(6, 2, 20, 2));
            contentPresenter.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(UIElement.IsHitTestVisibleProperty, false);
            contentPresenter.SetValue(TextBlock.ForegroundProperty, fg);
            contentPresenter.SetValue(WpfGrid.ColumnProperty, 0);
            gridF.AppendChild(contentPresenter);

            // Edit textbox (for editable combobox)
            var editBox = new FrameworkElementFactory(typeof(WpfTextBox));
            editBox.Name = "PART_EditableTextBox";
            editBox.SetBinding(WpfTextBox.IsReadOnlyProperty,
                new System.Windows.Data.Binding("IsReadOnly") { RelativeSource = tpRel });
            editBox.SetValue(UIElement.FocusableProperty, true);
            editBox.SetValue(WpfTextBox.BackgroundProperty, Brushes.Transparent);
            editBox.SetValue(WpfTextBox.ForegroundProperty, fg);
            editBox.SetValue(WpfTextBox.CaretBrushProperty, fg);
            editBox.SetValue(WpfTextBox.BorderThicknessProperty, new Thickness(0));
            editBox.SetValue(FrameworkElement.MarginProperty, new Thickness(4, 0, 20, 0));
            editBox.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            editBox.SetValue(WpfGrid.ColumnProperty, 0);
            editBox.SetValue(UIElement.VisibilityProperty, System.Windows.Visibility.Collapsed);
            gridF.AppendChild(editBox);

            // Popup
            var popup = new FrameworkElementFactory(typeof(System.Windows.Controls.Primitives.Popup));
            popup.Name = "Popup";
            popup.SetValue(System.Windows.Controls.Primitives.Popup.PlacementProperty,
                System.Windows.Controls.Primitives.PlacementMode.Bottom);
            popup.SetValue(System.Windows.Controls.Primitives.Popup.AllowsTransparencyProperty, true);
            popup.SetBinding(System.Windows.Controls.Primitives.Popup.IsOpenProperty,
                new System.Windows.Data.Binding("IsDropDownOpen") { RelativeSource = tpRel });

            var popupBorder = new FrameworkElementFactory(typeof(Border));
            popupBorder.SetValue(Border.BackgroundProperty, bg);
            popupBorder.SetValue(Border.BorderBrushProperty, borderBrush);
            popupBorder.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            popupBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            popupBorder.SetValue(Border.PaddingProperty, new Thickness(2));
            popupBorder.SetBinding(FrameworkElement.MinWidthProperty,
                new System.Windows.Data.Binding("ActualWidth") { RelativeSource = tpRel });
            popupBorder.SetValue(FrameworkElement.MaxHeightProperty, 200.0);

            var popupScroll = new FrameworkElementFactory(typeof(ScrollViewer));
            var itemsPresenter = new FrameworkElementFactory(typeof(ItemsPresenter));
            popupScroll.AppendChild(itemsPresenter);
            popupBorder.AppendChild(popupScroll);
            popup.AppendChild(popupBorder);
            gridF.AppendChild(popup);

            template.VisualTree = gridF;

            // IsEditable trigger
            var editableTrigger = new Trigger { Property = WpfComboBox.IsEditableProperty, Value = true };
            editableTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, System.Windows.Visibility.Visible, "PART_EditableTextBox"));
            editableTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, System.Windows.Visibility.Collapsed, "ContentSite"));
            template.Triggers.Add(editableTrigger);

            // Hover trigger
            var hoverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, hoverBg, "Border"));
            template.Triggers.Add(hoverTrigger);

            return template;
        }

        private ControlTemplate CreateCheckBoxTemplate(SolidColorBrush fg, SolidColorBrush borderBrush, SolidColorBrush checkedBg)
        {
            var template = new ControlTemplate(typeof(CheckBox));

            var gridF = new FrameworkElementFactory(typeof(WpfGrid));

            var col0 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col0.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);
            var col1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            col1.SetValue(ColumnDefinition.WidthProperty, new GridLength(1, GridUnitType.Star));
            gridF.AppendChild(col0);
            gridF.AppendChild(col1);

            // Checkbox border/glyph
            var boxBorder = new FrameworkElementFactory(typeof(Border));
            boxBorder.Name = "CheckBorder";
            boxBorder.SetValue(Border.WidthProperty, 16.0);
            boxBorder.SetValue(Border.HeightProperty, 16.0);
            boxBorder.SetValue(Border.BorderBrushProperty, borderBrush);
            boxBorder.SetValue(Border.BorderThicknessProperty, new Thickness(1.5));
            boxBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(3));
            boxBorder.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            boxBorder.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            boxBorder.SetValue(WpfGrid.ColumnProperty, 0);

            // Checkmark path
            var checkMark = new FrameworkElementFactory(typeof(System.Windows.Shapes.Path));
            checkMark.Name = "CheckMark";
            checkMark.SetValue(System.Windows.Shapes.Path.DataProperty, System.Windows.Media.Geometry.Parse("M 2,5 L 5.5,9 L 11,2"));
            checkMark.SetValue(System.Windows.Shapes.Path.StrokeProperty, Brush("#ffffff"));
            checkMark.SetValue(System.Windows.Shapes.Path.StrokeThicknessProperty, 2.0);
            checkMark.SetValue(UIElement.VisibilityProperty, System.Windows.Visibility.Collapsed);
            checkMark.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            checkMark.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            checkMark.SetValue(FrameworkElement.MarginProperty, new Thickness(1));
            boxBorder.AppendChild(checkMark);

            // Content
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(WpfGrid.ColumnProperty, 1);
            contentPresenter.SetValue(FrameworkElement.MarginProperty, new Thickness(6, 0, 0, 0));
            contentPresenter.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.RecognizesAccessKeyProperty, true);

            gridF.AppendChild(boxBorder);
            gridF.AppendChild(contentPresenter);

            template.VisualTree = gridF;

            // Checked trigger
            var checkedTrigger = new Trigger { Property = CheckBox.IsCheckedProperty, Value = true };
            checkedTrigger.Setters.Add(new Setter(UIElement.VisibilityProperty, System.Windows.Visibility.Visible, "CheckMark"));
            checkedTrigger.Setters.Add(new Setter(Border.BackgroundProperty, checkedBg, "CheckBorder"));
            checkedTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, checkedBg, "CheckBorder"));
            template.Triggers.Add(checkedTrigger);

            return template;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void BtnClose_Click(object sender, RoutedEventArgs e) { ResultOK = false; Close(); }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) { ResultOK = false; Close(); }

        private void Radio_Changed(object sender, RoutedEventArgs e)
        {
            if (btnSelectMain == null) return;
            bool byObj = radioByObject.IsChecked == true;
            bool byRange = radioPickRange.IsChecked == true;
            bool byLevel = radioByLevel.IsChecked == true;

            btnSelectMain.IsEnabled = byObj;
            btnSelectLink.IsEnabled = byObj;
            chkAlignAngle.IsEnabled = byObj;
            btnPickRange.IsEnabled = byRange;
            lstLevels.IsEnabled = byLevel;
        }

        private void BtnSelectMain_Click(object sender, RoutedEventArgs e)
        {
            pickedBox = null;
            _isSelectObject = true;
            _isObject = true;
            _isLink = false;
            _isRegion = false;
            ResultOK = true;
            Hide();
        }

        private void BtnSelectLink_Click(object sender, RoutedEventArgs e)
        {
            pickedBox = null;
            _isSelectObject = true;
            _isObject = false;
            _isLink = true;
            _isRegion = false;
            ResultOK = true;
            Hide();
        }

        private void BtnPickRange_Click(object sender, RoutedEventArgs e)
        {
            _isSelectObject = true;
            _isObject = false;
            _isLink = false;
            _isRegion = true;
            ResultOK = true;
            Hide();
        }

        private void ViewName_Changed(object sender, TextChangedEventArgs e)
        {
            if (btnOK == null) return;
            string text = cmbViewName.Text;
            btnOK.IsEnabled = !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            topValue = 0;
            lowerValue = 0;
            if (double.TryParse(txtTopOffset.Text, out double tv))
            {
                topValue = UnitUtils.ConvertToInternalUnits(tv, UnitTypeId.Millimeters);
            }
            if (double.TryParse(txtBottomOffset.Text, out double lv))
            {
                lowerValue = UnitUtils.ConvertToInternalUnits(lv, UnitTypeId.Millimeters);
            }

            CreateView();
            if (_returnFlag) return;

            UiDoc.ActiveView = newView3d;

            List<ElementId> idList = new List<ElementId>();
            if (chkFloors.IsChecked == true)
            {
                foreach (Element element in _elements)
                {
                    if (element.Category != null
                        && element.Category.BuiltInCategory == BuiltInCategory.OST_Floors)
                    {
                        idList.Add(element.Id);
                    }
                }
            }
            if (chkBeams.IsChecked == true)
            {
                foreach (Element element in _elements)
                {
                    if (element.Category != null
                        && element.Category.BuiltInCategory == BuiltInCategory.OST_StructuralFraming)
                    {
                        idList.Add(element.Id);
                    }
                }
            }
            if (idList.Count > 0)
            {
                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_HIDE")))
                {
                    tran.Start();
                    UiDoc.ActiveView.HideElements(idList);
                    tran.Commit();
                }
            }

            ResultExecuted = true;
            ResultOK = false;
            _isSelectObject = false;
            Close();
        }

        public void CreateView()
        {
            _returnFlag = false;
            string viewName = cmbViewName.Text;
            List<View3D> views = GetElements<View3D>(Doc);
            bool viewFlag = false;
            View3D selectView = null;

            foreach (View3D view in views)
            {
                if (view.Name == viewName)
                {
                    viewFlag = true;
                    selectView = view;
                    break;
                }
            }

            Level topEdge = null;
            Level lowerEnd = null;
            string topEdgeName = cmbTopLevel.SelectedItem?.ToString();
            string lowerEndName = cmbBottomLevel.SelectedItem?.ToString();

            if (topEdgeName == null || lowerEndName == null)
            {
                ComDialog.ShowDialog(_res.ResourceText("IDS_WARN_TITLE"), TaskDialogIcon.TaskDialogIconWarning, _res.ResourceText("IDS_WARN_LEVEL_ORDER"), false);
                _returnFlag = true;
                return;
            }

            foreach (Level level in levels)
            {
                if (level.Name == topEdgeName) topEdge = level;
                if (level.Name == lowerEndName) lowerEnd = level;
            }

            if (topEdge == null || lowerEnd == null)
            {
                _returnFlag = true;
                return;
            }

            if (topEdge.Elevation + topValue <= lowerEnd.Elevation + lowerValue)
            {
                ComDialog.ShowDialog(_res.ResourceText("IDS_WARN_TITLE"), TaskDialogIcon.TaskDialogIconWarning, _res.ResourceText("IDS_WARN_LEVEL_ORDER"), false);
                _returnFlag = true;
                return;
            }

            if (viewFlag)
            {
                ApplySectionBox(selectView, topEdge, lowerEnd);
                newView3d = selectView;
            }
            else
            {
                if (_baseView == null)
                {
                    ComDialog.ShowDialog(_res.ResourceText("IDS_ERR_TITLE"), TaskDialogIcon.TaskDialogIconError, _res.ResourceText("IDS_ERR_NO_DEFAULT_3DVIEW"), false);
                    _returnFlag = true;
                    return;
                }

                View3D newView = null;
                ElementId id;
                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
                {
                    tran.Start();
                    id = _baseView.Duplicate(new ViewDuplicateOption());
                    tran.Commit();
                }
                newView = (View3D)(Doc.GetElement(id));

                ApplySectionBox(newView, topEdge, lowerEnd);
                newView3d = newView;

                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
                {
                    tran.Start();
                    newView.Name = viewName;
                    ParameterSet para = newView.Parameters;
                    foreach (Parameter p in para)
                    {
                        if (p.Id == new ElementId(BuiltInParameter.VIEWER_CROP_REGION))
                            p.Set(0);
                        if (p.Id == new ElementId(BuiltInParameter.VIEWER_CROP_REGION_VISIBLE))
                            p.Set(0);
                    }
                    tran.Commit();
                }

                List<BuiltInCategory> builtInCategoryList = new List<BuiltInCategory>()
                {
                    BuiltInCategory.OST_StructuralFraming,
                    BuiltInCategory.OST_Floors
                };
                ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(builtInCategoryList);
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(Doc);
                List<Element> elementList = filteredElementCollector.WherePasses(elementMulticategoryFilter).WhereElementIsNotElementType().ToList();
                foreach (Element ele in elementList)
                {
                    if (ele.LevelId == topEdge.Id)
                    {
                        _elements.Add(ele);
                    }
                    else if (ele.GetType().Name == "FamilyInstance")
                    {
                        Parameter refLevelParam = ele.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
                        if (refLevelParam != null && refLevelParam.AsValueString() == topEdge.Name)
                        {
                            _elements.Add(ele);
                        }
                    }
                }
            }
        }

        private void ApplySectionBox(View3D targetView, Level topEdge, Level lowerEnd)
        {
            List<XYZ> boxValueList = new List<XYZ>();
            List<XYZ> cornerList = new List<XYZ>();
            double tilt = double.MinValue;
            double rad = 0;
            XYZ baseDirection = null;
            bool rotateFlag = false;

            foreach (Element element in mainElementList)
            {
                BoundingBoxXYZ box = element.get_BoundingBox(UiDoc.ActiveView);
                if (box != null)
                {
                    boxValueList.Add(box.Min);
                    boxValueList.Add(box.Max);
                }
                Options opt = new Options { ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine };
                List<Solid> solidList = GetSolids(element, opt);
                Dictionary<XYZ, int> corners = GetCorners(solidList);
                foreach (var kvp in corners) cornerList.Add(kvp.Key);
                ProcessElementDirection(element, ref tilt, ref rad, ref baseDirection, ref rotateFlag);
            }

            foreach (Reference reference in linkRefList)
            {
                Element element = Doc.GetElement(reference);
                RevitLinkInstance linkInstance = (RevitLinkInstance)element;
                Document linkDoc = linkInstance.GetLinkDocument();
                Autodesk.Revit.DB.Transform t = linkInstance.GetTotalTransform();
                Element linkedElement = linkDoc.GetElement(reference.LinkedElementId);
                BoundingBoxXYZ box = linkedElement.get_BoundingBox(UiDoc.ActiveView);
                if (box != null)
                {
                    boxValueList.Add(t.OfPoint(box.Min));
                    boxValueList.Add(t.OfPoint(box.Max));
                }
                Options opt = new Options { ComputeReferences = true, DetailLevel = ViewDetailLevel.Fine };
                List<Solid> solidList = GetSolids(linkedElement, opt);
                Dictionary<XYZ, int> corners = solidList.Count > 0 ? GetCorners(solidList) : new Dictionary<XYZ, int>();
                foreach (var kvp in corners) cornerList.Add(kvp.Key);
                ProcessLinkedElementDirection(linkedElement, t, ref tilt, ref rad, ref baseDirection, ref rotateFlag);
            }

            if (boxValueList.Count > 0 && radioByObject.IsChecked == true)
            {
                boxValueList.Sort((a, b) => Math.Sign(a.X - b.X));
                double maxX = boxValueList.Last().X;
                double minX = boxValueList.First().X;
                boxValueList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                double maxY = boxValueList.Last().Y;
                double minY = boxValueList.First().Y;
                BoundingBoxXYZ sbox = new BoundingBoxXYZ();
                sbox.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                sbox.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
                {
                    tran.Start();
                    targetView.SetSectionBox(sbox);
                    tran.Commit();
                }
            }

            if (cornerList.Count == 0) rotateFlag = false;

            if (boxValueList.Count > 0 && chkAlignAngle.IsChecked == true && rotateFlag)
            {
                ApplyRotatedBox(targetView, cornerList, tilt, rad, topEdge, lowerEnd);
            }

            if (pickedBox != null && radioPickRange.IsChecked == true)
            {
                try
                {
                    XYZ min = pickedBox.Min;
                    XYZ max = pickedBox.Max;
                    double maxX = Math.Max(max.X, min.X);
                    double minX = Math.Min(max.X, min.X);
                    double maxY = Math.Max(max.Y, min.Y);
                    double minY = Math.Min(max.Y, min.Y);
                    BoundingBoxXYZ sbox = new BoundingBoxXYZ();
                    sbox.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                    sbox.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                    using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
                    {
                        tran.Start();
                        targetView.SetSectionBox(sbox);
                        tran.Commit();
                    }
                }
                catch { }
            }

            if (radioByLevel.IsChecked == true)
            {
                ApplyLevelBox(targetView, topEdge, lowerEnd);
            }
        }

        private void ApplyLevelBox(View3D targetView, Level topEdge, Level lowerEnd)
        {
            List<XYZ> valueList = new List<XYZ>();
            List<string> nameList = new List<string>();
            foreach (object item in lstLevels.SelectedItems)
                nameList.Add(item.ToString());

            foreach (Level level in levels)
            {
                foreach (string name in nameList)
                {
                    if (level.Name == name)
                    {
                        using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
                        {
                            tran.Start();
                            targetView.EnableRevealHiddenMode();
                            Doc.Regenerate();
                            BoundingBoxXYZ box = level.get_BoundingBox(targetView);
                            if (box != null)
                            {
                                valueList.Add(box.Min);
                                valueList.Add(box.Max);
                            }
                            targetView.DisableTemporaryViewMode(TemporaryViewMode.RevealHiddenElements);
                            tran.Commit();
                        }
                    }
                }
            }

            if (valueList.Count > 0)
            {
                valueList.Sort((a, b) => Math.Sign(a.X - b.X));
                double maxX = valueList.Last().X;
                double minX = valueList.First().X;
                valueList.Sort((a, b) => Math.Sign(a.Y - b.Y));
                double maxY = valueList.Last().Y;
                double minY = valueList.First().Y;
                BoundingBoxXYZ sbox = new BoundingBoxXYZ();
                sbox.Max = new XYZ(maxX, maxY, topEdge.Elevation + topValue);
                sbox.Min = new XYZ(minX, minY, lowerEnd.Elevation + lowerValue);
                using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
                {
                    tran.Start();
                    targetView.SetSectionBox(sbox);
                    tran.Commit();
                }
            }
        }

        private void ApplyRotatedBox(View3D targetView, List<XYZ> cornerList, double tilt, double rad, Level topEdge, Level lowerEnd)
        {
            double mTilt = -(1 / tilt);
            double mTilt2 = tilt + (1 / tilt);

            cornerList.Sort((a, b) => Math.Sign(GetIntercept(tilt, a) - GetIntercept(tilt, b)));
            XYZ minTiltPoint = cornerList.First();
            XYZ maxTiltPoint = cornerList.Last();
            cornerList.Sort((a, b) => Math.Sign(GetIntercept(mTilt, a) - GetIntercept(mTilt, b)));
            XYZ minMtiltPoint = cornerList.First();
            XYZ maxMtiltPoint = cornerList.Last();

            double x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
            double y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
            XYZ leftBottomPoint = new XYZ(x1, y1, 0);

            XYZ leftTopPoint;
            if (tilt > 0)
            {
                x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(maxTiltPoint, tilt)) / mTilt2;
                y1 = tilt * x1 + GetInterceptXY(maxTiltPoint, tilt);
                leftTopPoint = new XYZ(x1, y1, 0);
            }
            else
            {
                x1 = (GetInterceptXY(maxMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                leftTopPoint = new XYZ(x1, y1, 0);
            }

            XYZ rightBottomPoint;
            if (tilt > 0)
            {
                x1 = (GetInterceptXY(maxMtiltPoint, mTilt) - GetInterceptXY(minTiltPoint, tilt)) / mTilt2;
                y1 = tilt * x1 + GetInterceptXY(minTiltPoint, tilt);
                rightBottomPoint = new XYZ(x1, y1, 0);
            }
            else
            {
                x1 = (GetInterceptXY(minMtiltPoint, mTilt) - GetInterceptXY(maxTiltPoint, tilt)) / mTilt2;
                y1 = tilt * x1 + GetInterceptXY(maxTiltPoint, tilt);
                rightBottomPoint = new XYZ(x1, y1, 0);
            }

            double bp1 = Math.Abs(leftBottomPoint.X - rightBottomPoint.X);
            double h1 = Math.Abs(leftBottomPoint.Y - rightBottomPoint.Y);
            double rightLength = Math.Sqrt(bp1 * bp1 + h1 * h1);
            double bp2 = Math.Abs(leftBottomPoint.X - leftTopPoint.X);
            double h2 = Math.Abs(leftBottomPoint.Y - leftTopPoint.Y);
            double leftLength = Math.Sqrt(bp2 * bp2 + h2 * h2);

            double boxX = leftBottomPoint.X + rightLength;
            double boxY = leftBottomPoint.Y + leftLength;

            BoundingBoxXYZ sbox = new BoundingBoxXYZ();
            sbox.Max = new XYZ(boxX, boxY, topEdge.Elevation + topValue);
            sbox.Min = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, lowerEnd.Elevation + lowerValue);
            using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
            {
                tran.Start();
                targetView.SetSectionBox(sbox);
                tran.Commit();
            }

            using (Transaction tran = new Transaction(Doc, _res.ResourceText("IDS_TRAN_VIEW_CREATE")))
            {
                tran.Start();
                FilteredElementCollector boxCollector = new FilteredElementCollector(Doc);
                IList<Element> boxCollection = boxCollector.OfCategory(BuiltInCategory.OST_SectionBox).ToElements();
                Element createBox = null;
                foreach (Element element in boxCollection)
                {
                    BoundingBoxXYZ elementBox = element.get_BoundingBox(targetView);
                    if (elementBox == null) continue;
                    double elementMaxX = Math.Round(elementBox.Max.X, 4, MidpointRounding.AwayFromZero);
                    double elementMinX = Math.Round(elementBox.Min.X, 4, MidpointRounding.AwayFromZero);
                    double newMaxX = Math.Round(targetView.GetSectionBox().Max.X, 4, MidpointRounding.AwayFromZero);
                    double newMinX = Math.Round(targetView.GetSectionBox().Min.X, 4, MidpointRounding.AwayFromZero);
                    if (newMaxX == elementMaxX && newMinX == elementMinX)
                        createBox = element;
                }
                if (createBox != null)
                {
                    XYZ point1 = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, 0);
                    XYZ point2 = new XYZ(leftBottomPoint.X, leftBottomPoint.Y, 10);
                    Line axis = Line.CreateBound(point1, point2);
                    double angle = Math.Round(rad / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                    if (angle < 180 && angle > 90)
                    {
                        angle = 90 - (180 - angle);
                        rad = angle * Math.PI / 180;
                    }
                    ElementTransformUtils.RotateElement(Doc, createBox.Id, axis, rad);
                }
                tran.Commit();
            }
        }

        private void ProcessElementDirection(Element element, ref double tilt, ref double rad, ref XYZ baseDirection, ref bool rotateFlag)
        {
            LocationCurve locationCurve = element.Location as LocationCurve;
            if (locationCurve != null && locationCurve.Curve.GetType().Name == "Line")
            {
                Line curve = (Line)locationCurve.Curve;
                XYZ point1 = curve.GetEndPoint(0);
                XYZ point2 = curve.GetEndPoint(1);
                Line line = point1.Y <= point2.Y ? Line.CreateBound(point1, point2) : Line.CreateBound(point2, point1);
                XYZ direction = new XYZ(line.Direction.X, line.Direction.Y, 0);
                XYZ zero = new XYZ(1, 0, 0);
                rad = zero.AngleTo(direction);
                if (baseDirection == null)
                {
                    baseDirection = direction;
                    rotateFlag = true;
                }
                else
                {
                    double rad2 = baseDirection.AngleTo(direction);
                    double angle2 = Math.Round(rad2 / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                    if (angle2 != 0 && angle2 != 90 && angle2 != 180 && angle2 != 270)
                        rotateFlag = false;
                }
                if (Math.Tan(rad) > tilt)
                    tilt = Math.Tan(rad);
            }
        }

        private void ProcessLinkedElementDirection(Element linkedElement, Autodesk.Revit.DB.Transform t, ref double tilt, ref double rad, ref XYZ baseDirection, ref bool rotateFlag)
        {
            LocationCurve locationCurve = linkedElement.Location as LocationCurve;
            if (locationCurve != null && locationCurve.Curve.GetType().Name == "Line")
            {
                Line curve = (Line)locationCurve.Curve;
                XYZ point1 = curve.GetEndPoint(0);
                XYZ point2 = curve.GetEndPoint(1);
                Line line = point1.Y <= point2.Y ? Line.CreateBound(point1, point2) : Line.CreateBound(point2, point1);
                XYZ direction = line.Direction;
                XYZ linkDirection = t.OfPoint(direction);
                linkDirection = new XYZ(linkDirection.X, linkDirection.Y, 0);
                XYZ zero = new XYZ(1, 0, 0);
                rad = zero.AngleTo(linkDirection);
                if (baseDirection == null)
                {
                    baseDirection = linkDirection;
                    rotateFlag = true;
                }
                else
                {
                    double rad2 = baseDirection.AngleTo(linkDirection);
                    double angle2 = Math.Round(rad2 / Math.PI * 180, 0, MidpointRounding.AwayFromZero);
                    if (angle2 != 0 && angle2 != 90 && angle2 != 180 && angle2 != 270)
                        rotateFlag = false;
                }
                if (Math.Tan(rad) > tilt)
                    tilt = Math.Tan(rad);
            }
        }

        private List<Solid> GetSolids(Element element, Options opt)
        {
            List<Solid> solidList = new List<Solid>();
            GeometryElement geo = element.get_Geometry(opt);
            if (geo == null) return solidList;

            foreach (GeometryObject obj in geo)
            {
                if (obj is GeometryInstance gi)
                {
                    GeometryElement geo2 = gi.GetInstanceGeometry();
                    foreach (GeometryObject obj2 in geo2)
                    {
                        if (obj2 is Solid solid && solid.Faces.Size > 0)
                            solidList.Add(solid);
                    }
                }
                else if (obj is Solid s && s.Faces.Size > 0)
                {
                    solidList.Add(s);
                }
            }
            return solidList;
        }

        private double GetInterceptXY(XYZ p1, double sl)
        {
            if (double.IsInfinity(sl)) return p1.X;
            if (Math.Round(sl, 3, MidpointRounding.AwayFromZero) == 0) return p1.Y;
            return p1.Y - (sl * p1.X);
        }

        private double GetIntercept(double tilt, XYZ point)
        {
            return point.Y - (tilt * point.X);
        }

        public static Dictionary<XYZ, int> GetCorners(List<Solid> solids)
        {
            Dictionary<XYZ, int> corners = new Dictionary<XYZ, int>(new XyzEqualityComparer());
            foreach (Solid solid in solids)
            {
                foreach (Face f in solid.Faces)
                {
                    foreach (EdgeArray ea in f.EdgeLoops)
                    {
                        foreach (Edge e in ea)
                        {
                            XYZ p = e.AsCurveFollowingFace(f).GetEndPoint(0);
                            if (!corners.ContainsKey(p)) corners[p] = 0;
                            ++corners[p];
                        }
                    }
                }
            }
            return corners;
        }

        private class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            private const double _sixteenthInchInFeet = 1.0 / (16.0 * 12.0);
            public bool Equals(XYZ p, XYZ q) => p.IsAlmostEqualTo(q, _sixteenthInchInFeet);
            public int GetHashCode(XYZ p) => $"({p.X:0.##},{p.Y:0.##},{p.Z:0.##})".GetHashCode();
        }

        public static List<T> GetElements<T>(Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>().ToList();
        }
    }
}
