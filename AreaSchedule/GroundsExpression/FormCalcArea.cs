
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using System.Reflection;

namespace ADSK.JExtRAC.AreaSchedule.GroundsExpression
{
    /// ================================================================================
    /// <summary>画面 面積計算</summary>
    /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class FormCalcArea : Form
    {
        // メンバ変数

        #region Memeber Variables

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

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="entDtArea"   >データテーブル - エリア</param>
        /// <param name="entDtCmd"    >データテーブル - コマンド</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public FormCalcArea(RvtExtApp.Components.Attribute cmpAttribute,
                            RvtExtApp.Entities.DtArea entDtArea,
                            RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _EntDtArea = entDtArea;
            _EntDtCmd = entDtCmd;
            _MLengthDecimal = "1";
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history><p>2011/08/02 Created GSA,Inc. Shinichi Ishii</p>
        ///           <p>2015/09/07 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_CALCAREA") + string.Format("[Ver.{0}]", Assembly.GetExecutingAssembly().GetName().Version);

            this.gpbLengthDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_LENGTHDECIMAL");
            this.lblLengthUnit.Text = _CmpAttribute.ResourceText("IDS_TXT_UNIT");
            this.gpbLengthUnit.Text = "";
            this.rdbLengthMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.rdbLengthM.Text = _CmpAttribute.ResourceText("IDS_TXT_M");

            this.lblLengthDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            this.lblLengthOrder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
            this.gpbLengthRounding.Text = "";
            this.rdbLengthCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            this.rdbLengthClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            this.rdbLengthRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            this.btnLengthDefault.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&B)";

