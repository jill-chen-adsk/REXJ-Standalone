
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;
using System.Reflection;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    /// ================================================================================
    /// <summary>FormSetting</summary>
    /// <history>2021/11/24 Created Applied Technology</history>
    /// ================================================================================
    public partial class FormSetting : Form
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - コマンド</summary>
        private RvtExtApp.Entities.DtCmd _EntDtCmd;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="commanndKind">コマンド種類</param>
        /// <param name="cmpAttribute"   >属性</param>
        /// <param name="entDtCmd"    >データテーブル - コマンド</param>
        ///
        /// <history><p>2021/11/24 Created Applied Technology</p><history>
        /// ================================================================================
        public FormSetting(int commanndKind,
                            RvtExtApp.Components.Attribute cmpAttribute,
                            RvtExtApp.Entities.DtCmd entDtCmd)
        {
            InitializeComponent();
            RevitFormTheme.Apply(this);
            _CmpAttribute = cmpAttribute;
            _EntDtCmd = entDtCmd;
            _EntDtCmd.CommandKind = commanndKind;
            SetText();
            SetData();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>Set Text</summary>
        ///
        /// <history><p>2021/11/24 Created Applied Technology</p><history>
        /// ================================================================================
        private void SetText()
        {
            this.gpbLegalArea.Text = _CmpAttribute.ResourceText("IDS_TXT_LEGALAREA");
            this.lblLegalArea.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
            this.btnDefaultA.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&A)";
            this.lblLegalAreaOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
            this.rdbLegalAreaCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
            this.rdbLegalAreaClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
            this.rdbLegalAreaRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
            int width = 402;
            int height = 457;
            switch (_EntDtCmd.CommandKind)
            {
                case 0:// lighting
                    this.Text = CheckingCommandTitles.GetCommandTitle(_CmpAttribute, 0);
                    //hide groupbox
                    gpbAreaToBeSmoked.Visible = false;
                    gpbAreaToBeVentilated.Visible = false;
                    gpbEffectiveSmokeExtractionArea.Visible = false;
                    gpbEffectiveVentilationArea.Visible = false;
                    this.MinimumSize = new System.Drawing.Size(400, 690);
                    this.gpbAreaToGetLight.Text = _CmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESAREA");
                    this.lblAreaToGetLight.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultN.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&N)";
                    this.lblRequiredAreaForDaylightingOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbAreaToGetLightCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
                    this.rdbAreaToGetLightClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbAreaToGetLightRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");

                    this.gpbdh.Text = _CmpAttribute.ResourceText("IDS_TXT_COMMON");
                    this.lbldh.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultD.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&D)";
                    this.lbldhOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbdhCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbdhClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS");
                    this.rdbdhRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");

                    this.gpbEffectiveOpeningArea.Text = _CmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEAREA");
                    this.lblEffectiveOpeningArea.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultO.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&O)";
                    this.lblEffectiveOpeningAreaOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbEffectiveOpeningAreaCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbEffectiveOpeningAreaClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS");
                    this.rdbEffectiveOpeningAreaRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");

                    this.gpbEffectiveLightingArea.Text = _CmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA");
                    this.lblEffectiveLightingArea.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultL.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&L)";
                    this.lblEffectiveLightingAreaOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbEffectiveLightingAreaCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbEffectiveLightingAreaClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS");
                    this.rdbEffectiveLightingAreaRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
                    break;

                case 1:// Ventilation
                    this.Text = CheckingCommandTitles.GetCommandTitle(_CmpAttribute, 1);
                    //Hide groupbox
                    gpbAreaToBeVentilated.Visible = false;
                    gpbEffectiveVentilationArea.Visible = false;
                    gpbAreaToGetLight.Visible = false;
                    gpbdh.Visible = false;
                    gpbEffectiveOpeningArea.Visible = false;
                    gpbEffectiveLightingArea.Visible = false;

                    this.ClientSize = new System.Drawing.Size(width, height);
                    this.MinimumSize = new System.Drawing.Size(400, 530);
                    //
                    this.gpbAreaToBeSmoked.Text = _CmpAttribute.ResourceText("IDS_TXT_SMOKENESAREA");
                    this.lblAreaToBeSmoked.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultN1.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&N)";
                    this.lblAreaToBeSmokedOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbAreaToBeSmokedCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
                    this.rdbAreaToBeSmokedClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbAreaToBeSmokedRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");

                    this.gpbEffectiveSmokeExtractionArea.Text = _CmpAttribute.ResourceText("IDS_TXT_WINDOWSMOKEUSABLEAREA");
                    this.lblEffectiveSmokeExtractionArea.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultS.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&S)";
                    this.lblEffectiveSmokeExtractionAreaOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbEffectiveSmokeExtractionAreaCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbEffectiveSmokeExtractionAreaClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS");
                    this.rdbEffectiveSmokeExtractionAreaRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
                    break;

                case 2:// Smoke Exhoustion
                    this.Text = CheckingCommandTitles.GetCommandTitle(_CmpAttribute, 2);
                    //Hide groupbox
                    gpbAreaToBeSmoked.Visible = false;
                    gpbEffectiveSmokeExtractionArea.Visible = false;
                    gpbAreaToGetLight.Visible = false;
                    gpbdh.Visible = false;
                    gpbEffectiveOpeningArea.Visible = false;
                    gpbEffectiveLightingArea.Visible = false;

                    this.ClientSize = new System.Drawing.Size(width, height);
                    this.MinimumSize = new System.Drawing.Size(400, 530);
                    //
                    this.gpbAreaToBeVentilated.Text = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESAREA");
                    this.lblAreaToBeVentilated.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultN2.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&N)";
                    this.lblAreaToBeVentilatedOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbAreaToBeVentilatedCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF");
                    this.rdbAreaToBeVentilatedClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbAreaToBeVentilatedRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");

                    this.gpbEffectiveVentilationArea.Text = _CmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA");
                    this.lblEffectiveVentilationArea.Text = _CmpAttribute.ResourceText("IDS_TXT_DECIMAL");
                    this.btnDefaultV.Text = _CmpAttribute.ResourceText("IDS_TXT_DEFAULT") + "(&V)";
                    this.lblEffectiveVentilationAreaOder.Text = _CmpAttribute.ResourceText("IDS_TXT_ORDER");
                    this.rdbEffectiveVentilationAreaCut.Text = _CmpAttribute.ResourceText("IDS_TXT_CUTOFF") + _CmpAttribute.ResourceText("IDS_TXT_SAFE_SIDE");
                    this.rdbEffectiveVentilationAreaClose.Text = _CmpAttribute.ResourceText("IDS_TXT_CLOS");
                    this.rdbEffectiveVentilationAreaRounding.Text = _CmpAttribute.ResourceText("IDS_TXT_ROUNDINGOFF");
                    break;
            }
            //Set text button
            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
            // Set icon
            this.Icon = _CmpAttribute.ResourceImage("IDI_SUBS_ICON") as System.Drawing.Icon;
        }

        /// ================================================================================
        /// <summary>Set Data </summary>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void SetData()
        {
            this.txtLegalAreaDecimal.Text = _EntDtCmd.LegalAreaRoundingDecimal.ToString();
            RdbLegalAreaRounding = _EntDtCmd.LegalAreaRoundingOpt;
            switch (_EntDtCmd.CommandKind)
            {
                case 0:// lighting
                    this.txtAreaToGetLight.Text = _EntDtCmd.AreaToGetLightRoundingDecimal.ToString();
                    RdbAreaToGetLightRounding = _EntDtCmd.AreaToGetLightRoundingOpt;

                    this.txtdhDecimal.Text = _EntDtCmd.DHRoundingDecimal.ToString();
                    RdbDHRounding = _EntDtCmd.DHRoundingOpt;

                    this.txtEffectiveOpeningAreaDecimal.Text = _EntDtCmd.EffectiveOpeningAreaRoundingDecimal.ToString();
                    RdbEffectiveOpeningAreaRounding = _EntDtCmd.EffectiveOpeningAreaRoundingOpt;

                    this.txtEffectiveLightingAreaDecimal.Text = _EntDtCmd.EffectiveLightingAreaRoundingDecimal.ToString();
                    RdbEffectiveLightingAreaRounding = _EntDtCmd.EffectiveLightingAreaRoundingOpt;
                    break;

                case 1:// Ventilation
                    this.txtAreaToBeSmokedDecimal.Text = _EntDtCmd.AreaToBeSmokedRoundingDecimal.ToString();
                    RdbAreaToBeSmokedRounding = _EntDtCmd.AreaToBeSmokedRoundingOtp;

                    this.txtEffectiveSmokeExtractionAreaDecimal.Text = _EntDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal.ToString();
                    RdbEffectiveSmokeExtractionAreaRounding = _EntDtCmd.EffectiveSmokeExtractionAreaRoundingOtp;
                    break;

                case 2:// Smoke Exhoustion
                    this.txtAreaToBeVentilatedDecimal.Text = _EntDtCmd.AreaToBeVentilatedRoundingDecimal.ToString();
                    RdbAreaToBeVentilatedRounding = _EntDtCmd.AreaToBeVentilatedRoundingOtp;

                    this.txtEffectiveVentilationAreaDecimal.Text = _EntDtCmd.EffectiveVentilationAreaRoundingDecimal.ToString();
                    RdbEffectiveVentilationAreaRounding = _EntDtCmd.EffectiveVentilationAreaRoundingOtp;
                    break;
            }
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history><p>2021/11/24 Created Applied Technology</p><history>
        /// ================================================================================
        private void GetData()
        {
            _EntDtCmd.LegalAreaRoundingDecimal = int.Parse(this.txtLegalAreaDecimal.Text);
            _EntDtCmd.LegalAreaRoundingOpt = RdbLegalAreaRounding;
            switch (_EntDtCmd.CommandKind)
            {
                case 0:// lighting
                    _EntDtCmd.AreaToGetLightRoundingDecimal = int.Parse(this.txtAreaToGetLight.Text);
                    _EntDtCmd.AreaToGetLightRoundingOpt = RdbAreaToGetLightRounding;

                    _EntDtCmd.DHRoundingDecimal = int.Parse(this.txtdhDecimal.Text);
                    _EntDtCmd.DHRoundingOpt = RdbDHRounding;

                    _EntDtCmd.EffectiveOpeningAreaRoundingDecimal = int.Parse(this.txtEffectiveOpeningAreaDecimal.Text);
                    _EntDtCmd.EffectiveOpeningAreaRoundingOpt = RdbEffectiveOpeningAreaRounding;

                    _EntDtCmd.EffectiveLightingAreaRoundingDecimal = int.Parse(this.txtEffectiveLightingAreaDecimal.Text);
                    _EntDtCmd.EffectiveLightingAreaRoundingOpt = RdbEffectiveLightingAreaRounding;

                    break;

                case 1:// Ventilation
                    _EntDtCmd.AreaToBeSmokedRoundingDecimal = int.Parse(this.txtAreaToBeSmokedDecimal.Text);
                    _EntDtCmd.AreaToBeSmokedRoundingOtp = RdbAreaToBeSmokedRounding;

                    _EntDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal = int.Parse(this.txtEffectiveSmokeExtractionAreaDecimal.Text);
                    _EntDtCmd.EffectiveSmokeExtractionAreaRoundingOtp = RdbEffectiveSmokeExtractionAreaRounding;

                    break;

                case 2:// Smoke Exhoustion
                    _EntDtCmd.AreaToBeVentilatedRoundingDecimal = int.Parse(this.txtAreaToBeVentilatedDecimal.Text);
                    _EntDtCmd.AreaToBeVentilatedRoundingOtp = RdbAreaToBeVentilatedRounding;

                    _EntDtCmd.EffectiveVentilationAreaRoundingDecimal = int.Parse(this.txtEffectiveVentilationAreaDecimal.Text);
                    _EntDtCmd.EffectiveVentilationAreaRoundingOtp = RdbEffectiveVentilationAreaRounding;

                    break;
            }
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダ取得</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = エラーなし</p>
        ///             <p>False = エラーあり</p></returns>
        ///
        /// <history><p>2021/11/24 Created Applied Technology</p><history>
        /// ================================================================================
        private bool GetErrPvd()
        {
            bool ret = false;
            if (this.errPvd.GetError(this.txtLegalAreaDecimal) != string.Empty)
            {
                return ret;
            }
            switch (_EntDtCmd.CommandKind)
            {
                case 0:// lighting
                    if (this.errPvd.GetError(this.txtAreaToGetLight) != string.Empty)
                        return ret;
                    if (this.errPvd.GetError(this.txtdhDecimal) != string.Empty)
                        return ret;

                    if (this.errPvd.GetError(this.txtEffectiveOpeningAreaDecimal) != string.Empty)
                        return ret;
                    if (this.errPvd.GetError(this.txtEffectiveLightingAreaDecimal) != string.Empty)
                        return ret;

                    break;

                case 1:// Ventilation
                    if (this.errPvd.GetError(this.txtAreaToBeSmokedDecimal) != string.Empty)
                        return ret;

                    if (this.errPvd.GetError(this.txtEffectiveSmokeExtractionAreaDecimal) != string.Empty)
                        return ret;

                    break;

                case 2:// Smoke Exhoustion
                    if (this.errPvd.GetError(this.txtAreaToBeVentilatedDecimal) != string.Empty)
                        return ret;

                    if (this.errPvd.GetError(this.txtEffectiveVentilationAreaDecimal) != string.Empty)
                        return ret;

                    break;
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>フォームのエラープロバイダチェック</summary>
        ///
        /// <history><p>2021/11/24 Created Applied Technology</p><history>
        /// ================================================================================
        private void ChkErrPvd()
        {
            this.errPvd.SetError(this.txtLegalAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtLegalAreaDecimal.Text.Trim(), true));
            switch (_EntDtCmd.CommandKind)
            {
                case 0: // lighting
                    this.errPvd.SetError(this.txtAreaToGetLight, _EntDtCmd.SetErrPvdNumeric(this.txtAreaToGetLight.Text.Trim(), true));
                    this.errPvd.SetError(this.txtdhDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtdhDecimal.Text.Trim(), true));
                    this.errPvd.SetError(this.txtEffectiveOpeningAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveOpeningAreaDecimal.Text.Trim(), true));
                    this.errPvd.SetError(this.txtEffectiveLightingAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveLightingAreaDecimal.Text.Trim(), true));
                    break;

                case 1: // Ventilation
                    this.errPvd.SetError(this.txtAreaToBeSmokedDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtAreaToBeSmokedDecimal.Text.Trim(), true));
                    this.errPvd.SetError(this.txtEffectiveSmokeExtractionAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveSmokeExtractionAreaDecimal.Text.Trim(), true));
                    break;

                case 2: // Smoke Exhoustion
                    this.errPvd.SetError(this.txtAreaToBeVentilatedDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtAreaToBeVentilatedDecimal.Text.Trim(), true));
                    this.errPvd.SetError(this.txtEffectiveVentilationAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveVentilationAreaDecimal.Text.Trim(), true));
                    break;
            }
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary> Legal area fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbLegalAreaRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbLegalAreaCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbLegalAreaClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbLegalAreaRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:  //Truncate
                        this.rdbLegalAreaCut.Checked = true;
                        break;

                    case 1: //Round up
                        this.rdbLegalAreaClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbLegalAreaRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> Area To Get Light fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbAreaToGetLightRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbAreaToGetLightCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbAreaToGetLightClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbAreaToGetLightRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:  //Truncate
                        this.rdbAreaToGetLightCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbAreaToGetLightClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbAreaToGetLightRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> D / H fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbDHRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbdhCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbdhClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbdhRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:  //Truncate
                        this.rdbdhCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbdhClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbdhRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> Effective Opening Area fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbEffectiveOpeningAreaRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbEffectiveOpeningAreaCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbEffectiveOpeningAreaClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbEffectiveOpeningAreaRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:  //Truncate
                        this.rdbEffectiveOpeningAreaCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbEffectiveOpeningAreaClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbEffectiveOpeningAreaRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> Effective Lighting Area fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbEffectiveLightingAreaRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbEffectiveLightingAreaCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbEffectiveLightingAreaClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbEffectiveLightingAreaRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:  //Truncate
                        this.rdbEffectiveLightingAreaCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbEffectiveLightingAreaClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbEffectiveLightingAreaRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> Area ToBe moked fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbAreaToBeSmokedRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbAreaToBeSmokedCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbAreaToBeSmokedClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbAreaToBeSmokedRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:  //Truncate
                        this.rdbAreaToBeSmokedCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbAreaToBeSmokedClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbAreaToBeSmokedRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> Effective Smoke Extraction Area fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbEffectiveSmokeExtractionAreaRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbEffectiveSmokeExtractionAreaCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbEffectiveSmokeExtractionAreaClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbEffectiveSmokeExtractionAreaRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:     //Truncate
                        this.rdbEffectiveSmokeExtractionAreaCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbEffectiveSmokeExtractionAreaClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbEffectiveSmokeExtractionAreaRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> Area To Be Ventilated fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbAreaToBeVentilatedRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbAreaToBeVentilatedCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbAreaToBeVentilatedClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbAreaToBeVentilatedRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:     //Truncate
                        this.rdbAreaToBeVentilatedCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbAreaToBeVentilatedClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbAreaToBeVentilatedRounding.Checked = true;
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary> Effective Ventilation Area fraction type</summary>
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private int RdbEffectiveVentilationAreaRounding
        {
            get
            {
                int ret = 0;

                if (this.rdbEffectiveVentilationAreaCut.Checked)
                    ret = 0;//Truncate
                else if (this.rdbEffectiveVentilationAreaClose.Checked)
                    ret = 1;//Round up
                else if (this.rdbEffectiveVentilationAreaRounding.Checked)
                    ret = 2;//conditional rounding
                return ret;
            }
            set
            {
                switch (value)
                {
                    case 0:     //Truncate
                        this.rdbEffectiveVentilationAreaCut.Checked = true;
                        break;

                    case 1://Round up
                        this.rdbEffectiveVentilationAreaClose.Checked = true;
                        break;

                    case 2://conditional rounding
                        this.rdbEffectiveVentilationAreaRounding.Checked = true;
                        break;
                }
            }
        }

        #endregion Properties

        // イベント

        #region Event

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtLegalAreaDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtLegalAreaDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtLegalAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtLegalAreaDecimal.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtRequiredAreaForDaylightingDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtRequiredAreaForDaylightingDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtAreaToGetLight, _EntDtCmd.SetErrPvdNumeric(this.txtAreaToGetLight.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtdhDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtdhDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtdhDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtdhDecimal.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtEffectiveOpeningAreaDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtEffectiveOpeningAreaDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtEffectiveOpeningAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveOpeningAreaDecimal.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtEffectiveLightingAreaDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtEffectiveLightingAreaDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtEffectiveLightingAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveLightingAreaDecimal.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnOK control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
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
        /// <summary>Handles the Click event of the btnDefault control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void btnDefault_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = sender as System.Windows.Forms.Button;
            if (btn == null)
                return;
            //legal area
            if (btn.Handle == btnDefaultA.Handle)
            {
                _EntDtCmd.Initvalue(1);
                this.txtLegalAreaDecimal.Text = _EntDtCmd.LegalAreaRoundingDecimal.ToString();
                RdbLegalAreaRounding = _EntDtCmd.LegalAreaRoundingOpt;
                this.errPvd.SetError(this.txtLegalAreaDecimal, string.Empty);
            }
            else if (btn.Handle == btnDefaultN.Handle ||
                btn.Handle == btnDefaultN1.Handle ||
                btn.Handle == btnDefaultN2.Handle)
            {
                switch (_EntDtCmd.CommandKind)
                {    //Area to get light
                    case 0:
                        _EntDtCmd.Initvalue(2);
                        this.txtAreaToGetLight.Text = _EntDtCmd.AreaToGetLightRoundingDecimal.ToString();
                        RdbAreaToGetLightRounding = _EntDtCmd.AreaToGetLightRoundingOpt;
                        this.errPvd.SetError(this.txtAreaToGetLight, string.Empty);
                        break;
                    //Area to be smoked
                    case 1:
                        _EntDtCmd.Initvalue(6);
                        this.txtAreaToBeSmokedDecimal.Text = _EntDtCmd.AreaToBeSmokedRoundingDecimal.ToString();
                        RdbAreaToBeSmokedRounding = _EntDtCmd.AreaToBeSmokedRoundingOtp;
                        this.errPvd.SetError(this.txtAreaToBeSmokedDecimal, string.Empty);
                        break;
                    // Area to be ventilated
                    case 2:
                        _EntDtCmd.Initvalue(8);
                        this.txtAreaToBeVentilatedDecimal.Text = _EntDtCmd.AreaToBeVentilatedRoundingDecimal.ToString();
                        RdbAreaToBeVentilatedRounding = _EntDtCmd.AreaToBeVentilatedRoundingOtp;
                        this.errPvd.SetError(this.txtAreaToBeVentilatedDecimal, string.Empty);
                        break;
                }
            }
            //d/h
            else if (btn.Handle == btnDefaultD.Handle)
            {
                _EntDtCmd.Initvalue(3);
                this.txtdhDecimal.Text = _EntDtCmd.DHRoundingDecimal.ToString();
                RdbDHRounding = _EntDtCmd.DHRoundingOpt;
                this.errPvd.SetError(this.txtdhDecimal, string.Empty);
            }
            //Effective opening area
            else if (btn.Handle == btnDefaultO.Handle)
            {
                _EntDtCmd.Initvalue(4);
                this.txtEffectiveOpeningAreaDecimal.Text = _EntDtCmd.EffectiveOpeningAreaRoundingDecimal.ToString();
                RdbEffectiveOpeningAreaRounding = _EntDtCmd.EffectiveOpeningAreaRoundingOpt;
                this.errPvd.SetError(this.txtEffectiveOpeningAreaDecimal, string.Empty);
            }
            // Effective light area
            else if (btn.Handle == btnDefaultL.Handle)
            {
                _EntDtCmd.Initvalue(5);
                this.txtEffectiveLightingAreaDecimal.Text = _EntDtCmd.EffectiveLightingAreaRoundingDecimal.ToString();
                RdbEffectiveLightingAreaRounding = _EntDtCmd.EffectiveLightingAreaRoundingOpt;
                this.errPvd.SetError(this.txtEffectiveLightingAreaDecimal, string.Empty);
            }
            //Effective smoke extraction area
            else if (btn.Handle == btnDefaultS.Handle)
            {
                _EntDtCmd.Initvalue(7);
                this.txtEffectiveSmokeExtractionAreaDecimal.Text = _EntDtCmd.EffectiveSmokeExtractionAreaRoundingDecimal.ToString();
                RdbEffectiveSmokeExtractionAreaRounding = _EntDtCmd.EffectiveSmokeExtractionAreaRoundingOtp;
                this.errPvd.SetError(this.txtEffectiveSmokeExtractionAreaDecimal, string.Empty);
            }
            //Effective ventilation area
            else if (btn.Handle == btnDefaultV.Handle)
            {
                _EntDtCmd.Initvalue(9);
                this.txtEffectiveVentilationAreaDecimal.Text = _EntDtCmd.EffectiveVentilationAreaRoundingDecimal.ToString();
                RdbEffectiveVentilationAreaRounding = _EntDtCmd.EffectiveVentilationAreaRoundingOtp;
                this.errPvd.SetError(this.txtEffectiveVentilationAreaDecimal, string.Empty);
            }
            else
                return;
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtAreaToBeSmokedDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtAreaToBeSmokedDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtAreaToBeSmokedDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtAreaToBeSmokedDecimal.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtEffectiveVentilationAreaDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtEffectiveVentilationAreaDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtEffectiveVentilationAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveVentilationAreaDecimal.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtEffectiveSmokeExtractionAreaDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtEffectiveSmokeExtractionAreaDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtEffectiveSmokeExtractionAreaDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtEffectiveSmokeExtractionAreaDecimal.Text.Trim(), true));
        }

        /// ================================================================================
        /// <summary>Handles the Validated event of the txtAreaToBeVentilatedDecimal control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System.EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        private void txtAreaToBeVentilatedDecimal_Validated(object sender, EventArgs e)
        {
            this.errPvd.SetError(this.txtAreaToBeVentilatedDecimal, _EntDtCmd.SetErrPvdNumeric(this.txtAreaToBeVentilatedDecimal.Text.Trim(), true));
        }

        #endregion Event
    }
}