using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ADSK.JExtRAC.AutomaticFloor.Utils;
using RvtExtApp = ADSK.JExtRAC.AutomaticFloor;

namespace ADSK.JExtRAC.AutomaticFloor.UI
{
    public partial class FormConfigWPF : Window, IWeaveChromeWindow
    {
        private readonly RvtExtApp.Components.Attribute _CmpAttribute;
        private readonly RvtExtApp.Entities.DtSlabType _EntDtSlabType;
        private readonly RvtExtApp.Entities.DtCmd _EntDtCmd;

        public FormConfigWPF(RvtExtApp.Components.Attribute cmpAttribute,
                             RvtExtApp.Entities.DtSlabType entDtSlabType,
                             RvtExtApp.Entities.DtCmd entDtCmd,
                             eFloorType eFloorType)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _CmpAttribute = cmpAttribute;
            _EntDtSlabType = entDtSlabType;
            _EntDtCmd = entDtCmd;

            WeaveTheme.Apply(this, this, GetTitle(eFloorType));

            SetText(eFloorType);
            SetData();
            SetIcon();

            cboDirectionAngle.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(CboDirectionAngle_TextChanged));
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private string GetTitle(eFloorType eFloorType)
        {
            string titleId = eFloorType switch
            {
                eFloorType.Arch => "IDS_TXT_ARCHITECTURE",
                eFloorType.Struct => "IDS_TXT_STRUCTURAL",
                _ => "IDS_TXT_FOUDATION_SLAB"
            };
            return _CmpAttribute.ResourceText(titleId)
                + string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void SetText(eFloorType eFloorType)
        {
            if (eFloorType == eFloorType.Arch)
            {
                lblSlabType.Text = _CmpAttribute.ResourceText("IDS_TXT_ARCHITECTURE_FTYPE");
                chbLock.Content = _CmpAttribute.ResourceText("IDS_TXT_LOCK_ARCHITECT");
            }
            else if (eFloorType == eFloorType.Struct)
            {
                lblSlabType.Text = _CmpAttribute.ResourceText("IDS_TXT_STRUCTURAL_FTYPE");
                chbLock.Content = _CmpAttribute.ResourceText("IDS_TXT_LOCK_STRUCTURAL");
            }
            else
            {
                lblSlabType.Text = _CmpAttribute.ResourceText("IDS_TXT_FOUDATION_SLAB_FTYPE");
                chbLock.Content = _CmpAttribute.ResourceText("IDS_TXT_LOCK_FOUNDATION_SLAB");
            }

            lblDirectionAngle.Text = _CmpAttribute.ResourceText("IDS_TXT_SLAB_DIRECTION");
            lblHeightOffset.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVELHEIGHTOFFSET");
            lblHeightOffsetUnit.Text = _CmpAttribute.ResourceText("IDS_UNIT_MM");
            lblDegree.Text = _CmpAttribute.ResourceText("IDS_DEGREE");
            btnOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        private void SetIcon()
        {
            if (_CmpAttribute.ResourceImage("IDI_SUBS_ICON") is not Icon icon)
                return;

            Icon = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void SetData()
        {
            cboSlabType.ItemsSource = _EntDtSlabType.Data.DefaultView;
            cboSlabType.DisplayMemberPath = _EntDtSlabType.ColNameName;
            cboSlabType.SelectedValuePath = _EntDtSlabType.ColNameID;

            cboDirectionAngle.ItemsSource = _EntDtCmd.DataDirection.DefaultView;
            cboDirectionAngle.DisplayMemberPath = "Name";
            cboDirectionAngle.SelectedValuePath = "Value";

            if (_EntDtCmd.Data.Count >= 4)
            {
                double dValue = 0.0;
                if (double.TryParse(_EntDtCmd.Data[0], out double parsedVal))
                    dValue = parsedVal;
                txtHeightOffset.Text = dValue.ToString();

                SelectSlabTypeByName(_EntDtCmd.Data[1] ?? "");

                chbLock.IsChecked = _EntDtCmd.Data[2] == "true";

                if (!string.IsNullOrEmpty(_EntDtCmd.Data[3]))
                    cboDirectionAngle.Text = _EntDtCmd.Data[3];
                else if (cboDirectionAngle.Items.Count > 0)
                    cboDirectionAngle.SelectedValue = "0";
            }
        }

        private void SelectSlabTypeByName(string name)
        {
            DataView view = _EntDtSlabType.Data.DefaultView;
            for (int i = 0; i < view.Count; i++)
            {
                if (view[i][_EntDtSlabType.ColNameName] as string == name)
                {
                    cboSlabType.SelectedIndex = i;
                    return;
                }
            }
        }

        private void GetData()
        {
            if (_EntDtCmd.Data.Count >= 4)
            {
                double dValue = 0.0;
                if (double.TryParse(txtHeightOffset.Text, out double parsedVal))
                    dValue = parsedVal;
                _EntDtCmd.Data[0] = dValue.ToString();

                _EntDtCmd.Data[1] = (cboSlabType.SelectedItem as DataRowView)?[_EntDtSlabType.ColNameName] as string ?? "";

                _EntDtCmd.Data[2] = chbLock.IsChecked == true ? "true" : "false";

                _EntDtCmd.DegreeAngle = double.Parse(cboDirectionAngle.Text.Trim());
                _EntDtCmd.Data[3] = _EntDtCmd.DegreeAngle.ToString();
            }

            int iValue = 0;
            if (int.TryParse(cboSlabType.SelectedValue?.ToString() ?? "0", out int parsedInt))
                iValue = parsedInt;
            _EntDtSlabType.GetWorkElem(iValue);
        }

        private bool ValidateDirectionAngle()
        {
            string errMsg = _EntDtCmd.SetErrPvdDecimalText(cboDirectionAngle.Text.Trim());
            bool isValid = string.IsNullOrEmpty(errMsg);

            lblDirectionAngleError.Text = errMsg;
            lblDirectionAngleError.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            cboDirectionAngle.SetResourceReference(
                Control.BorderBrushProperty,
                isValid ? "Weave.Brush.Border" : "Weave.Brush.Error");

            return isValid;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateDirectionAngle())
            {
                cboDirectionAngle.Focus();
                return;
            }

            GetData();
            DialogResult = true;
            Close();
        }

        private void TxtHeightOffset_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string current = txtHeightOffset.Text;
            foreach (char c in e.Text)
            {
                bool allowed = char.IsDigit(c)
                    || (c == '.' && current.IndexOf('.') < 0)
                    || (c == '-' && current.IndexOf('-') < 0);
                if (!allowed)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void CboDirectionAngle_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!cboDirectionAngle.IsKeyboardFocusWithin && !cboDirectionAngle.IsDropDownOpen)
                ValidateDirectionAngle();
        }

        private void CboDirectionAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lblDirectionAngleError.Visibility == Visibility.Visible)
                ValidateDirectionAngle();
        }
    }
}
