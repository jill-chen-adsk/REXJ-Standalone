using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using UTILS = SectionListRC.Utils;

namespace SectionListRC.Setting
{
    public partial class FormBeamListSetting1 : Form
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        // 左のあき - 梁
        private string _BeamLeftSpace;

        // 右のあき - 梁
        private string _BeamRightSpace;

        // 中間あきタイプ
        private int _BeamCenterSpaceType;

        // 中間のあき
        private string _BeamCenterSpace;

        // 上のあき - 梁
        private string _BeamTopSpace;

        // 下のあき - 梁
        private string _BeamBottomSpace;

        // 位置表示枠高さ
        private string _PotsitionFrameHeight;

        // 肋筋括弧表示
        private int _StirrupBracketShow;

        // 追加枠数 - 梁
        private string _BeamAddFrameNumber;

        // 位置表示枠タイトル表示
        private int _PositionFrameTitleShow;

        // 位置表示枠区切り線表示
        private int _PositionFrameSpaceLineShow;

        // 全断面タイトル
        private string _AllSectionTitle;

        // 端部タイトル
        private string _EdgeTitle;

        // 中央部タイトル
        private string _CenterSection;

        // i端タイトル
        private string _ItanSection;

        // j端タイトル
        private string _JtanSection;

        // 片持ち梁元端タイトル
        private string _CantileverStartTitle;

        // 片持ち梁先端タイトル
        private string _CantileverEndTitle;

        // 肋筋枠タイトル
        private string _StirrupFrameTitle;

        // 肋筋枠区切り記号
        private string _StirrupFrameSpaceSymbol;

        // かぶり厚 - 梁
        private string _BeamProtectThick;

