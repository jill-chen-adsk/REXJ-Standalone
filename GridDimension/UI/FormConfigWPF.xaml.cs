using ADSK.JExtRAC.GridDimension.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.GridDimension;

namespace ADSK.JExtRAC.GridDimension.UI
{
    public partial class FormConfigWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly RvtExtApp.Components.Geometry _CmpGeometry;
        private readonly RvtExtApp.Entities.DtCmd _EntDtCmd;
        private readonly Collections.Generic.Dictionary<Revit.DB.DimensionType, string> _Dic_dimensionType;
        private Revit.DB.DimensionType _SelectedType;
        private readonly int _optDirection;
        private readonly bool _isCurveDim;

        private bool _SelectedLeft;
        private bool _SelectedRight;
        private bool _SelectedTop;
        private bool _SelectedBottom;

        private static readonly System.Drawing.Color DarkNeutralInk = System.Drawing.Color.FromArgb(0xE4, 0xEB, 0xF2);
        private static readonly System.Drawing.Color DarkAccentInk = System.Drawing.Color.FromArgb(0xFF, 0x7B, 0x84);

        private const double InkCoverageFloor = 0.08;
        private const double AccentSaturation = 0.25;
        private const double NeutralInkGamma = 0.55;
        private const double AccentInkGamma = 0.45;

        public FormConfigWPF(
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Components.Geometry cmpGeometry,
            RvtExtApp.Entities.DtCmd entDtCmd,
            Collections.Generic.IList<Revit.DB.DimensionType> list_dimensionType,
            bool isCurveDim,
            int optDirection)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _CmpAttribute = cmpAttribute;
            _CmpGeometry = cmpGeometry;
            _EntDtCmd = entDtCmd;
            _Dic_dimensionType = list_dimensionType.ToDictionary(x => x, x => x.Name);
            _isCurveDim = isCurveDim;
            _optDirection = optDirection;

            string title = _CmpAttribute.ResourceText("IDS_TXT_GRIDDIMENSION")
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

        public Revit.DB.DimensionType GetSelectDimensionType => _SelectedType;

        public bool GetSelectedCheckBox()
        {
            if (!_SelectedLeft && !_SelectedRight && !_SelectedTop && !_SelectedBottom)
                return true;
            return false;
        }

