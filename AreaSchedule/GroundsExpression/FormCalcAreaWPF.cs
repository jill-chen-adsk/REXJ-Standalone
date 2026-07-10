
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Reflection;

using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.GroundsExpression
{
    /// ================================================================================
    /// <summary>画面 面積計算 (WPF版)</summary>
    /// <history>2025/05/07 Created</history>
    /// ================================================================================
    public partial class FormCalcAreaWPF : Window, IWeaveChromeWindow
    {
        #region Member Variables

        private RvtExtApp.Components.Attribute _CmpAttribute;
        private string _MLengthDecimal;
        private RvtExtApp.Entities.DtArea _EntDtArea;
        private RvtExtApp.Entities.DtCmd _EntDtCmd;
        private bool _isCheckRdb;

        #endregion

        #region Constructor

        public FormCalcAreaWPF(RvtExtApp.Components.Attribute cmpAttribute,
                              RvtExtApp.Entities.DtArea entDtArea,
                              RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _EntDtArea = entDtArea;
            _EntDtCmd = entDtCmd;
            _MLengthDecimal = "1";

            WeaveTheme.Apply(this, this, WeaveCommandTitles.GroundsExpression(_CmpAttribute), CancelDialog);

            btnLengthDefault.Command = new RoutedCommand();
            btnLengthDefault.CommandBindings.Add(new CommandBinding(btnLengthDefault.Command, BtnLengthDefault_Click));

            btnAreaDefault.Command = new RoutedCommand();
            btnAreaDefault.CommandBindings.Add(new CommandBinding(btnAreaDefault.Command, BtnAreaDefault_Click));

            btnCancel.Command = new RoutedCommand();
            btnCancel.CommandBindings.Add(new CommandBinding(btnCancel.Command, BtnCancel_Click));

            Loaded += FormCalcAreaWPF_Loaded;
            btnOK.Click += BtnOK_Click;
            btnLengthDefault.Click += BtnLengthDefault_Click;
            btnAreaDefault.Click += BtnAreaDefault_Click;
            rdbLengthProject.Checked += RdbLengthUnit_Checked;
            rdbLengthMeters.Checked += RdbLengthUnit_Checked;
            txtLengthDecimal.TextChanged += TxtLengthDecimal_TextChanged;
            txtAreaDecimal.TextChanged += TxtAreaDecimal_TextChanged;
        }

        #endregion

        #region Event Handlers

        private void FormCalcAreaWPF_Loaded(object sender, RoutedEventArgs e)
        {
            SetText();
            SetData();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                GetData();
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, ExecutedRoutedEventArgs e)
        {
            CancelDialog();
        }

        private void CancelDialog()
        {
            DialogResult = false;
            Close();
        }

        public Border ChromeOuterBorder => chromeOuterBorder;
        public Grid ChromeTitleBar => chromeTitleBar;
        public Border ChromeDivider => chromeDivider;
        public TextBlock ChromeTitleText => chromeTitleText;
        public Button ChromeCloseButton => chromeCloseButton;

        private void BtnLengthDefault_Click(object sender, RoutedEventArgs e)
        {
            _EntDtArea.Initvalue(1, UsesMillimeterPrecision());
            txtLengthDecimal.Text = _EntDtArea.LengthDecimal.ToString();
            SetLengthRounding(_EntDtArea.LengthRoundingOpt);
        }

        private void BtnAreaDefault_Click(object sender, RoutedEventArgs e)
        {
            _EntDtArea.Initvalue(2, UsesMillimeterPrecision());
            txtAreaDecimal.Text = _EntDtArea.AreaDecimal.ToString();
            SetAreaRounding(_EntDtArea.AreaRoundingOpt);
        }

        private void RdbLengthUnit_Checked(object sender, RoutedEventArgs e)
        {
            if (_isCheckRdb)
                SetLengthUnit();
            else
                _isCheckRdb = true;
        }

        private void TxtLengthDecimal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateLengthDecimal();
        }

        private void TxtAreaDecimal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateAreaDecimal();
        }

        #endregion

        #region Private Methods

        private void SetText()
        {
            WeaveWindowChrome.SetTitle(this, this, WeaveCommandTitles.GroundsExpression(_CmpAttribute));

            lblLengthSection.Text = _CmpAttribute.ResourceText("IDS_TXT_LENGTHDECIMAL");
            lblUnit.Text = _CmpAttribute.ResourceText("IDS_TXT_PROJECTUNITSLABEL") + ":";
            UpdateUnitLabels();

            lblLengthDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            lblLengthOrder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
            rdbLengthCut.Content = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            rdbLengthClose.Content = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            rdbLengthRounding.Content = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            btnLengthDefault.Content = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT");

            lblAreaSection.Text = _CmpAttribute.ResourceText("IDS_TXT_AREADECIMAL");
            lblAreaDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            lblAreaOrder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
            rdbAreaCut.Content = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            rdbAreaClose.Content = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            rdbAreaRounding.Content = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            btnAreaDefault.Content = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT");

            lblPiSection.Text = _CmpAttribute.ResourceText("IDS_TXT_PI");
            btnOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL") + " (_C)";
        }

        private void UpdateUnitLabels()
        {
            string areaLabel = _EntDtArea.ProjectAreaUnitLabel;

            rdbLengthProject.Content = GetProjectLengthUnitOptionLabel();
            rdbLengthMeters.Content = _EntDtArea.ProjectIsImperial
                ? _CmpAttribute.ResourceText("IDS_TXT_INCHESUNIT")
                : _CmpAttribute.ResourceText("IDS_TXT_METERSUNIT");
            lblAreaUnitNote.Text = string.Format(
                _CmpAttribute.ResourceText("IDS_TXT_AREAUNITNOTE"), areaLabel);
        }

        private string GetProjectLengthUnitOptionLabel()
        {
            if (_EntDtArea.ProjectIsImperial)
                return _CmpAttribute.ResourceText("IDS_TXT_FEETUNIT");

            if (_EntDtArea.ProjectLengthUnitIsMillimeters)
                return _CmpAttribute.ResourceText("IDS_TXT_MILLIMETERSUNIT");

            if (_EntDtArea.ProjectLengthUnitLabel == _CmpAttribute.ResourceText("IDS_TXT_M"))
                return _CmpAttribute.ResourceText("IDS_TXT_METERSUNIT");

            return string.Format(
                _CmpAttribute.ResourceText("IDS_TXT_PROJECTUNITS"),
                _EntDtArea.ProjectLengthUnitLabel);
        }

        private void SetData()
        {
            _isCheckRdb = false;

            txtLengthDecimal.Text = _EntDtArea.LengthDecimal.ToString();
            _MLengthDecimal = txtLengthDecimal.Text;
            txtAreaDecimal.Text = _EntDtArea.AreaDecimal.ToString();

            SetLengthRounding(_EntDtArea.LengthRoundingOpt);
            SetAreaRounding(_EntDtArea.AreaRoundingOpt);
            SetLengthUnit(_EntDtArea.LengthUnit);
            UpdateUnitLabels();
            ClampLengthDecimalForCurrentUnit();

            cboPi.ItemsSource = _EntDtArea.DataPI.DefaultView;
            cboPi.DisplayMemberPath = _EntDtArea.DataPI.Columns[1].ColumnName;
            cboPi.SelectedValuePath = _EntDtArea.DataPI.Columns[0].ColumnName;

            if (cboPi.Items.Count > 0)
            {
                cboPi.SelectedValue = _EntDtArea.PiOpt;
                if (cboPi.SelectedIndex == -1)
                    cboPi.SelectedIndex = 0;
            }
        }

        private void GetData()
        {
            _EntDtArea.LengthUnit = GetLengthUnit();
            _EntDtArea.LengthDecimal = int.Parse(txtLengthDecimal.Text);
            _EntDtArea.AreaDecimal = int.Parse(txtAreaDecimal.Text);
            _EntDtArea.LengthRoundingOpt = GetLengthRounding();
            _EntDtArea.AreaRoundingOpt = GetAreaRounding();
            _EntDtArea.PiOpt = cboPi.SelectedValue != null ? (int)cboPi.SelectedValue : -1;

            _EntDtCmd.Data[0] = _EntDtArea.LengthDecimal.ToString();
            _EntDtCmd.Data[1] = _EntDtArea.AreaDecimal.ToString();
            _EntDtCmd.Data[2] = _EntDtArea.LengthRoundingOpt.ToString();
            _EntDtCmd.Data[3] = _EntDtArea.AreaRoundingOpt.ToString();
            _EntDtCmd.Data[4] = _EntDtArea.PiOpt.ToString();
            _EntDtCmd.Data[5] = _EntDtArea.LengthUnit.ToString();
        }

        private bool ValidateInput()
        {
            return ValidateLengthDecimal() && ValidateAreaDecimal();
        }

        private bool ValidateLengthDecimal()
        {
            string error = _EntDtArea.SetErrPvdDecimalText(
                txtLengthDecimal.Text.Trim(), true, true, UsesExtendedLengthDecimalRange());
            if (!string.IsNullOrEmpty(error))
            {
                ShowValidationMessage(error);
                return false;
            }
            return true;
        }

        private bool ValidateAreaDecimal()
        {
            string error = _EntDtArea.SetErrPvdDecimalText(txtAreaDecimal.Text.Trim(), false, false, false);
            if (!string.IsNullOrEmpty(error))
            {
                ShowValidationMessage(error);
                return false;
            }
            return true;
        }

        private void SetLengthUnit()
        {
            string errMsg = "";

            if (rdbLengthProject.IsChecked ?? false)
            {
                errMsg = _EntDtArea.SetErrPvdDecimalText(
                    txtLengthDecimal.Text.Trim(), true, false, UsesExtendedLengthDecimalRange());
                if (string.IsNullOrEmpty(errMsg))
                {
                    _MLengthDecimal = txtLengthDecimal.Text.Trim();

                    if (UsesExtendedLengthDecimalRange())
                    {
                        if (txtLengthDecimal.Text.Trim() == "5")
                            txtLengthDecimal.Text = "2";
                        else if (txtLengthDecimal.Text.Trim() == "1" ||
                                txtLengthDecimal.Text.Trim() == "2" ||
                                txtLengthDecimal.Text.Trim() == "3" ||
                                txtLengthDecimal.Text.Trim() == "4")
                        {
                            txtLengthDecimal.Text = "1";
                        }
                        else
                        {
                            txtLengthDecimal.Text = "1";
                        }
                    }
                }
                else
                {
                    rdbLengthProject.IsChecked = false;
                    rdbLengthMeters.IsChecked = true;
                }
            }
            else
            {
                errMsg = _EntDtArea.SetErrPvdDecimalText(_MLengthDecimal.Trim(), true, false, true);
                if (string.IsNullOrEmpty(errMsg))
                {
                    if (txtLengthDecimal.Text.Trim() == "1")
                    {
                        txtLengthDecimal.Text = "4";
                        txtLengthDecimal.IsEnabled = true;
                    }
                    else if (txtLengthDecimal.Text.Trim() == "2")
                    {
                        txtLengthDecimal.Text = "5";
                        txtLengthDecimal.IsEnabled = true;
                    }
                    else
                    {
                        txtLengthDecimal.Text = "3";
                    }
                }
                else
                {
                    rdbLengthProject.IsChecked = true;
                    rdbLengthMeters.IsChecked = false;
                }
            }
        }

        private bool UsesExtendedLengthDecimalRange()
        {
            if (rdbLengthMeters.IsChecked ?? false)
                return true;

            return _EntDtArea.ProjectLengthUnitIsMillimeters;
        }

        private void ClampLengthDecimalForCurrentUnit()
        {
            if (!int.TryParse(txtLengthDecimal.Text.Trim(), out int value))
                return;

            int min = _EntDtArea.DecimalMin;
            int max = UsesExtendedLengthDecimalRange()
                ? _EntDtArea.DecimalMax - 4
                : _EntDtArea.DecimalMax - 7;

            if (value < min)
                value = min;
            else if (value > max)
                value = max;

            txtLengthDecimal.Text = value.ToString();
            _MLengthDecimal = txtLengthDecimal.Text;
        }

        private bool UsesMillimeterPrecision()
        {
            if (rdbLengthMeters.IsChecked ?? false)
                return false;

            return _EntDtArea.ProjectLengthUnitIsMillimeters;
        }

        private int GetLengthUnit()
        {
            return rdbLengthProject.IsChecked ?? false ? 0 : 1;
        }

        private void SetLengthUnit(int value)
        {
            switch (value)
            {
                case 0:
                    rdbLengthProject.IsChecked = true;
                    break;
                case 1:
                    rdbLengthMeters.IsChecked = true;
                    break;
            }
        }

        private int GetLengthRounding()
        {
            if (rdbLengthCut.IsChecked ?? false) return 0;
            if (rdbLengthClose.IsChecked ?? false) return 1;
            if (rdbLengthRounding.IsChecked ?? false) return 2;
            return 0;
        }

        private void SetLengthRounding(int value)
        {
            switch (value)
            {
                case 0:
                    rdbLengthCut.IsChecked = true;
                    break;
                case 1:
                    rdbLengthClose.IsChecked = true;
                    break;
                case 2:
                    rdbLengthRounding.IsChecked = true;
                    break;
            }
        }

        private int GetAreaRounding()
        {
            if (rdbAreaCut.IsChecked ?? false) return 0;
            if (rdbAreaClose.IsChecked ?? false) return 1;
            if (rdbAreaRounding.IsChecked ?? false) return 2;
            return 0;
        }

        private void SetAreaRounding(int value)
        {
            switch (value)
            {
                case 0:
                    rdbAreaCut.IsChecked = true;
                    break;
                case 1:
                    rdbAreaClose.IsChecked = true;
                    break;
                case 2:
                    rdbAreaRounding.IsChecked = true;
                    break;
            }
        }

        private void ShowValidationMessage(string message)
        {
            IntPtr ownerHandle = new WindowInteropHelper(this).Handle;
            WeaveDialogHost.ShowMessage(
                ownerHandle,
                message,
                WeaveCommandTitles.GroundsExpression(_CmpAttribute),
                _CmpAttribute.ResourceText("IDS_TXT_OK"));
        }

        #endregion
    }
}