        /// <summary>操作結果</summary>
        private int _Result;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        ///
        /// <history>2013/02/05 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public FormBeamListSetting1(SectionListRC.Components.Attribute cmpAttribute,
                                    string settingFileName,
                                    string settingFileDirectory)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InitializeComponent();
            _CmpAttribute = cmpAttribute;

            _Result = 0;

            _SettingFileName = settingFileName;
            _SettingFileDirectory = settingFileDirectory;
            
            SetData();
            SetSettingValue();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2013/02/07 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        private void SetData()
        {
            this.cmbBoxCenterGap.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_SPECIFY"));
            this.cmbBoxCenterGap.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_UNITY"));
            this.cmbBoxCenterGap.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_AVERAGE"));

            this.txtBoxPositionFrameHeight.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamListLeftGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamListCenterGap1.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamListCenterGap2.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamListRightGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamListTopGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxBeamListBottomGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxAddFrameNumber.TextAlign = HorizontalAlignment.Right;
            this.txtBoxKaburi.TextAlign = HorizontalAlignment.Right;

            this.txtBoxBeamListLeftGap.MaxLength = 5;
            this.txtBoxBeamListCenterGap1.MaxLength = 5;
            this.txtBoxBeamListRightGap.MaxLength = 5;
            this.txtBoxBeamListTopGap.MaxLength = 5;
            this.txtBoxBeamListBottomGap.MaxLength = 5;
            this.txtBoxPositionFrameHeight.MaxLength = 5;
            this.txtBoxAddFrameNumber.MaxLength = 2;

            this.cmbBoxStirrupBracket.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACKETNOSHOW"));
            this.cmbBoxStirrupBracket.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACKETSHOW"));

            this.cmbBoxPositionFrameTitleShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_ALL"));
            this.cmbBoxPositionFrameTitleShow.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_TOPONLY"));

            this.cmbBoxStirrupBracket.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBoxCenterGap.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBoxPositionFrameTitleShow.DropDownStyle = ComboBoxStyle.DropDownList;

            SetCmbBoxDropWidth(this.cmbBoxStirrupBracket);
            SetCmbBoxDropWidth(this.cmbBoxCenterGap);
            SetCmbBoxDropWidth(this.cmbBoxPositionFrameTitleShow);
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2013/02/07 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/02/16 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTSETTING") ;

            this.grpBoxPositionFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_POSITIONFRAME");

            this.lblPositionFrameHeightMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.grpBoxPositionSeparator.Text = _CmpAttribute.ResourceText("IDS_TXT_SEPARATORLINE");
            this.rdoBtnPositionSeparatorShow.Text = _CmpAttribute.ResourceText("IDS_TXT_SHOW");
            this.rdoBtnPositionSeparatorNoShow.Text = _CmpAttribute.ResourceText("IDS_TXT_NOSHOW");

            this.grpBoxPositionFrameTitleShow.Text = _CmpAttribute.ResourceText("IDS_TXT_TITLESHOW");
            this.lblPositionFrameTitleShowExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_TITLESHOWEXPLAIN");

            this.grpBoxPositionFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_TITLE");
            this.lblPositionFrameTitleExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_POSITIONFRAMETITLEEXPLAIN");
            this.lblAllSection.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLSECTION");
            this.lblEndSection.Text = _CmpAttribute.ResourceText("IDS_TXT_ENDSECTION");
            this.lblCantiLeverBase.Text = _CmpAttribute.ResourceText("IDS_TXT_CANTIBASE");
            this.lblCantiLeverEnd.Text = _CmpAttribute.ResourceText("IDS_TXT_CANTIEND");
            this.lblCenterSection.Text = _CmpAttribute.ResourceText("IDS_TXT_CENTERSECTION");
            this.lblItanSecction.Text = _CmpAttribute.ResourceText("IDS_TXT_ITANSECTION");
            this.lblJtanSection.Text = _CmpAttribute.ResourceText("IDS_TXT_JTANSECTION");

            this.grpBoxBeamSectionFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SECTIONFRAME");

            this.lblBeamSectionFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SECTIONFRAMEEXPLAIN");
            this.grpBoxBeamListCenterGap.Text = _CmpAttribute.ResourceText("IDS_TXT_CENTERGAP");
            this.lblBeamListLeftGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamListCenterGapMM1.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamListCenterGapMM2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamListRightGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamListTopGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblBeamListBottomGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.grpBoxStirrupFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_STIRRUPFRAME");
            this.lblStirrupFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_HOOPFRAMEEXPLAIN");
            this.lblStirrupFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_TITLE");
            this.lblStirrupFrameSeparator.Text = _CmpAttribute.ResourceText("IDS_TXT_SEPARATOR");

            this.grpBoxWriteParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_WRITEPARAMETER");
            this.lblWriteParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_WRITEPARAMETEREXPLAIN");
            this.lblKaburi.Text = _CmpAttribute.ResourceText("IDS_TXT_KABURI");
            this.lblmm.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.btnToSettingFromBeamListSetting1.Text = _CmpAttribute.ResourceText("IDS_TXT_COMMONSETTING");
            this.btnToColumnListSettingFromBeamListSetting1.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNLISTSETTING");
            this.btnToBeamListSetteing2FromBeamListSetting1.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTSETTING2");
            this.btnOverWriteSave.Text = _CmpAttribute.ResourceText("IDS_TXT_OVERWRITESAVE");
            this.btnSaveAs.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVEAS");
            this.btnEnd.Text = _CmpAttribute.ResourceText("IDS_TXT_END");

            this.toolTipHelp.SetToolTip(this.pictureBoxHelp, _CmpAttribute.ResourceText("IDS_TXT_SHOWHELPVIEW"));
        }

        /// ================================================================================
        /// <summary>設定値設定</summary>
        ///
        /// <history><p>2013/04/11 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetSettingValue()
        {
            GetSettingValue();

            this.cmbBoxCenterGap.SelectedIndex = _BeamCenterSpaceType;

            this.txtBoxBeamListLeftGap.Text = _BeamLeftSpace;
            this.txtBoxBeamListRightGap.Text = _BeamRightSpace;
            this.txtBoxBeamListTopGap.Text = _BeamTopSpace;
            this.txtBoxBeamListBottomGap.Text = _BeamBottomSpace;

            if (_BeamCenterSpaceType == 0)
            {
                this.txtBoxBeamListCenterGap1.Text = _BeamCenterSpace;
            }
            else if (_BeamCenterSpaceType == 1)
            {
                double left = 0;
                double right = 0;

                double.TryParse(_BeamLeftSpace, out left);
                double.TryParse(_BeamRightSpace, out right);

                this.txtBoxBeamListCenterGap1.Text = (left + right).ToString();
            }
            else if (_BeamCenterSpaceType == 2)
            {
                double left = 0;
                double right = 0;

                double.TryParse(_BeamLeftSpace, out left);
                double.TryParse(_BeamRightSpace, out right);

                this.txtBoxBeamListCenterGap1.Text = ((left + right) / 2).ToString();
            }
            this.txtBoxBeamListCenterGap1.Text = this.txtBoxBeamListCenterGap1.Text;

            this.txtBoxPositionFrameHeight.Text = _PotsitionFrameHeight;

            if (_PositionFrameSpaceLineShow == 0)
            {
                this.rdoBtnPositionSeparatorShow.Checked = true;
            }
            else
            {
                this.rdoBtnPositionSeparatorNoShow.Checked = true;
            }

            this.cmbBoxStirrupBracket.SelectedIndex = _StirrupBracketShow;
            this.txtBoxAddFrameNumber.Text = _BeamAddFrameNumber;
            this.cmbBoxPositionFrameTitleShow.SelectedIndex = _PositionFrameTitleShow;
            if (_PositionFrameSpaceLineShow == 0)
            {
                this.rdoBtnPositionSeparatorShow.Checked = true;
            }
            else
            {
                this.rdoBtnPositionSeparatorNoShow.Checked = true;
            }

            this.txtBoxAllSection.Text = _AllSectionTitle;
            this.txtBoxEndSection.Text = _EdgeTitle;
            this.txtBoxCenterSection.Text = _CenterSection;
            this.txtBoxItanSection.Text = _ItanSection;
            this.txtBoxJtanSection.Text = _JtanSection;
            this.txtBoxCantiLeverBase.Text = _CantileverStartTitle;
            this.txtBoxCantiLeverEnd.Text = _CantileverEndTitle;

            this.txtBoxStirrupFrameTitle.Text = _StirrupFrameTitle;
            this.txtBoxStirrupFrameSeparator.Text = _StirrupFrameSpaceSymbol;

            this.txtBoxKaburi.Text = _BeamProtectThick;
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history><p>2013/04/10 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        public void GetSettingValue()
        {
            string fullName = _SettingFileDirectory + _SettingFileName;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            if (System.IO.File.Exists(fullName))
            {
                string[] strAry = System.IO.File.ReadAllLines(fullName, enc);

                //if (strAry.Length == 61 || strAry.Length == 62)
                {
                    _BeamLeftSpace = strAry[32];
                    _BeamRightSpace = strAry[33];
                    int.TryParse(strAry[34], out _BeamCenterSpaceType);
                    _BeamCenterSpace = strAry[35];
                    _BeamTopSpace = strAry[36];
                    _BeamBottomSpace = strAry[37];
                    _PotsitionFrameHeight = strAry[38];
                    int.TryParse(strAry[39], out _StirrupBracketShow);
                    _BeamAddFrameNumber = strAry[40];
                    int.TryParse(strAry[41], out _PositionFrameTitleShow);
                    int.TryParse(strAry[42], out _PositionFrameSpaceLineShow);
                    _AllSectionTitle = strAry[43];
                    _EdgeTitle = strAry[44];
                    _CenterSection = strAry[45];
                    _ItanSection = strAry[46];
                    _JtanSection = strAry[47];
                    _CantileverStartTitle = strAry[48];
                    _CantileverEndTitle = strAry[49];
                    _StirrupFrameTitle = strAry[50];
                    _StirrupFrameSpaceSymbol = strAry[51];

                    _BeamProtectThick = strAry[62];
                }
            }
        }

        /// ================================================================================
        /// <summary>コンボボックス - ドロップダウン幅設定</summary>
        ///
        /// <history><p>2013/02/13 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetCmbBoxDropWidth(System.Windows.Forms.ComboBox cmbBox)
        {
            if (cmbBox.Items.Count > 0)
            {
                System.Drawing.Graphics graphics = this.CreateGraphics();

                float maxWidth = 0;

                // 最大幅取得
                foreach (object item in cmbBox.Items)
                {
                    maxWidth = System.Math.Max(maxWidth, graphics.MeasureString(item.ToString(), cmbBox.Font).Width);
                }

                // 余白
                maxWidth += 15;

                // 切り上げ、int型に変換
                int newWidth = (int)System.Math.Ceiling((decimal)maxWidth);

                // ドロップダウン幅の変更
                if (cmbBox.DropDownWidth < newWidth)
                {
                    cmbBox.DropDownWidth = newWidth;
                }
            }
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定(0＜,≦100)</summary>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private bool IsDoubleString(string strVal)
        {
            bool ret = false;

            double outDouble = 0;

            if (double.TryParse(strVal, out outDouble))
            {
                if (outDouble != 0 && outDouble != 0.0 && outDouble > 0 && outDouble <= 100)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定(> 0)</summary>
        ///
        /// <history><p>2019/10/15</p></history>
        /// ================================================================================
        private bool IsDoubleStringGreaterThanZero(string strVal)
        {
            bool ret = false;

            double outDouble = 0;

            if (double.TryParse(strVal, out outDouble))
            {
                if (outDouble != 0 && outDouble != 0.0 && outDouble > 0)
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字列の整数値判定</summary>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private bool IsIntString(string strVal, int mode)
        {
            bool ret = false;

            int outInt = 0;

            if (int.TryParse(strVal, out outInt))
            {
                if (outInt > 10)
                {
                    ret = false;
                    return ret;
                }

                if (mode == 0)
                {
                    ret = true;
                }
                else
                {
                    if (outInt != 0)
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ボタン使用可否切り替え</summary>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void BtnEnabledChange(bool enableBool)
        {
            this.btnOverWriteSave.Enabled = enableBool;
            this.btnToSettingFromBeamListSetting1.Enabled = enableBool;
            this.btnToColumnListSettingFromBeamListSetting1.Enabled = enableBool;
            this.btnToBeamListSetteing2FromBeamListSetting1.Enabled = enableBool;
        }

        /// ================================================================================
        /// <summary>コントロールの入力判定</summary>
        ///
        /// <param name="ctrl">テキストボックス、コンボボックス、ラジオボタン</param>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private bool InputJudge(System.Windows.Forms.Control ctrl)
        {
            bool ret = true;

            if (ctrl is System.Windows.Forms.TextBox)
            {
                System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                if (txtBox.Text == "" || txtBox.Text == null)
                {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.ComboBox)
            {
                System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                if (cmbBox.SelectedItem == null)
                {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.RadioButton)
            {
                // 親をとって、親に含まれるラジオボタンを取得し、チェック
                System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                System.Windows.Forms.Control.ControlCollection ctrlCollection = rdoBtn.Parent.Controls;

                bool check = false;

                foreach (System.Windows.Forms.Control cont in ctrlCollection)
                {
                    if (cont is System.Windows.Forms.RadioButton)
                    {
                        System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)cont;
                        check = rb.Checked;

                        if (check == true)
                        {
                            break;
                        }
                    }
                }

                if (check == false)
                {
                    ret = false;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>コントロール内全コントロール</summary>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private System.Windows.Forms.Control[] GetCtrls(System.Windows.Forms.Control ctrl)
        {
            Collections.ArrayList ret = new Collections.ArrayList();

            foreach (System.Windows.Forms.Control c in ctrl.Controls)
            {
                ret.Add(c);
                ret.AddRange(GetCtrls(c));
            }

            return (System.Windows.Forms.Control[])ret.ToArray(typeof(System.Windows.Forms.Control));
        }

        /// ================================================================================
        /// <summary>未入力の入力</summary>
        ///
        /// <history><p>2013/05/16 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetUnWrite()
        {
            if (this.txtBoxPositionFrameHeight.Text == "" || this.txtBoxPositionFrameHeight.Text == null)
            {
                this.txtBoxPositionFrameHeight.Text = "4.5";
            }
            if (this.rdoBtnPositionSeparatorShow.Checked == false && this.rdoBtnPositionSeparatorNoShow.Checked == false)
            {
                this.rdoBtnPositionSeparatorShow.Checked = true;
            }
            if (this.txtBoxAllSection.Text == "" || this.txtBoxAllSection.Text == null)
            {
                this.txtBoxAllSection.Text = "全断";
            }
            if (this.txtBoxEndSection.Text == "" || this.txtBoxEndSection.Text == null)
            {
                this.txtBoxEndSection.Text = "端部";
            }
            if (this.txtBoxCenterSection.Text == "" || this.txtBoxCenterSection.Text == null)
            {
                this.txtBoxCenterSection.Text = "中央";
            }
            if (this.txtBoxItanSection.Text == "" || this.txtBoxItanSection.Text == null)
            {
                this.txtBoxItanSection.Text = "始端";
            }
            if (this.txtBoxJtanSection.Text == "" || this.txtBoxJtanSection.Text == null)
            {
                this.txtBoxJtanSection.Text = "終端";
            }
            if (this.txtBoxCantiLeverBase.Text == "" || this.txtBoxCantiLeverBase.Text == null)
            {
                this.txtBoxCantiLeverBase.Text = "元端";
            }
            if (this.txtBoxCantiLeverEnd.Text == "" || this.txtBoxCantiLeverEnd.Text == null)
            {
                this.txtBoxCantiLeverEnd.Text = "先端";
            }
            if (this.txtBoxBeamListRightGap.Text == "" || this.txtBoxBeamListRightGap.Text == null)
            {
                this.txtBoxBeamListRightGap.Text = "12";
            }
            if (this.txtBoxBeamListCenterGap1.Text == "" || this.txtBoxBeamListCenterGap1.Text == null)
            {
                this.txtBoxBeamListCenterGap1.Text = "15";
            }
            if (this.txtBoxBeamListCenterGap2.Text == "" || this.txtBoxBeamListCenterGap2.Text == null)
            {
                this.txtBoxBeamListCenterGap2.Text = "15";
            }
            if (this.txtBoxBeamListLeftGap.Text == "" || this.txtBoxBeamListLeftGap.Text == null)
            {
                this.txtBoxBeamListLeftGap.Text = "12";
            }
            if (this.txtBoxBeamListTopGap.Text == "" || this.txtBoxBeamListTopGap.Text == null)
            {
                this.txtBoxBeamListTopGap.Text = "11";
            }
            if (this.txtBoxBeamListBottomGap.Text == "" || this.txtBoxBeamListBottomGap.Text == null)
            {
                this.txtBoxBeamListBottomGap.Text = "11";
            }
            if (this.txtBoxAddFrameNumber.Text == "" || this.txtBoxAddFrameNumber.Text == null)
            {
                this.txtBoxAddFrameNumber.Text = "0";
            }
            if (this.txtBoxStirrupFrameTitle.Text == "" || this.txtBoxStirrupFrameTitle.Text == null)
            {
                this.txtBoxStirrupFrameTitle.Text = "肋筋";
            }
            if (this.txtBoxStirrupFrameSeparator.Text == "" || this.txtBoxStirrupFrameSeparator.Text == null)
            {
                this.txtBoxStirrupFrameSeparator.Text = "-";
            }

            if (this.cmbBoxStirrupBracket.SelectedItem == null)
            {
                this.cmbBoxStirrupBracket.SelectedIndex = 0;
                this.lblStirrupSample.Text = _CmpAttribute.ResourceText("IDS_TXT_GRIDERREBARWITHBRACKET");
            }

            if (this.cmbBoxCenterGap.SelectedItem == null)
            {
                this.cmbBoxCenterGap.SelectedIndex = 1;
            }

            if (this.cmbBoxPositionFrameTitleShow.SelectedItem == null)
            {
                this.cmbBoxPositionFrameTitleShow.SelectedIndex = 0;
            }

            if (this.txtBoxKaburi.Text == "" || this.txtBoxKaburi.Text == null)
            {
                this.txtBoxKaburi.Text = "40";
            }
        }

        /// ================================================================================
        /// <summary>画像サイズ補正</summary>
        ///
        /// <history><p>2015/04/30 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private
        void SetDPISizing()
        {
            // サイズ補正
            System.Drawing.Graphics gra = this.CreateGraphics();
            float dpiX = gra.DpiX;
            float dpiY = gra.DpiY;

            Bitmap bmp1 = Resources.Image.GapSetting_Beam;
            Bitmap bmp2 = Resources.Image.PositionSeparator_Show;
            Bitmap bmp3 = Resources.Image.PositionSeparator_NoShow;
            Bitmap bmp4 = Resources.Image.help_24x24;

            // 係数
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictureBoxBeam.SizeMode = PictureBoxSizeMode.AutoSize;
            Bitmap newBmp = new Bitmap((int)(bmp1.Width * coefficientX), (int)(bmp1.Height * coefficientY));
            this.pictureBoxBeam.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictureBoxBeam.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBoxBeam.BackColor);
            g.DrawImage(bmp1, 0, 0, (float)(bmp1.Width * coefficientX), (float)(bmp1.Height * coefficientY));
            this.pictureBoxBeam.Refresh();

            this.pictureBoxPositionSeparatorShow.SizeMode = PictureBoxSizeMode.AutoSize;
            newBmp = new Bitmap((int)(bmp2.Width * coefficientX), (int)(bmp2.Height * coefficientY));
            this.pictureBoxPositionSeparatorShow.Image = newBmp;
            g = Graphics.FromImage(this.pictureBoxPositionSeparatorShow.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBoxPositionSeparatorShow.BackColor);
            g.DrawImage(bmp2, 1, 1, (float)(bmp2.Width * coefficientX), (float)(bmp2.Height * coefficientY));
            this.pictureBoxPositionSeparatorShow.Refresh();

            this.pictureBoxPositionSeparatorNoShow.SizeMode = PictureBoxSizeMode.AutoSize;
            newBmp = new Bitmap((int)(bmp3.Width * coefficientX), (int)(bmp3.Height * coefficientY));
            this.pictureBoxPositionSeparatorNoShow.Image = newBmp;
            g = Graphics.FromImage(this.pictureBoxPositionSeparatorNoShow.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBoxPositionSeparatorNoShow.BackColor);
            g.DrawImage(bmp3, 1, 1, (float)(bmp3.Width * coefficientX), (float)(bmp3.Height * coefficientY));
            this.pictureBoxPositionSeparatorNoShow.Refresh();

            newBmp = new Bitmap((int)(bmp4.Width * coefficientX), (int)(bmp4.Height * coefficientY));
            this.pictureBoxHelp.Image = newBmp;
            g = Graphics.FromImage(this.pictureBoxHelp.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBoxHelp.BackColor);
            g.DrawImage(bmp4, 0, 0, (float)(bmp4.Width * coefficientX), (float)(bmp4.Height * coefficientY));
            this.pictureBoxHelp.Refresh();
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>設定値 - 梁リスト設定1</summary>
        /// ================================================================================
        public Collections.Generic.IList<string> SettingValues_Beam1
        {
            get
            {
                SetUnWrite();

                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                ret.Add(this.txtBoxBeamListLeftGap.Text);
                ret.Add(this.txtBoxBeamListRightGap.Text);
                ret.Add(this.cmbBoxCenterGap.SelectedIndex.ToString());
                ret.Add(this.txtBoxBeamListCenterGap1.Text);
                ret.Add(this.txtBoxBeamListTopGap.Text);
                ret.Add(this.txtBoxBeamListBottomGap.Text);
                ret.Add(this.txtBoxPositionFrameHeight.Text);
                ret.Add(this.cmbBoxStirrupBracket.SelectedIndex.ToString());
                ret.Add(this.txtBoxAddFrameNumber.Text);
                ret.Add(this.cmbBoxPositionFrameTitleShow.SelectedIndex.ToString());
                if (this.rdoBtnPositionSeparatorShow.Checked == true)
                {
                    ret.Add("0");
                }
                else
                {
                    ret.Add("1");
                }
                ret.Add(this.txtBoxAllSection.Text);
                ret.Add(this.txtBoxEndSection.Text);
                ret.Add(this.txtBoxCenterSection.Text);
                ret.Add(this.txtBoxItanSection.Text);
                ret.Add(this.txtBoxJtanSection.Text);
                ret.Add(this.txtBoxCantiLeverBase.Text);
                ret.Add(this.txtBoxCantiLeverEnd.Text);
                ret.Add(this.txtBoxStirrupFrameTitle.Text);
                ret.Add(this.txtBoxStirrupFrameSeparator.Text);

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>設定値 - 梁リスト設定 - 描画パラメータ</summary>
        /// ================================================================================
        public string SettingValue_WriteParam
        {
            get
            {
                return this.txtBoxKaburi.Text;
            }
        }

        /// ================================================================================
        /// <summary>操作結果 - 梁リスト設定1</summary>
        /// ================================================================================
        public int BeamListSettingResult1
        {
            get
            {
                return _Result;
            }
        }

        /// ================================================================================
        /// <summary>全項目の入力判定</summary>
        /// ================================================================================
        private bool AllInputJudge
        {
            get
            {
                bool ret = true;

                foreach (System.Windows.Forms.Control ctrl in GetCtrls(this))
                {
                    // テキストボックス
                    if (ctrl is System.Windows.Forms.TextBox)
                    {
                        System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                        if (txtBox.Text == "" || txtBox.Text == null)
                        {
                            ret = false;
                            string na = txtBox.Name;
                            break;
                        }
                    }
                    // コンボボックス
                    else if (ctrl is System.Windows.Forms.ComboBox)
                    {
                        System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                        if (cmbBox.SelectedItem == null)
                        {
                            ret = false;
                            break;
                        }
                    }
                    // ラジオボタン
                    else if (ctrl is System.Windows.Forms.RadioButton)
                    {
                        // 親をとって、親に含まれるラジオボタンを取得し、チェック
                        System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                        System.Windows.Forms.Control.ControlCollection ctrls = rdoBtn.Parent.Controls;

                        bool check = false;

                        foreach (System.Windows.Forms.Control cont in ctrls)
                        {
                            if (cont is System.Windows.Forms.RadioButton)
                            {
                                System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)cont;
                                check = rb.Checked;

                                if (check == true)
                                {
                                    break;
                                }
                            }
                        }

                        if (check == false)
                        {
                            ret = false;
                            break;
                        }
                    }
                }

                return ret;
            }
        }

        #endregion Properties

        // イベント

        #region Events

        // ロード
        private void FormBeamListSetting1_Load(object sender, EventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(2, 2, 24, 24);
            //this.pictureBoxHelp.Region = new System.Drawing.Region(path);

            SetText();
            SetSettingValue();

            this.cmbBoxCenterGap.Select();

            SetUnWrite();

            BtnEnabledChange(AllInputJudge);

            SetDPISizing();
        }

        #region 入力文字制限 - 数字

        private void txtBoxBeamListLeftGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtBoxBeamListCenterGap1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtBoxBeamListRightGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtBoxBeamListTopGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtBoxBeamListBottomGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtBoxPositionFrameHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtBoxAddFrameNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        private void txtBoxKaburi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        #endregion 入力文字制限 - 数字

        #region Change イベント

        // 中間の空き2は中間の空き1と同じ
        private void txtBoxBeamListCenterGap1_TextChanged(object sender, EventArgs e)
        {
            this.txtBoxBeamListCenterGap2.Text = this.txtBoxBeamListCenterGap1.Text;

            this.errorProviderInvalid.SetError(this.txtBoxBeamListCenterGap1, "");

            if (IsDoubleString(this.txtBoxBeamListCenterGap1.Text))
            {
                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        #region 中間のあき

        // 中間タイプ
        private void cmbBoxCenterGap_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 指定
            if (this.cmbBoxCenterGap.SelectedIndex == 0)
            {
                this.txtBoxBeamListCenterGap1.Enabled = true;
            }
            // 合計
            else if (this.cmbBoxCenterGap.SelectedIndex == 1)
            {
                this.txtBoxBeamListCenterGap1.Enabled = false;

                float f1 = 0;
                float f2 = 0;

                if (float.TryParse(this.txtBoxBeamListLeftGap.Text, out f1) && float.TryParse(this.txtBoxBeamListRightGap.Text, out f2))
                {
                    this.txtBoxBeamListCenterGap1.Text = (f1 + f2).ToString();
                }
            }
            // 平均
            else if (this.cmbBoxCenterGap.SelectedIndex == 2)
            {
                this.txtBoxBeamListCenterGap1.Enabled = false;

                float f1 = 0;
                float f2 = 0;

                if (float.TryParse(this.txtBoxBeamListLeftGap.Text, out f1) && float.TryParse(this.txtBoxBeamListRightGap.Text, out f2))
                {
                    this.txtBoxBeamListCenterGap1.Text = ((f1 + f2) / 2).ToString();
                }
            }
        }

        // 左の空き
        private void txtBoxBeamListLeftGap_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxBeamListLeftGap, "");

            if (IsDoubleString(this.txtBoxBeamListLeftGap.Text))
            {
                // 合計
                if (this.cmbBoxCenterGap.SelectedIndex == 1)
                {
                    float f1 = 0;
                    float f2 = 0;

                    if (float.TryParse(this.txtBoxBeamListLeftGap.Text, out f1) && float.TryParse(this.txtBoxBeamListRightGap.Text, out f2))
                    {
                        this.txtBoxBeamListCenterGap1.Text = (f1 + f2).ToString();
                    }
                }
                // 平均
                if (this.cmbBoxCenterGap.SelectedIndex == 2)
                {
                    float f1 = 0;
                    float f2 = 0;

                    if (float.TryParse(this.txtBoxBeamListLeftGap.Text, out f1) && float.TryParse(this.txtBoxBeamListRightGap.Text, out f2))
                    {
                        this.txtBoxBeamListCenterGap1.Text = ((f1 + f2) / 2).ToString();
                    }
                }

                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        // 右の空き
        private void txtBoxBeamListRightGap_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxBeamListRightGap, "");

            if (IsDoubleString(this.txtBoxBeamListRightGap.Text))
            {
                // 合計
                if (this.cmbBoxCenterGap.SelectedIndex == 1)
                {
                    float f1 = 0;
                    float f2 = 0;

                    if (float.TryParse(this.txtBoxBeamListLeftGap.Text, out f1) && float.TryParse(this.txtBoxBeamListRightGap.Text, out f2))
                    {
                        this.txtBoxBeamListCenterGap1.Text = (f1 + f2).ToString();
                    }
                }
                // 平均
                if (this.cmbBoxCenterGap.SelectedIndex == 2)
                {
                    float f1 = 0;
                    float f2 = 0;

                    if (float.TryParse(this.txtBoxBeamListLeftGap.Text, out f1) && float.TryParse(this.txtBoxBeamListRightGap.Text, out f2))
                    {
                        this.txtBoxBeamListCenterGap1.Text = ((f1 + f2) / 2).ToString();
                    }
                }

                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        #endregion 中間のあき

        // 位置表示枠高さ
        private void txtBoxPositionFrameHeight_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxPositionFrameHeight, "");

            if (IsDoubleString(this.txtBoxPositionFrameHeight.Text))
            {
                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        // 上の空き
        private void txtBoxBeamListTopGap_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxBeamListTopGap, "");

            if (IsDoubleString(this.txtBoxBeamListTopGap.Text))
            {
                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        // 下の空き
        private void txtBoxBeamListBottomGap_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxBeamListBottomGap, "");

            if (IsDoubleString(this.txtBoxBeamListBottomGap.Text))
            {
                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        // 追加枠数
        private void txtBoxAddFrameNumber_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxAddFrameNumber, "");

            if (IsIntString(this.txtBoxAddFrameNumber.Text, 0))
            {
                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        // 括弧表示
        private void cmbBoxStirrupBracket_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBoxStirrupBracket.SelectedIndex == 0)
            {
                this.lblStirrupSample.Text = _CmpAttribute.ResourceText("IDS_TXT_GIRDERREBARWITHBRACKETNON");
            }
            if (this.cmbBoxStirrupBracket.SelectedIndex == 1)
            {
                this.lblStirrupSample.Text = _CmpAttribute.ResourceText("IDS_TXT_GIRDERREBARWITHBRACKET");
            }

            BtnEnabledChange(AllInputJudge);
        }

        // 全断
        private void txtBoxAllSection_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 始端
        private void txtBoxStartSection_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 終端
        private void txtBoxEndSection_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 片持ち梁元端
        private void txtBoxCantiLeverBase_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 片持ち梁先端
        private void txtBoxCantiLeverEnd_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 肋筋枠タイトル
        private void txtBoxStirrupFrameTitle_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        // 肋筋括弧文字
        private void txtBoxStirrupFrameSeparator_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        //始端部の表示
        private void txtBoxItanSection_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        //終端部の表示
        private void txtBoxJtanSection_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        private void txtBoxCenterSection_TextChanged(object sender, EventArgs e)
        {
            BtnEnabledChange(AllInputJudge);
        }

        private void txtBoxKaburi_TextChanged(object sender, EventArgs e)
        {
            this.errorProviderInvalid.SetError(this.txtBoxKaburi, "");

            if (IsDoubleString(this.txtBoxKaburi.Text))
            {
                if (AllInputJudge == true)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        #endregion Change イベント

        #region コントロールが離れたとき

        // 左の空き
        private void txtBoxBeamListLeftGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxBeamListLeftGap.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListLeftGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                               "\r\n" +
                                                                               _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxBeamListLeftGap.Select();
                this.txtBoxBeamListLeftGap.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListLeftGap, "");
            }
        }

        // 中間の空き
        private void txtBoxBeamListCenterGap1_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxBeamListCenterGap1.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListCenterGap1, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                  "\r\n" +
                                                                                  _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxBeamListCenterGap1.Select();
                this.txtBoxBeamListCenterGap1.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListCenterGap1, "");
            }
        }

        // 右の空き
        private void txtBoxBeamListRightGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxBeamListRightGap.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListRightGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                "\r\n" +
                                                                                _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxBeamListRightGap.Select();
                this.txtBoxBeamListRightGap.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListRightGap, "");
            }
        }

        // 上の空き
        private void txtBoxBeamListTopGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxBeamListTopGap.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListTopGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                              "\r\n" +
                                                                              _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxBeamListTopGap.Select();
                this.txtBoxBeamListTopGap.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListTopGap, "");
            }
        }

        // 下の空き
        private void txtBoxBeamListBottomGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxBeamListBottomGap.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListBottomGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                 "\r\n" +
                                                                                 _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxBeamListBottomGap.Select();
                this.txtBoxBeamListBottomGap.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxBeamListBottomGap, "");
            }
        }

        // 位置表示枠高さ
        private void txtBoxPositionFrameHeight_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxPositionFrameHeight.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxPositionFrameHeight, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                   "\r\n" +
                                                                                   _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxPositionFrameHeight.Select();
                this.txtBoxPositionFrameHeight.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxPositionFrameHeight, "");
            }
        }

        // 追加枠数
        private void txtBoxAddFrameNumber_Leave(object sender, EventArgs e)
        {
            if (!IsIntString(this.txtBoxAddFrameNumber.Text, 0))
            {
                this.errorProviderInvalid.SetError(this.txtBoxAddFrameNumber, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                              "\r\n" +
                                                                              _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTINT") +
                                                                              "\r\n" +
                                                                              _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTZEROTOTEN"));
                this.txtBoxAddFrameNumber.Select();
                this.txtBoxAddFrameNumber.SelectAll();
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxAddFrameNumber, "");
            }
        }

        private void txtBoxKaburi_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleStringGreaterThanZero(this.txtBoxKaburi.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                               "\r\n" +
                                                                               _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTGREATERTHANZERO"));
                this.txtBoxKaburi.Select();
                this.txtBoxKaburi.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi, "");
            }
        }

        #endregion コントロールが離れたとき

        #region ボタン

        // 共通設定へ
        private void btnToSetteingFromBeamListSetting1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 0;

            this.Close();
        }

        // 柱リスト設定へ
        private void btnToColumnListSetteingFromBeamListSetting1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 1;

            this.Close();
        }

        // 梁リスト設定2へ
        private void btnToBeamListSetteing2FromBeamListSetting1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 3;

            this.Close();
        }

        // 上書き保存
        private void btnOverWriteSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 10;

            this.Close();
        }

        // 名前を付けて保存
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 11;

            this.Close();
        }

        // 終了
        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        // ヒント
        private void pictureBoxHelp_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formHint = new SectionListRC.Setting.FormHintView(_CmpAttribute, 0, this);
            formHint.ShowDialog();
        }

        #endregion ボタン

        #endregion Events
    }
}