        public bool GetSelectLeft => _SelectedLeft;
        public bool GetSelectRight => _SelectedRight;
        public bool GetSelectTop => _SelectedTop;
        public bool GetSelectBottom => _SelectedBottom;

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(
                this,
                this,
                _CmpAttribute.ResourceText("IDS_TXT_GRIDDIMENSION")
                    + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version));

            gpbDist.Text = _CmpAttribute.ResourceText("IDS_TXT_DIST");
            lblA.Text = _CmpAttribute.ResourceText("IDS_TXT_A");
            lblAUnit.Text = _CmpGeometry.LengthUnitLabel;
            lblB.Text = _CmpAttribute.ResourceText("IDS_TXT_B");
            lblBUnit.Text = _CmpGeometry.LengthUnitLabel;
            ckbMultiView.Content = _CmpAttribute.ResourceText("IDS_TXT_MULTIVIEW");
            lblType.Text = _CmpAttribute.ResourceText("IDS_TXT_TYPE");
            btnOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            SetDiagram();
        }

        private void SetDiagram()
        {
            if (!_isCurveDim)
            {
                diagramVector.Child = GridDiagram.CreateLinear(
                    (System.Windows.Media.Brush)FindResource("Weave.Brush.Text.Primary"),
                    (System.Windows.Media.Brush)FindResource("Weave.Brush.Diagram.Accent"));
                diagramVector.Visibility = Visibility.Visible;
                picDist.Visibility = Visibility.Collapsed;
                return;
            }

            // The curved-grid artwork is still a bitmap. Re-ink it for the dark palette
            // and show it at its native pixel size so nothing is resampled.
            ImageSource diagram = ToImageSource(
                _CmpAttribute.ResourceImage("IDI_PIC_GRID_CURVE") as System.Drawing.Image,
                WeaveTheme.IsDarkTheme);

            picDist.Source = diagram;

            if (diagram is BitmapSource bitmap)
            {
                picDist.Width = bitmap.PixelWidth;
                picDist.Height = bitmap.PixelHeight;
            }

            diagramVector.Visibility = Visibility.Collapsed;
            picDist.Visibility = Visibility.Visible;
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
            string sValue = string.Empty;
            bool checkValue = false;
            bool useLegacyMillimeterStorage = _CmpGeometry.UsesLegacyMillimeterStorage(_EntDtCmd.Data);

            cbDimensionType.ItemsSource = _Dic_dimensionType.ToList();
            cbDimensionType.DisplayMemberPath = "Value";
            cbDimensionType.SelectedValuePath = "Key";

            if (_EntDtCmd.Data.Count >= 4)
            {
                if (!_isCurveDim)
                {
                    txtA.Text = _CmpGeometry.ConvertStoredDistanceForDisplay(_EntDtCmd.Data[0], useLegacyMillimeterStorage);
                }
                else
                {
                    txtA.Text = string.Empty;
                    txtA.IsEnabled = false;
                }

                sValue = _CmpGeometry.ConvertStoredDistanceForDisplay(_EntDtCmd.Data[1], useLegacyMillimeterStorage);
                if (string.IsNullOrWhiteSpace(sValue) && _isCurveDim)
                    sValue = _CmpGeometry.FormatDisplayLength(1.0);
                txtB.Text = sValue;

                sValue = _EntDtCmd.Data[2];
                if (bool.TryParse(sValue, out bool cb))
                    checkValue = cb;
                ckbMultiView.IsChecked = checkValue;

                sValue = _EntDtCmd.Data[3];
                if (!string.IsNullOrEmpty(sValue))
                {
                    int index = _Dic_dimensionType.Values.ToList().IndexOf(sValue);
                    if (index >= 0)
                        cbDimensionType.SelectedIndex = index;
                }

                sValue = _EntDtCmd.Data[4];
                ckbLeft.IsChecked = sValue == "True";

                sValue = _EntDtCmd.Data[5];
                ckbRight.IsChecked = sValue == "True";

                sValue = _EntDtCmd.Data[6];
                ckbTop.IsChecked = sValue == "True";

                sValue = _EntDtCmd.Data[7];
                ckbBottom.IsChecked = sValue == "True";

                if (!_isCurveDim)
                {
                    if (_optDirection == 0)
                    {
                        ckbBottom.IsEnabled = false;
                        ckbTop.IsEnabled = false;
                        ckbBottom.IsChecked = false;
                        ckbTop.IsChecked = false;
                    }
                    else if (_optDirection == 1)
                    {
                        ckbLeft.IsEnabled = false;
                        ckbRight.IsEnabled = false;
                        ckbLeft.IsChecked = false;
                        ckbRight.IsChecked = false;
                    }
                }
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetData())
            {
                DialogResult = true;
                Close();
            }
        }

        private bool TryGetData()
        {
            IntPtr ownerHandle = new WindowInteropHelper(this).Handle;

            if (_EntDtCmd.Data.Count < 6)
                return false;

            if (!_CmpGeometry.TryParseDisplayLength(txtA.Text, out double distAInternal))
                distAInternal = 0.0;

            if (distAInternal < 0.0)
            {
                WeaveDialogHost.ShowMessage(
                    ownerHandle,
                    _CmpAttribute.ResourceText("IDS_TXT_ANO") + _CmpAttribute.ResourceText("IDS_ERR_VALMORE0"),
                    _CmpAttribute.ResourceText("IDS_TXT_ERROR"),
                    _CmpAttribute.ResourceText("IDS_TXT_OK"));
                return false;
            }

            _EntDtCmd.Data[0] = _CmpGeometry.FormatStoredLength(
                Revit.DB.UnitUtils.ConvertFromInternalUnits(distAInternal, _CmpGeometry.LengthUnitTypeId));

            if (!_CmpGeometry.TryParseDisplayLength(txtB.Text, out double distBInternal))
                distBInternal = 0.0;

            if (_isCurveDim)
            {
                double minInternal = Revit.DB.UnitUtils.ConvertToInternalUnits(1.0, _CmpGeometry.LengthUnitTypeId);
                if (distBInternal < minInternal)
                    distBInternal = minInternal;
            }

            if (distBInternal < 0.0)
            {
                WeaveDialogHost.ShowMessage(
                    ownerHandle,
                    _CmpAttribute.ResourceText("IDS_TXT_BNO") + _CmpAttribute.ResourceText("IDS_ERR_VALLARGE0"),
                    _CmpAttribute.ResourceText("IDS_TXT_ERROR"),
                    _CmpAttribute.ResourceText("IDS_TXT_OK"));
                return false;
            }

            _EntDtCmd.Data[1] = _CmpGeometry.FormatStoredLength(
                Revit.DB.UnitUtils.ConvertFromInternalUnits(distBInternal, _CmpGeometry.LengthUnitTypeId));
            _EntDtCmd.Data[2] = (ckbMultiView.IsChecked == true).ToString();

            _SelectedType = cbDimensionType.SelectedValue as Revit.DB.DimensionType;
            if (_SelectedType != null)
                _EntDtCmd.Data[3] = _SelectedType.Name;

            _SelectedLeft = ckbLeft.IsChecked == true;
            _SelectedRight = ckbRight.IsChecked == true;
            _SelectedTop = ckbTop.IsChecked == true;
            _SelectedBottom = ckbBottom.IsChecked == true;

            _EntDtCmd.Data[4] = _SelectedLeft ? "True" : "False";
            _EntDtCmd.Data[5] = _SelectedRight ? "True" : "False";
            _EntDtCmd.Data[6] = _SelectedTop ? "True" : "False";
            _EntDtCmd.Data[7] = _SelectedBottom ? "True" : "False";

            while (_EntDtCmd.Data.Count <= RvtExtApp.Components.Geometry.DistanceStorageVersionIndex)
                _EntDtCmd.Data.Add(string.Empty);

            _EntDtCmd.Data[RvtExtApp.Components.Geometry.DistanceStorageVersionIndex] =
                RvtExtApp.Components.Geometry.DistanceStorageVersion.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return true;
        }

        private static ImageSource ToImageSource(System.Drawing.Image image, bool forDarkTheme)
        {
            if (image == null)
                return null;

            if (forDarkTheme)
            {
                using var source = new Bitmap(image);
                using var adapted = AdaptDiagramForDarkTheme(source);
                return ToImageSourceCore(adapted);
            }

            return ToImageSourceCore(image);
        }

        private static Bitmap AdaptDiagramForDarkTheme(Bitmap source)
        {
            var adapted = new Bitmap(
                source.Width,
                source.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    System.Drawing.Color pixel = source.GetPixel(x, y);
                    adapted.SetPixel(x, y, AdaptDiagramPixel(pixel));
                }
            }

            return adapted;
        }

        private static System.Drawing.Color AdaptDiagramPixel(System.Drawing.Color pixel)
        {
            // The artwork is ink printed on a white page. Recovering the ink coverage
            // and re-inking it keeps anti-aliased edges at their true weight instead of
            // turning the pale halo around each glyph into an opaque blob.
            int minChannel = Math.Min(pixel.R, Math.Min(pixel.G, pixel.B));
            double coverage = (255.0 - minChannel) / 255.0;

            if (coverage < InkCoverageFloor)
                return System.Drawing.Color.FromArgb(0, 0, 0, 0);

            bool isAccent = pixel.R - Math.Max(pixel.G, pixel.B) > AccentSaturation * (255.0 - minChannel);
            System.Drawing.Color ink = isAccent ? DarkAccentInk : DarkNeutralInk;

            double alpha = Math.Pow(coverage, isAccent ? AccentInkGamma : NeutralInkGamma);
            int opacity = Math.Max(0, Math.Min(255, (int)Math.Round(alpha * 255.0)));

            return System.Drawing.Color.FromArgb(opacity, ink.R, ink.G, ink.B);
        }

        private static ImageSource ToImageSourceCore(System.Drawing.Image image)
        {
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = memoryStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