            this.gpbAreaDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_AREADECIMAL") +
                                          _CmpAttribute.ResourceText("IDS_TXT_M22");
            this.lblAreaDecimal.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            this.lblAreaOrder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
            this.gpbAreaRounding.Text = "";
            this.rdbAreaCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            this.rdbAreaClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOSE");
            this.rdbAreaRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            this.btnAreaDefault.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&F)";

            this.gpbPi.Text = _CmpAttribute.ResourceText("IDS_TXT_PI");

            this.btnOK.Text = "&" + _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL") + "(&C)";

            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void SetData()
        {
            _isCheckRdb = false;

            this.txtLengthDecimal.Text = _EntDtArea.LengthDecimal.ToString();
            _MLengthDecimal = this.txtLengthDecimal.Text;
            this.txtAreaDecimal.Text = _EntDtArea.AreaDecimal.ToString();
            RdbLengthRounding = _EntDtArea.LengthRoundingOpt;
            RdbAreaRounding = _EntDtArea.AreaRoundingOpt;
            RdbLengthUnit = _EntDtArea.LengthUnit;
            this.cboPi.DataSource = _EntDtArea.DataPI;
            this.cboPi.DisplayMember = _EntDtArea.DataPI.Columns[1].ColumnName;
            this.cboPi.ValueMember = _EntDtArea.DataPI.Columns[0].ColumnName;

            if (this.cboPi.Items.Count > 0)
            {
                this.cboPi.SelectedValue = _EntDtArea.PiOpt;
                if (this.cboPi.SelectedIndex == -1)
                    this.cboPi.SelectedIndex = 0;
            }
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void GetData()
        {
            _EntDtArea.LengthUnit = RdbLengthUnit;
            _EntDtArea.LengthDecimal = int.Parse(this.txtLengthDecimal.Text);
            _EntDtArea.AreaDecimal = int.Parse(this.txtAreaDecimal.Text);
            _EntDtArea.LengthRoundingOpt = RdbLengthRounding;
            _EntDtArea.AreaRoundingOpt = RdbAreaRounding;
            _EntDtArea.PiOpt = -1;
            if (this.cboPi.SelectedValue != null)
            {
                _EntDtArea.PiOpt = (int)this.cboPi.SelectedValue;
            }

            _EntDtCmd.Data[0] = _EntDtArea.LengthDecimal.ToString();
            _EntDtCmd.Data[1] = _EntDtArea.AreaDecimal.ToString();
            _EntDtCmd.Data[2] = _EntDtArea.LengthRoundingOpt.ToString();
            _EntDtCmd.Data[3] = _EntDtArea.AreaRoundingOpt.ToString();
            _EntDtCmd.Data[4] = _EntDtArea.PiOpt.ToString();
            _EntDtCmd.Data[5] = _EntDtArea.LengthUnit.ToString();
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダ取得</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = エラーなし</p>
        ///             <p>False = エラーあり</p></returns>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool GetErrPvd()
        {
            bool ret = false;

            if (this.errPvd.GetError(this.txtLengthDecimal) != "")
            {
                return ret;
            }
            if (this.errPvd.GetError(this.txtAreaDecimal) != "")
            {
                return ret;
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダチェック</summary>
        ///

        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modifed Applied Techbology</p><history>
        /// ================================================================================
        private void ChkErrPvd()
        {
            _checkRdn = GetCheckRdn();
            this.errPvd.SetError(this.txtLengthDecimal, _EntDtArea.SetErrPvdDecimalText(this.txtLengthDecimal.Text.Trim(), true, true, _checkRdn));
            this.errPvd.SetError(this.txtAreaDecimal, _EntDtArea.SetErrPvdDecimalText(this.txtAreaDecimal.Text.Trim(), false, false, false));
        }

        /// ================================================================================
        /// <summary>長さ単位のデータ設定</summary>
        ///
        /// <history><p>2011/08/02 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modifed Applied Techbology</p><history>
        /// ================================================================================
        private void SetLengthUnit()
        {
            string errMsg = "";

            if (this.rdbLengthMM.Checked == true)
            {
                errMsg = _EntDtArea.SetErrPvdDecimalText(this.txtLengthDecimal.Text.Trim(), true, false, false);
                if (errMsg == "")
                {
                    _MLengthDecimal = this.txtLengthDecimal.Text.Trim();

                    if (this.txtLengthDecimal.Text.Trim() == "5")
                        this.txtLengthDecimal.Text = "2";
                    else if (this.txtLengthDecimal.Text.Trim() == "1"
                        || this.txtLengthDecimal.Text.Trim() == "2"
                        || this.txtLengthDecimal.Text.Trim() == "3"
                        || this.txtLengthDecimal.Text.Trim() == "4")
                    {
                        this.txtLengthDecimal.Text = "1";
                    }
                    else
                    {
                        this.errPvd.SetError(this.txtLengthDecimal, string.Empty);
                        this.txtLengthDecimal.Text = "1";
                    }
                }
                else
                {
                    this.rdbLengthMM.Checked = false;
                    this.rdbLengthM.Checked = true;
                }
            }
            else
            {
                errMsg = _EntDtArea.SetErrPvdDecimalText(_MLengthDecimal.Trim(), true, false, true);
                if (errMsg == "")
                {
                    if (this.txtLengthDecimal.Text.Trim() == "1")
                    {
                        this.txtLengthDecimal.Text = "4";
                        this.txtLengthDecimal.Enabled = true;
                    }
                    else if (this.txtLengthDecimal.Text.Trim() == "2")
                    {
                        this.txtLengthDecimal.Text = "5";
                        this.txtLengthDecimal.Enabled = true;
                    }
                    else
                    {
                        this.errPvd.SetError(this.txtLengthDecimal, string.Empty);
                        this.txtLengthDecimal.Text = "3";
                    }
                }
                else
                {
                    this.rdbLengthMM.Checked = true;
                    this.rdbLengthM.Checked = false;
                }
            }
        }

        /// ================================================================================
        /// <summary>get lenght unit</summary>
        ///
        /// <returns>bool</returns>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private bool GetCheckRdn()
        {
            int ret = RdbLengthUnit;
            if (ret == 1)
                return true;
            return false;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>長さの端数タイプ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbLengthRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbLengthCut.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbLengthClose.Checked == true)
                {
                    ret = 1;
                }
                else if (this.rdbLengthRounding.Checked == true)
                {
                    ret = 2;
                }
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbLengthCut.Checked = true;
                        break;

                    case 1:
                        this.rdbLengthClose.Checked = true;
                        break;

                    case 2:
                        this.rdbLengthRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>面積の端数タイプ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbAreaRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbAreaCut.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbAreaClose.Checked == true)
                {
                    ret = 1;
                }
                else if (this.rdbAreaRounding.Checked == true)
                {
                    ret = 2;
                }
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbAreaCut.Checked = true;
                        break;

                    case 1:
                        this.rdbAreaClose.Checked = true;
                        break;

                    case 2:
                        this.rdbAreaRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>長さの単位</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbLengthUnit
        {
            get
            {
                int ret = 0;

                if (this.rdbLengthMM.Checked == true)
                {
                    ret = 0;
                }
                else if (this.rdbLengthM.Checked == true)
                {
                    ret = 1;
                }
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:
                        this.rdbLengthMM.Checked = true;
                        break;

                    case 1:
                        this.rdbLengthM.Checked = true;
                        break;
                }
            }
        }

        #endregion Properties

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>Handles the Load event of the FormCalcArea control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void FormCalcArea_Load(object sender, EventArgs e)
        {
            SetText();
            SetData();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            ChkErrPvd();
            if (GetErrPvd() == true)
            {
                GetData();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnLengthDefault control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnLengthDefault_Click(object sender, EventArgs e)
        {
            _EntDtArea.Initvalue(1, this.rdbLengthMM.Checked);
            this.txtLengthDecimal.Text = _EntDtArea.LengthDecimal.ToString();
            this.errPvd.SetError(this.txtLengthDecimal, string.Empty);
            RdbLengthRounding = _EntDtArea.LengthRoundingOpt;
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnAreaDefault control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnAreaDefault_Click(object sender, EventArgs e)
        {
            _EntDtArea.Initvalue(2, this.rdbLengthMM.Checked);
            this.txtAreaDecimal.Text = _EntDtArea.AreaDecimal.ToString();
            this.errPvd.SetError(this.txtAreaDecimal, string.Empty);
            RdbAreaRounding = _EntDtArea.AreaRoundingOpt;
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the dbDisplayMM control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/07/15 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void rdbDisplayMM_Click(object sender, EventArgs e)
        {
            if (_isCheckRdb)
                SetLengthUnit();
            else
                _isCheckRdb = true;
        }

        // ================================================================================
        /// <summary>Handles the Click event of the rdbDisplayM control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2010/07/15 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void rdbDisplayM_Click(object sender, EventArgs e)
        {
            if (_isCheckRdb)
                SetLengthUnit();
            else
                _isCheckRdb = true;
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtLengthDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history><p>2009/05/26 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Modifed Applied Techbology</p><history>
        /// ================================================================================
        private void txtLengthDecimal_Validated(object sender, EventArgs e)
        {
            _checkRdn = GetCheckRdn();
            this.errPvd.SetError(this.txtLengthDecimal, _EntDtArea.SetErrPvdDecimalText(this.txtLengthDecimal.Text.Trim(), true, true, _checkRdn));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtAreaDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2009/05/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void txtAreaDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtAreaDecimal, _EntDtArea.SetErrPvdDecimalText(this.txtAreaDecimal.Text.Trim(), false, false, false));
        }

        #endregion Events
    }
}