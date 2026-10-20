using ADSK.JExtRAC.FloorHeightDimension.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;

namespace ADSK.JExtRAC.FloorHeightDimension.UI
{
    public partial class FormConfigWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly RvtExtApp.Entities.DtCmd _EntDtCmd;
        private readonly Dictionary<Revit.DB.DimensionType, string> _Dic_dimensionType;
        private Revit.DB.DimensionType _SelectedType;

        public System.Windows.Forms.DialogResult LegacyDialogResult { get; private set; } =
            System.Windows.Forms.DialogResult.Cancel;

        public FormConfigWPF(
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Entities.DtCmd entDtCmd,
            Collections.Generic.IList<Revit.DB.DimensionType> list_dimensionType)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _CmpAttribute = cmpAttribute;
            _EntDtCmd = entDtCmd;
            _Dic_dimensionType = list_dimensionType.ToDictionary(x => x, x => x.Name);

            string title = _CmpAttribute.ResourceText("IDS_TXT_FLOORHEIGHTDIMENSION")
                + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            WeaveTheme.Apply(this, this, title, () =>
            {
                LegacyDialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            });

            SetText();
            SetData();
            SetIcon();
            SetDiagram();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public System.Windows.Controls.Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public System.Windows.Controls.Button ChromeCloseButton => chromeCloseButton;

        public Revit.DB.DimensionType GetSelectDimensionType => _SelectedType;

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(
                this,
                this,
                _CmpAttribute.ResourceText("IDS_TXT_FLOORHEIGHTDIMENSION")
                    + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version));

            gpbDist.Text = _CmpAttribute.ResourceText("IDS_TXT_GROUP");
            lblDist.Text = _CmpAttribute.ResourceText("IDS_TXT_DIST");
            lblDistUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");
            lblTotalDist.Text = _CmpAttribute.ResourceText("IDS_TXT_TOTALDIST");
            lblTotalDistUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");
            lblType.Text = _CmpAttribute.ResourceText("IDS_TXT_TYPE");
            btnSelPos.Content = _CmpAttribute.ResourceText("IDS_TXT_SELPOS");
            btnOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            ckbMultiView.Content = _CmpAttribute.ResourceText("IDS_TXT_MULTIVIEW");
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

        private void SetDiagram()
        {
            diagramVector.Child = LevelDiagram.Create(
                (System.Windows.Media.Brush)FindResource("Weave.Brush.Text.Primary"),
                (System.Windows.Media.Brush)FindResource("Weave.Brush.Diagram.Accent"),
                (System.Windows.Media.FontFamily)FindResource("Weave.Font.Family"),
                _CmpAttribute.ResourceText("IDS_TXT_DIAGRAM_LEVEL"),
                _CmpAttribute.ResourceText("IDS_TXT_DIAGRAM_BASEPOINT"));
        }

        private static bool TryParseDouble(string s, out double d) =>
            double.TryParse(s?.Trim(), NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out d);

        private void SetData()
        {
            cbDimensionType.ItemsSource = _Dic_dimensionType.ToList();
            cbDimensionType.DisplayMemberPath = "Value";
            cbDimensionType.SelectedValuePath = "Key";

            if (_EntDtCmd.Data.Count >= 4)
            {
                if (TryParseDouble(_EntDtCmd.Data[0], out double dist))
                    txtDist.Text = dist.ToString(CultureInfo.CurrentCulture);
                else
                    txtDist.Text = "0";

                if (TryParseDouble(_EntDtCmd.Data[1], out double total))
                    txtTotalDist.Text = total.ToString(CultureInfo.CurrentCulture);
                else
                    txtTotalDist.Text = "0";

                if (bool.TryParse(_EntDtCmd.Data[2], out bool multi))
                    ckbMultiView.IsChecked = multi;

                string typeName = _EntDtCmd.Data[3];
                if (!string.IsNullOrEmpty(typeName))
                {
                    int index = _Dic_dimensionType.Values.ToList().IndexOf(typeName);
                    if (index >= 0)
                        cbDimensionType.SelectedIndex = index;
                }
            }

            // Without a type the command creates nothing, so never leave the combo empty.
            if (cbDimensionType.SelectedIndex < 0 && _Dic_dimensionType.Count > 0)
                cbDimensionType.SelectedIndex = 0;
        }

        private void GetData()
        {
            if (_EntDtCmd.Data.Count < 4)
                return;

            if (TryParseDouble(txtDist.Text, out double dist))
                _EntDtCmd.Data[0] = dist.ToString(CultureInfo.InvariantCulture);

            if (TryParseDouble(txtTotalDist.Text, out double total))
                _EntDtCmd.Data[1] = total.ToString(CultureInfo.InvariantCulture);

            _EntDtCmd.Data[2] = (ckbMultiView.IsChecked == true).ToString();

            _SelectedType = cbDimensionType.SelectedValue as Revit.DB.DimensionType;
            if (_SelectedType != null)
                _EntDtCmd.Data[3] = _SelectedType.Name;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            LegacyDialogResult = System.Windows.Forms.DialogResult.OK;
            DialogResult = true;
            Close();
        }

        private void BtnSelPos_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            LegacyDialogResult = System.Windows.Forms.DialogResult.Yes;
            DialogResult = true;
            Close();
        }
    }
}
