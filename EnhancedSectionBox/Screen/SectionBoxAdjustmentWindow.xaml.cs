using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using WpfTextBox = System.Windows.Controls.TextBox;
using WpfBorder = System.Windows.Controls.Border;
using TextChangedEventArgs = System.Windows.Controls.TextChangedEventArgs;

namespace ADSK.JExtRAC.EnhancedSectionBox.Screen
{
    public partial class SectionBoxAdjustmentWindow : Window
    {
        private readonly bool _isDarkTheme;
        private readonly Components.Attribute _res = new Components.Attribute();

        private readonly UIDocument _uiDoc;
        private readonly Document _doc;

        private readonly double _defaultMaxX;
        private readonly double _defaultMinX;
        private readonly double _defaultMaxY;
        private readonly double _defaultMinY;
        private readonly double _defaultMaxZ;
        private readonly double _defaultMinZ;


        public enum AdjustResult { OK, Cancel }
        public AdjustResult Result { get; private set; } = AdjustResult.Cancel;

        public SectionBoxAdjustmentWindow(ExternalCommandData commandData)
        {
            InitializeComponent();

            _isDarkTheme = UIThemeManager.CurrentTheme == UITheme.Dark;

            _uiDoc = commandData.Application.ActiveUIDocument;
            _doc = _uiDoc.Document;

            View3D view3d = (View3D)_uiDoc.ActiveView;
            BoundingBoxXYZ box = view3d.GetSectionBox();

            _defaultMaxX = box.Max.X;
            _defaultMinX = box.Min.X;
            _defaultMaxY = box.Max.Y;
            _defaultMinY = box.Min.Y;
            _defaultMaxZ = box.Max.Z;
            _defaultMinZ = box.Min.Z;

            SetText();
            ApplyThemeColors();
            AttachEvents();
        }

        private void SetText()
        {
            titleText.Text = _res.ResourceText("IDS_FORM_SECTIONBOX_TITLE");
            groupTitle.Text = _res.ResourceText("IDS_GRP_LENGTH_ADJUST");
            lblLeft.Text = _res.ResourceText("IDS_LBL_LEFT");
            lblRight.Text = _res.ResourceText("IDS_LBL_RIGHT");
            lblFront.Text = _res.ResourceText("IDS_LBL_FRONT");
            lblBack.Text = _res.ResourceText("IDS_LBL_BACK");
            lblTop.Text = _res.ResourceText("IDS_LBL_TOP");
            lblBottom.Text = _res.ResourceText("IDS_LBL_BOTTOM");
            btnOk.Content = _res.ResourceText("IDS_BTN_OK");
            btnCancel.Content = _res.ResourceText("IDS_BTN_CANCEL");
        }

        private void AttachEvents()
        {
            textOffsetLeft.TextChanged += OffsetLeft_TextChanged;
            textOffsetRight.TextChanged += OffsetRight_TextChanged;
            textOffsetForward.TextChanged += OffsetForward_TextChanged;
            textOffsetBack.TextChanged += OffsetBack_TextChanged;
            textOffsetTop.TextChanged += OffsetTop_TextChanged;
            textOffsetBottom.TextChanged += OffsetBottom_TextChanged;

            textOffsetLeft.PreviewTextInput += TextBox_PreviewTextInput;
            textOffsetRight.PreviewTextInput += TextBox_PreviewTextInput;
            textOffsetForward.PreviewTextInput += TextBox_PreviewTextInput;
            textOffsetBack.PreviewTextInput += TextBox_PreviewTextInput;
            textOffsetTop.PreviewTextInput += TextBox_PreviewTextInput;
            textOffsetBottom.PreviewTextInput += TextBox_PreviewTextInput;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (c == '-' || c == '.') continue;
                if (c < '0' || c > '9')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void OffsetLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!double.TryParse(textOffsetLeft.Text, out double leftOffset)) return;

                View3D view3d = (View3D)_uiDoc.ActiveView;
                double centerPos = (_defaultMaxX + _defaultMinX) / 2;
                double length = (_defaultMaxX - _defaultMinX) / 2;

                double leftFeet = UnitUtils.Convert(leftOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                double newMinX = centerPos - length - leftFeet;

                using (Transaction tran = new Transaction(_doc, _res.ResourceText("IDS_TRAN_OFFSET")))
                {
                    tran.Start();
                    BoundingBoxXYZ nBox = view3d.GetSectionBox();
                    nBox.Min = new XYZ(newMinX, nBox.Min.Y, nBox.Min.Z);
                    view3d.SetSectionBox(nBox);
                    tran.Commit();
                }
            }
            catch { }
        }

