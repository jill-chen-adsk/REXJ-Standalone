using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ADSK.JExtRAC.LocateSlab.Entities;
using ADSK.JExtRAC.LocateSlab.Utils;

namespace ADSK.JExtRAC.LocateSlab.Config
{
    public partial class FormConfigWPF : Window, IWeaveChromeWindow
    {
        private readonly Components.Attribute _cmpAttribute;
        private readonly DtSlabType _entDtSlabType;
        private readonly DtCmd _entDtCmd;

        public FormConfigWPF(Components.Attribute cmpAttribute, DtSlabType entDtSlabType, DtCmd entDtCmd)
        {
            InitializeComponent();
            Style = (Style)FindResource("Weave.ChromeWindow");

            _cmpAttribute = cmpAttribute;
            _entDtSlabType = entDtSlabType;
            _entDtCmd = entDtCmd;

            WeaveTheme.Apply(this, this, _cmpAttribute.ResourceText("IDS_TXT_LOCATESLAB")
                + string.Format(" [Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version));

            SetText();
            SetData();

            cboDirectionAngle.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(CboDirectionAngle_TextChanged));
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void SetText()
        {
            lblSlabType.Text = _cmpAttribute.ResourceText("IDS_TXT_SLABTYPE");
            lblHeightOffset.Text = _cmpAttribute.ResourceText("IDS_TXT_LEVELHEIGHTOFFSET");
            lblHeightOffsetUnit.Text = _cmpAttribute.ResourceText("IDS_UNIT_MM");
            lblDirectionAngle.Text = _cmpAttribute.ResourceText("IDS_TXT_DIRECTION");
            lblDegree.Text = _cmpAttribute.ResourceText("IDS_DEGREE");
            btnOK.Content = _cmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        private void SetData()
        {
            cboSlabType.ItemsSource = _entDtSlabType.Data.DefaultView;
            cboSlabType.DisplayMemberPath = _entDtSlabType.ColNameName;
            cboSlabType.SelectedValuePath = _entDtSlabType.ColNameID;

            cboDirectionAngle.ItemsSource = _entDtCmd.DataDirection.DefaultView;
            cboDirectionAngle.DisplayMemberPath = "Name";
            cboDirectionAngle.SelectedValuePath = "Value";

            if (_entDtCmd.Data != null && _entDtCmd.Data.Count >= 3)
            {
                double dValue = 0.0;
                if (double.TryParse(_entDtCmd.Data[0], out double ho)) dValue = ho;
                txtHeightOffset.Text = dValue.ToString();

                SelectSlabTypeByName(_entDtCmd.Data[1] ?? "");

                if (!string.IsNullOrEmpty(_entDtCmd.Data[2]))
                    cboDirectionAngle.Text = _entDtCmd.Data[2];
                else
                    cboDirectionAngle.Text = "0";
            }
            else
            {
                cboDirectionAngle.Text = "0";
            }

            if (cboSlabType.SelectedIndex < 0 && cboSlabType.Items.Count > 0)
                cboSlabType.SelectedIndex = 0;
        }

        private void SelectSlabTypeByName(string name)
        {
            DataView view = _entDtSlabType.Data.DefaultView;
            for (int i = 0; i < view.Count; i++)
            {
                if (view[i][_entDtSlabType.ColNameName] as string == name)
                {
                    cboSlabType.SelectedIndex = i;
                    return;
                }
            }
        }

        private void GetData()
        {
            if (_entDtCmd.Data != null && _entDtCmd.Data.Count >= 3)
            {
                double dValue = 0.0;
                if (double.TryParse(txtHeightOffset.Text, out double ho)) dValue = ho;
                _entDtCmd.Data[0] = dValue.ToString();

                _entDtCmd.Data[1] = (cboSlabType.SelectedItem as DataRowView)?[_entDtSlabType.ColNameName] as string ?? "";

                _entDtCmd.DegreeAngle = double.Parse(cboDirectionAngle.Text.Trim());
                _entDtCmd.Data[2] = _entDtCmd.DegreeAngle.ToString();
            }

            if (int.TryParse(cboSlabType.SelectedValue?.ToString() ?? "", out int elemId))
                _entDtSlabType.GetWorkElem(elemId);
        }

        private bool ValidateDirectionAngle()
        {
            string errMsg = _entDtCmd.SetErrPvdDecimalText(cboDirectionAngle.Text.Trim());
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
