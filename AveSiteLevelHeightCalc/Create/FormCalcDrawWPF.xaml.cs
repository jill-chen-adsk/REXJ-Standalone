using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Create
{
    public partial class FormCalcDrawWPF : Window, IWeaveChromeWindow
    {
        readonly RvtExtApp.Components.Attribute _cmpAttribute;
        readonly RvtExtApp.Entities.DtAnnotation _entDtAnnotation;
        readonly RvtExtApp.Entities.DtCmd _entDtCmd;
        readonly IntPtr _ownerHandle;
        string _bmHeightOld = string.Empty;

        public FormCalcDrawWPF(
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Entities.DtAnnotation entDtAnnotation,
            RvtExtApp.Entities.DtCmd entDtCmd,
            IntPtr ownerHandle)
        {
            InitializeComponent();

            _cmpAttribute = cmpAttribute;
            _entDtAnnotation = entDtAnnotation;
            _entDtCmd = entDtCmd;
            _ownerHandle = ownerHandle;

            string title = _cmpAttribute.ResourceText("IDS_TXT_CREATEAVEGLLEVELDRAW")
                + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);
            WeaveTheme.Apply(this, this, title, CancelDialog);

            SetText();
            ConfigureGridColumns();
            SetData();
        }

        public System.Windows.Forms.DialogResult WinFormsDialogResult { get; private set; }
            = System.Windows.Forms.DialogResult.Cancel;

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        void SetText()
        {
            lblCalcPoint.Text = _cmpAttribute.ResourceText("IDS_TXT_CALCPOINT");
            btnUp.Content = _cmpAttribute.ResourceText("IDS_TXT_UPSIGN");
            btnDn.Content = _cmpAttribute.ResourceText("IDS_TXT_DNSIGN");
            btnDel.Content = _cmpAttribute.ResourceText("IDS_TXT_DEL");
            btnUpdateNumber.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATENUMBER");
            lblBMHeight.Text = _cmpAttribute.ResourceText("IDS_TXT_BMHEIGHT");
            btnUpdateLevel.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATELEVEL");
            lblCalcDraw.Text = _cmpAttribute.ResourceText("IDS_TXT_CALCDRAW");
            lblScale.Text = _cmpAttribute.ResourceText("IDS_TXT_SCALE");
            lbl1Slash.Text = _cmpAttribute.ResourceText("IDS_TXT_1SLASH");
            lblRate.Text = _cmpAttribute.ResourceText("IDS_TXT_VHRATE");
            lblHorizontal.Text = _cmpAttribute.ResourceText("IDS_TXT_HORIZONTAL");
            lblVertical.Text = _cmpAttribute.ResourceText("IDS_TXT_VERTICAL");
            lblLengthUnit.Text = _cmpAttribute.ResourceText("IDS_TXT_UNITLENGTH");
            rdbLengthMM.Content = _cmpAttribute.ResourceText("IDS_TXT_MM");
            rdbLengthM.Content = _cmpAttribute.ResourceText("IDS_TXT_M");
            lblAreaDecimalSection.Text = _cmpAttribute.ResourceText("IDS_TXT_AREADECIMAL");
            lblAreaDecimal.Text = _cmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            lblAreaOrder.Text = _cmpAttribute.ResourceText("IDS_TXT_ORDER");
            rdbAreaCut.Content = _cmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            rdbAreaClose.Content = _cmpAttribute.ResourceText("IDS_TXT_CLOSE1");
            rdbAreaRounding.Content = _cmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            btnCreate.Content = _cmpAttribute.ResourceText("IDS_TXT_CREATECALCDRAW");
            btnClose.Content = _cmpAttribute.ResourceText("IDS_TXT_CLOSE");
            btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        void ConfigureGridColumns()
        {
            dgvCalcPoint.Columns.Clear();

            var numberColumn = new DataGridTextColumn
            {
                Header = _cmpAttribute.ResourceText("IDS_TXT_NUMBER"),
                Binding = new Binding("Number"),
                IsReadOnly = true,
                Width = 56,
                ElementStyle = (Style)FindResource("Weave.DataGridTextBlock.Left")
            };
            dgvCalcPoint.Columns.Add(numberColumn);

            var levelColumn = new DataGridTextColumn
            {
                Header = _cmpAttribute.ResourceText("IDS_TXT_LEVELFROMBM"),
                Binding = new Binding("Level") { UpdateSourceTrigger = UpdateSourceTrigger.LostFocus },
                IsReadOnly = false,
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                ElementStyle = (Style)FindResource("Weave.DataGridTextBlock.Right"),
                EditingElementStyle = (Style)Resources[typeof(TextBox)]
            };
            dgvCalcPoint.Columns.Add(levelColumn);
        }

        void SetData()
        {
            txtBMHeight.Text = _entDtAnnotation.BMHeight.ToString();
            _bmHeightOld = txtBMHeight.Text;
            txtScale.Text = _entDtAnnotation.Scale.ToString();
            txtHorizontal.Text = _entDtAnnotation.RaiteHorizontal.ToString();
            txtVertical.Text = _entDtAnnotation.RaiteVertical.ToString();
            dgvCalcPoint.ItemsSource = _entDtAnnotation.TableAveGlLvlCalcPos.DefaultView;
            txtAreaDecimal.Text = _entDtAnnotation.AreaDecimal.ToString();
            RdbAreaRounding = _entDtAnnotation.AreaRoundingOpt;
            RdbLengthUnit = _entDtAnnotation.LengthUnit;
        }

        void GetData()
        {
            _entDtAnnotation.BMHeight = double.Parse(txtBMHeight.Text);
            _entDtAnnotation.Scale = int.Parse(txtScale.Text);
            _entDtAnnotation.RaiteHorizontal = int.Parse(txtHorizontal.Text);
            _entDtAnnotation.RaiteVertical = int.Parse(txtVertical.Text);
            _entDtAnnotation.AreaDecimal = int.Parse(txtAreaDecimal.Text);
            _entDtAnnotation.AreaRoundingOpt = RdbAreaRounding;
            _entDtAnnotation.LengthUnit = RdbLengthUnit;

            _entDtCmd.Data[0] = _entDtAnnotation.BMHeight.ToString();
            _entDtCmd.Data[1] = _entDtAnnotation.Scale.ToString();
            _entDtCmd.Data[2] = _entDtAnnotation.RaiteHorizontal.ToString();
            _entDtCmd.Data[3] = _entDtAnnotation.RaiteVertical.ToString();
            _entDtCmd.Data[4] = _entDtAnnotation.AreaDecimal.ToString();
            _entDtCmd.Data[5] = _entDtAnnotation.AreaRoundingOpt.ToString();
            _entDtCmd.Data[6] = _entDtAnnotation.LengthUnit.ToString();
        }

        int RdbAreaRounding
        {
            get
            {
                if (rdbAreaClose.IsChecked == true)
                    return 1;
                if (rdbAreaRounding.IsChecked == true)
                    return 2;
                return 0;
            }
            set
            {
                rdbAreaCut.IsChecked = value == 0;
                rdbAreaClose.IsChecked = value == 1;
                rdbAreaRounding.IsChecked = value == 2;
            }
        }

        int RdbLengthUnit
        {
            get => rdbLengthM.IsChecked == true ? 1 : 0;
            set
            {
                rdbLengthMM.IsChecked = value == 0;
                rdbLengthM.IsChecked = value == 1;
            }
        }

        void CommitGridEdit()
        {
            dgvCalcPoint.CommitEdit(DataGridEditingUnit.Cell, true);
            dgvCalcPoint.CommitEdit(DataGridEditingUnit.Row, true);
        }

        bool GetErrPvd(int mode)
        {
            if (!string.IsNullOrEmpty(GetFieldError(txtBMHeight))
                || !string.IsNullOrEmpty(GetFieldError(txtScale))
                || !string.IsNullOrEmpty(GetFieldError(txtHorizontal))
                || !string.IsNullOrEmpty(GetFieldError(txtVertical))
                || !string.IsNullOrEmpty(GetFieldError(txtAreaDecimal)))
            {
                return false;
            }

            if (mode < 1 && !GetErrPvdLevel())
                return false;

            return true;
        }

        void ChkErrPvd(int mode)
        {
            CheckBMHeight();
            SetFieldError(txtScale, _entDtAnnotation.SetErrPvdValue(txtScale.Text.Trim(), 0, 1));
            SetFieldError(txtHorizontal, _entDtAnnotation.SetErrPvdValue(txtHorizontal.Text.Trim(), 0, 1));
            SetFieldError(txtVertical, _entDtAnnotation.SetErrPvdValue(txtVertical.Text.Trim(), 0, 1));
            SetFieldError(txtAreaDecimal, _entDtAnnotation.SetErrPvdDecimalText(txtAreaDecimal.Text.Trim()));

            if (mode < 1)
                ChkErrPvdLevel();
        }

        void ChkUpdateBMHeight()
        {
            if (txtBMHeight.Text == _bmHeightOld)
                return;

            var confirm = new WeaveConfirmDialog(
                _cmpAttribute.ResourceText("IDS_WAR_UPDATELEVEL"),
                Title,
                _cmpAttribute.ResourceText("IDS_TXT_UPDATELEVEL"),
                _cmpAttribute.ResourceText("IDS_TXT_CANCEL"));
            WeaveDialogHost.ShowDialog(confirm, _ownerHandle);

            if (confirm.DialogResult == true)
                UpdateBMHeight();
            else
                txtBMHeight.Text = _bmHeightOld;
        }

        bool UpdateBMHeight()
        {
            _entDtAnnotation.UpdateLevelViewTable(txtBMHeight.Text, _bmHeightOld);
            _bmHeightOld = txtBMHeight.Text;
            dgvCalcPoint.Items.Refresh();
            return true;
        }

        void ChkErrPvdLevel()
        {
            if (dgvCalcPoint.Items.Count == 0)
                return;

            for (int i = 0; i < dgvCalcPoint.Items.Count; ++i)
            {
                if (dgvCalcPoint.Items[i] is System.Data.DataRowView rowView)
                {
                    string value = rowView["Level"]?.ToString() ?? string.Empty;
                    string error = _entDtAnnotation.SetErrPvdValue(value, 1, 0);
                    SetRowLevelError(i, error);
                }
            }
        }

        bool GetErrPvdLevel()
        {
            for (int i = 0; i < dgvCalcPoint.Items.Count; ++i)
            {
                if (!string.IsNullOrEmpty(GetRowLevelError(i)))
                    return false;
            }
            return true;
        }

        void SetFieldError(TextBox control, string error)
        {
            control.ToolTip = string.IsNullOrEmpty(error) ? null : error;
            control.BorderBrush = string.IsNullOrEmpty(error)
                ? (System.Windows.Media.Brush)FindResource("Weave.Brush.Border")
                : System.Windows.Media.Brushes.IndianRed;
        }

        string GetFieldError(TextBox control) =>
            control.ToolTip as string ?? string.Empty;

        readonly Dictionary<int, string> _levelErrors = new Dictionary<int, string>();

        void SetRowLevelError(int rowIndex, string error)
        {
            if (string.IsNullOrEmpty(error))
                _levelErrors.Remove(rowIndex);
            else
                _levelErrors[rowIndex] = error;
        }

        string GetRowLevelError(int rowIndex) =>
            _levelErrors.TryGetValue(rowIndex, out string error) ? error : string.Empty;

        void CheckBMHeight()
        {
            SetFieldError(txtBMHeight, _entDtAnnotation.SetErrPvdValue(txtBMHeight.Text.Trim(), 1, 0));

            if (!string.IsNullOrEmpty(GetFieldError(txtBMHeight)))
                return;

            string bmLevelNew = txtBMHeight.Text.Trim();
            if (!decimal.TryParse(bmLevelNew, out decimal value)
                || value < decimal.MinValue
                || value > decimal.MaxValue)
            {
                SetFieldError(txtBMHeight, _cmpAttribute.ResourceText("IDS_ERROR_INVALID_LEVEL_VALUE"));
            }
        }

        bool TrySubmit(System.Windows.Forms.DialogResult result)
        {
            CommitGridEdit();
            ChkErrPvd(0);
            if (!GetErrPvd(0))
                return false;

            ChkUpdateBMHeight();
            GetData();
            WinFormsDialogResult = result;
            DialogResult = true;
            Close();
            return true;
        }

        void CancelDialog()
        {
            WinFormsDialogResult = System.Windows.Forms.DialogResult.Cancel;
            DialogResult = false;
            Close();
        }

        void BtnCreate_Click(object sender, RoutedEventArgs e) =>
            TrySubmit(System.Windows.Forms.DialogResult.Yes);

        void BtnClose_Click(object sender, RoutedEventArgs e) =>
            TrySubmit(System.Windows.Forms.DialogResult.No);

        void BtnCancel_Click(object sender, RoutedEventArgs e) =>
            CancelDialog();

        void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            CommitGridEdit();
            int index = dgvCalcPoint.Items.IndexOf(dgvCalcPoint.CurrentItem);
            if (index > 0)
            {
                _entDtAnnotation.UpDnViewTable(index, true);
                dgvCalcPoint.Items.Refresh();
                dgvCalcPoint.SelectedIndex = index - 1;
            }
        }

        void BtnDn_Click(object sender, RoutedEventArgs e)
        {
            CommitGridEdit();
            int index = dgvCalcPoint.Items.IndexOf(dgvCalcPoint.CurrentItem);
            if (index >= 0 && index < dgvCalcPoint.Items.Count - 1)
            {
                _entDtAnnotation.UpDnViewTable(index, false);
                dgvCalcPoint.Items.Refresh();
                dgvCalcPoint.SelectedIndex = index + 1;
            }
        }

        void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            CommitGridEdit();
            int index = dgvCalcPoint.Items.IndexOf(dgvCalcPoint.CurrentItem);
            if (index >= 0)
            {
                _entDtAnnotation.DelViewTable(index);
                dgvCalcPoint.Items.Refresh();
            }
        }

        void BtnUpdateNumber_Click(object sender, RoutedEventArgs e)
        {
            CommitGridEdit();
            _entDtAnnotation.UpdateNumberViewTable();
            dgvCalcPoint.Items.Refresh();
        }

        void BtnUpdateLevel_Click(object sender, RoutedEventArgs e)
        {
            ChkErrPvd(1);
            if (!GetErrPvd(1))
                return;

            if (!UpdateBMHeight())
                return;

            ChkErrPvdLevel();
        }

        void TxtBMHeight_LostFocus(object sender, RoutedEventArgs e) => CheckBMHeight();

        void TxtScale_LostFocus(object sender, RoutedEventArgs e) =>
            SetFieldError(txtScale, _entDtAnnotation.SetErrPvdValue(txtScale.Text.Trim(), 0, 1));

        void TxtHorizontal_LostFocus(object sender, RoutedEventArgs e) =>
            SetFieldError(txtHorizontal, _entDtAnnotation.SetErrPvdValue(txtHorizontal.Text.Trim(), 0, 1));

        void TxtVertical_LostFocus(object sender, RoutedEventArgs e) =>
            SetFieldError(txtVertical, _entDtAnnotation.SetErrPvdValue(txtVertical.Text.Trim(), 0, 1));

        void TxtAreaDecimal_LostFocus(object sender, RoutedEventArgs e) =>
            SetFieldError(txtAreaDecimal, _entDtAnnotation.SetErrPvdDecimalText(txtAreaDecimal.Text.Trim()));

        void DgvCalcPoint_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex != 1)
                return;

            int rowIndex = e.Row.GetIndex();
            if (e.EditingElement is TextBox textBox)
            {
                string error = _entDtAnnotation.SetErrPvdValue(textBox.Text.Trim(), 1, 0);
                SetRowLevelError(rowIndex, error);
                e.Cancel = !string.IsNullOrEmpty(error);
            }
        }
    }
}
