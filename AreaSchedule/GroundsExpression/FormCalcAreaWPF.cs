
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Reflection;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;

namespace ADSK.JExtRAC.AreaSchedule.GroundsExpression
{
    /// ================================================================================
    /// <summary>画面 面積計算 (WPF版)</summary>
    /// <history>2025/05/07 Created</history>
    /// ================================================================================
    public partial class FormCalcAreaWPF : Window
    {
        #region Member Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>メートル用長さの小数点位置</summary>
        private string _MLengthDecimal;

        /// <summary>データテーブル - エリア</summary>
        private RvtExtApp.Entities.DtArea _EntDtArea;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        /// <summary>Check radio button mm or m</summary>
        private bool _checkRdn = false;

        /// <summary>IsCheck radio button mm or m</summary>
        private bool _isCheckRdb;

        #endregion

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtArea">データテーブル - エリア</param>
        /// <param name="entDtCmd">データテーブル - コマンド</param>
        ///
        /// <history>2024/03/21 Created</history>
        /// ================================================================================
        public FormCalcAreaWPF(RvtExtApp.Components.Attribute cmpAttribute,
                              RvtExtApp.Entities.DtArea entDtArea,
                              RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _EntDtArea = entDtArea;
            _EntDtCmd = entDtCmd;
            _MLengthDecimal = "1";

            // コマンドの設定
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
            rdbLengthMM.Checked += RdbLengthMM_Checked;
            rdbLengthM.Checked += RdbLengthM_Checked;
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnLengthDefault_Click(object sender, RoutedEventArgs e)
        {
            _EntDtArea.Initvalue(1, rdbLengthMM.IsChecked ?? false);
            txtLengthDecimal.Text = _EntDtArea.LengthDecimal.ToString();
            SetLengthRounding(_EntDtArea.LengthRoundingOpt);
        }

        private void BtnAreaDefault_Click(object sender, RoutedEventArgs e)
        {
            _EntDtArea.Initvalue(2, rdbLengthMM.IsChecked ?? false);
            txtAreaDecimal.Text = _EntDtArea.AreaDecimal.ToString();
            SetAreaRounding(_EntDtArea.AreaRoundingOpt);
        }

        private void RdbLengthMM_Checked(object sender, RoutedEventArgs e)
        {
            if (_isCheckRdb)
                SetLengthUnit();
            else
                _isCheckRdb = true;
        }

        private void RdbLengthM_Checked(object sender, RoutedEventArgs e)
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
            Title = _CmpAttribute.ResourceText("IDS_TXT_CALCAREA") + 
                   string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            var gpbLength = (System.Windows.Controls.GroupBox)FindName("gpbLength");
            if (gpbLength == null)
            {
                var grid = (Grid)Content;
                gpbLength = (System.Windows.Controls.GroupBox)grid.Children[0];
            }
            gpbLength.Header = _CmpAttribute.ResourceText("IDS_TXT_LENGTHDECIMAL");

            FindLabel("lblUnit").Content = _CmpAttribute.ResourceText("IDS_TXT_UNIT") + ":";
            FindLabel("lblLengthDecimal").Content = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            FindLabel("lblLengthOrder").Content = _CmpAttribute.ResourceText("IDS_TXT_ORDER");

            rdbLengthCut.Content = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            rdbLengthClose.Content = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            rdbLengthRounding.Content = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            btnLengthDefault.Content = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(_B)";

            var grid2 = (Grid)Content;
            var gpbArea = (System.Windows.Controls.GroupBox)grid2.Children[1];
            gpbArea.Header = _CmpAttribute.ResourceText("IDS_TXT_AREADECIMAL");

            FindLabel("lblAreaDecimal").Content = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            FindLabel("lblAreaOrder").Content = _CmpAttribute.ResourceText("IDS_TXT_ORDER");

            rdbAreaCut.Content = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            rdbAreaClose.Content = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            rdbAreaRounding.Content = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            btnAreaDefault.Content = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(_F)";

            var gpbPi = (System.Windows.Controls.GroupBox)grid2.Children[2];
            gpbPi.Header = _CmpAttribute.ResourceText("IDS_TXT_PI");

            btnOK.Content = _CmpAttribute.ResourceText("IDS_TXT_OK");
            btnCancel.Content = _CmpAttribute.ResourceText("IDS_TXT_CANCEL") + "(_C)";
        }

        private System.Windows.Controls.Label FindLabel(string name)
        {
            var lbl = (System.Windows.Controls.Label)FindName(name);
            if (lbl != null) return lbl;
            return new System.Windows.Controls.Label();
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
            _checkRdn = GetCheckRdn();
            string error = _EntDtArea.SetErrPvdDecimalText(txtLengthDecimal.Text.Trim(), true, true, _checkRdn);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool ValidateAreaDecimal()
        {
            string error = _EntDtArea.SetErrPvdDecimalText(txtAreaDecimal.Text.Trim(), false, false, false);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void SetLengthUnit()
        {
            string errMsg = "";

            if (rdbLengthMM.IsChecked ?? false)
            {
                errMsg = _EntDtArea.SetErrPvdDecimalText(txtLengthDecimal.Text.Trim(), true, false, false);
                if (string.IsNullOrEmpty(errMsg))
                {
                    _MLengthDecimal = txtLengthDecimal.Text.Trim();

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
                else
                {
                    rdbLengthMM.IsChecked = false;
                    rdbLengthM.IsChecked = true;
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
                    rdbLengthMM.IsChecked = true;
                    rdbLengthM.IsChecked = false;
                }
            }
        }

        private bool GetCheckRdn()
        {
            return GetLengthUnit() == 1;
        }

        private int GetLengthUnit()
        {
            return rdbLengthMM.IsChecked ?? false ? 0 : 1;
        }

        private void SetLengthUnit(int value)
        {
            switch (value)
            {
                case 0:
                    rdbLengthMM.IsChecked = true;
                    break;
                case 1:
                    rdbLengthM.IsChecked = true;
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

        #endregion
    }
} 