        private void OffsetRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!double.TryParse(textOffsetRight.Text, out double rightOffset)) return;

                View3D view3d = (View3D)_uiDoc.ActiveView;
                double centerPos = (_defaultMaxX + _defaultMinX) / 2;
                double length = (_defaultMaxX - _defaultMinX) / 2;

                double rightFeet = UnitUtils.Convert(rightOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                double newMaxX = centerPos + length + rightFeet;

                using (Transaction tran = new Transaction(_doc, _res.ResourceText("IDS_TRAN_OFFSET")))
                {
                    tran.Start();
                    BoundingBoxXYZ nBox = view3d.GetSectionBox();
                    nBox.Max = new XYZ(newMaxX, nBox.Max.Y, nBox.Max.Z);
                    view3d.SetSectionBox(nBox);
                    tran.Commit();
                }
            }
            catch { }
        }

        private void OffsetForward_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!double.TryParse(textOffsetForward.Text, out double forwardOffset)) return;

                View3D view3d = (View3D)_uiDoc.ActiveView;
                double centerPos = (_defaultMaxY + _defaultMinY) / 2;
                double length = (_defaultMaxY - _defaultMinY) / 2;

                double forwardFeet = UnitUtils.Convert(forwardOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                double newMinY = centerPos - length - forwardFeet;

                using (Transaction tran = new Transaction(_doc, _res.ResourceText("IDS_TRAN_OFFSET")))
                {
                    tran.Start();
                    BoundingBoxXYZ nBox = view3d.GetSectionBox();
                    nBox.Min = new XYZ(nBox.Min.X, newMinY, nBox.Min.Z);
                    view3d.SetSectionBox(nBox);
                    tran.Commit();
                }
            }
            catch { }
        }

        private void OffsetBack_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!double.TryParse(textOffsetBack.Text, out double backOffset)) return;

                View3D view3d = (View3D)_uiDoc.ActiveView;
                double centerPos = (_defaultMaxY + _defaultMinY) / 2;
                double length = (_defaultMaxY - _defaultMinY) / 2;

                double backFeet = UnitUtils.Convert(backOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                double newMaxY = centerPos + length + backFeet;

                using (Transaction tran = new Transaction(_doc, _res.ResourceText("IDS_TRAN_OFFSET")))
                {
                    tran.Start();
                    BoundingBoxXYZ nBox = view3d.GetSectionBox();
                    nBox.Max = new XYZ(nBox.Max.X, newMaxY, nBox.Max.Z);
                    view3d.SetSectionBox(nBox);
                    tran.Commit();
                }
            }
            catch { }
        }

        private void OffsetTop_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!double.TryParse(textOffsetTop.Text, out double topOffset)) return;

                View3D view3d = (View3D)_uiDoc.ActiveView;
                double centerPos = (_defaultMaxZ + _defaultMinZ) / 2;
                double length = (_defaultMaxZ - _defaultMinZ) / 2;

                double topFeet = UnitUtils.Convert(topOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                double newMaxZ = centerPos + length + topFeet;

                using (Transaction tran = new Transaction(_doc, _res.ResourceText("IDS_TRAN_OFFSET")))
                {
                    tran.Start();
                    BoundingBoxXYZ nBox = view3d.GetSectionBox();
                    nBox.Max = new XYZ(nBox.Max.X, nBox.Max.Y, newMaxZ);
                    view3d.SetSectionBox(nBox);
                    tran.Commit();
                }
            }
            catch { }
        }

        private void OffsetBottom_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!double.TryParse(textOffsetBottom.Text, out double bottomOffset)) return;

                View3D view3d = (View3D)_uiDoc.ActiveView;
                double centerPos = (_defaultMaxZ + _defaultMinZ) / 2;
                double length = (_defaultMaxZ - _defaultMinZ) / 2;

                double bottomFeet = UnitUtils.Convert(bottomOffset, UnitTypeId.Millimeters, UnitTypeId.Feet);
                double newMinZ = centerPos - length - bottomFeet;

                using (Transaction tran = new Transaction(_doc, _res.ResourceText("IDS_TRAN_OFFSET")))
                {
                    tran.Start();
                    BoundingBoxXYZ nBox = view3d.GetSectionBox();
                    nBox.Min = new XYZ(nBox.Min.X, nBox.Min.Y, newMinZ);
                    view3d.SetSectionBox(nBox);
                    tran.Commit();
                }
            }
            catch { }
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

            if (_isDarkTheme)
            {
                var bg = Brush("#263545");
                var titleBg = Brush("#263545");
                var divider = Brush("#3a4f63");
                var textColor = Brush("#e0e8f0");
                var subtextColor = Brush("#b0c4d8");
                var inputBg = Brush("#1a2633");
                var inputBorder = Brush("#3a4f63");
                var groupBorderColor = Brush("#3a4f63");
                var cancelBg = Brush("#3a4f63");

                outerBorder.Background = bg;
                titleBarGrid.Background = titleBg;
                dividerBorder.Background = divider;
                titleText.Foreground = textColor;
                btnClose.Foreground = Brush("#8fa4b8");
                footerBorder.Background = System.Windows.Media.Brushes.Transparent;

                groupBorder.BorderBrush = groupBorderColor;
                groupBorder.Background = Brush("#1e2f3f");
                groupTitle.Foreground = textColor;

                lblLeft.Foreground = subtextColor;
                lblRight.Foreground = subtextColor;
                lblFront.Foreground = subtextColor;
                lblBack.Foreground = subtextColor;
                lblTop.Foreground = subtextColor;
                lblBottom.Foreground = subtextColor;

                var textBoxStyle = new Style(typeof(WpfTextBox));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.BackgroundProperty, inputBg));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.ForegroundProperty, textColor));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.BorderBrushProperty, inputBorder));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.BorderThicknessProperty, new Thickness(1)));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.CaretBrushProperty, textColor));
                this.Resources[typeof(WpfTextBox)] = textBoxStyle;

                var tbStyle = new Style(typeof(System.Windows.Controls.TextBlock));
                tbStyle.Setters.Add(new Setter(System.Windows.Controls.TextBlock.ForegroundProperty, subtextColor));
                this.Resources[typeof(System.Windows.Controls.TextBlock)] = tbStyle;

                btnOk.Foreground = System.Windows.Media.Brushes.White;
                btnCancel.Foreground = textColor;

                this.Resources["CancelBorderBrush"] = groupBorderColor;
            }
            else
            {
                var bg = Brush("#ffffff");
                var titleBg = Brush("#f5f5f5");
                var divider = Brush("#e0e0e0");
                var textColor = Brush("#1e1e1e");
                var subtextColor = Brush("#5c5c5c");
                var inputBg = Brush("#ffffff");
                var inputBorder = Brush("#d0d0d0");
                var groupBorderColor = Brush("#d0d0d0");

                outerBorder.Background = bg;
                titleBarGrid.Background = titleBg;
                dividerBorder.Background = divider;
                titleText.Foreground = textColor;
                btnClose.Foreground = Brush("#5c5c5c");
                footerBorder.Background = System.Windows.Media.Brushes.Transparent;

                groupBorder.BorderBrush = groupBorderColor;
                groupBorder.Background = Brush("#fafafa");
                groupTitle.Foreground = textColor;

                lblLeft.Foreground = subtextColor;
                lblRight.Foreground = subtextColor;
                lblFront.Foreground = subtextColor;
                lblBack.Foreground = subtextColor;
                lblTop.Foreground = subtextColor;
                lblBottom.Foreground = subtextColor;

                var textBoxStyle = new Style(typeof(WpfTextBox));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.BackgroundProperty, inputBg));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.ForegroundProperty, textColor));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.BorderBrushProperty, inputBorder));
                textBoxStyle.Setters.Add(new Setter(WpfTextBox.BorderThicknessProperty, new Thickness(1)));
                this.Resources[typeof(WpfTextBox)] = textBoxStyle;

                btnOk.Foreground = System.Windows.Media.Brushes.White;
                btnCancel.Foreground = textColor;

                this.Resources["CancelBorderBrush"] = groupBorderColor;
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
            btnOk.ApplyTemplate();
            if (VisualTreeHelper.GetChildrenCount(btnOk) > 0)
            {
                var border = VisualTreeHelper.GetChild(btnOk, 0) as WpfBorder;
                if (border != null) border.Background = accent;
            }

            var cancelBorderBrush = _isDarkTheme ? Brush("#3a4f63") : Brush("#d0d0d0");
            var cancelBg = _isDarkTheme ? Brush("#263545") : Brush("#ffffff");
            btnCancel.ApplyTemplate();
            if (VisualTreeHelper.GetChildrenCount(btnCancel) > 0)
            {
                var border = VisualTreeHelper.GetChild(btnCancel, 0) as WpfBorder;
                if (border != null)
                {
                    border.Background = cancelBg;
                    border.BorderBrush = cancelBorderBrush;
                }
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Result = AdjustResult.Cancel;
            Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = AdjustResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = AdjustResult.Cancel;
            Close();
        }
    }
}
