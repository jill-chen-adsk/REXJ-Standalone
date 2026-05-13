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
    public partial class FormColumnListSetting : Form
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>左のあき - 柱</summary>
        private string _ColumnLeftSpace;

        /// <summary>右のあき - 柱</summary>
        private string _ColumnRightSpace;

        /// <summary>上のあき - 柱</summary>
        private string _ColumnTopSpace;

        /// <summary>下のあき - 柱</summary>
        private string _ColumnBottomSpace;

        /// <summary>帯筋括弧表示</summary>
        private int _HoopBracketShow;

        /// <summary>追加枠数 - 柱</summary>
        private string _ColumnAddFrameNumber;

        /// <summary>主筋表示 - 柱</summary>
        private int _ColumnRebarShow;

        // 帯筋枠タイトル
        private string _HoopFrameTitle;

        // 帯筋枠区切り記号
        private string _HoopFrameSpaceSymbol;

        // かぶり厚 - 角柱
        private string _ColumnProtectThick;

        // 2段筋コーナー配筋フラグ
        private string _2ndRebarCornerSetFlag;

        // かぶり厚 - 円柱
        private string _CylinderProtectThick;

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
        public FormColumnListSetting(SectionListRC.Components.Attribute cmpAttribute,
                                     string settingFileName,
                                     string settingFileDirectory)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InitializeComponent();
            _CmpAttribute = cmpAttribute;
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
        /// <history><p>2013/02/05 Created GSA,Inc Ryo Kuroda</p>
        ///          <p>2013/02/22 Modified GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetData()
        {
            this.cmbBoxHoopBracket.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACKETNOSHOW"));
            this.cmbBoxHoopBracket.Items.Add(_CmpAttribute.ResourceText("IDS_TXT_BRACKETSHOW"));
            this.cmbBoxHoopBracket.DropDownStyle = ComboBoxStyle.DropDownList;

            this.txtBoxColumnListLeftGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxColumnListRightGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxColumnListTopGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxColumnListBottomGap.TextAlign = HorizontalAlignment.Right;
            this.txtBoxColumnAddFrameNumber.TextAlign = HorizontalAlignment.Right;
            this.txtBoxKaburi_Kaku.TextAlign = HorizontalAlignment.Right;
            this.txtBoxKaburi_En.TextAlign = HorizontalAlignment.Right;

            this.txtBoxColumnListLeftGap.MaxLength = 5;
            this.txtBoxColumnListRightGap.MaxLength = 5;
            this.txtBoxColumnListTopGap.MaxLength = 5;
            this.txtBoxColumnListBottomGap.MaxLength = 5;
            this.txtBoxColumnAddFrameNumber.MaxLength = 2;
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history><p>2013/02/05 Created GSA,Inc Ryo Kuroda</p>
        ///           <p>2017/02/16 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMNLISTSETTING") ;

            this.grpBoxColumnSectionFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_SECTIONFRAME");
            this.lblColumnSectionFrameExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SECTIONFRAMEEXPLAIN");
            this.lblColumnListLeftGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblColumnListRightGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblColumnListTopGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblColumnListbottomGapMM.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");

            this.grpBoxColumnRebar.Text = _CmpAttribute.ResourceText("IDS_TXT_MAINREBAR");
            this.lblColumnRebarExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_SECTIONFRAMEREBAREXPLAIN");
            this.rdoBtnColumnRebarNoShow.Text = _CmpAttribute.ResourceText("IDS_TXT_NOSHOW");
            this.rdoBtnColumnRebarPartitionShow.Text = _CmpAttribute.ResourceText("IDS_TXT_REBARPARTITIONSHOW");
            this.rdoBtnColumnRebarShow.Text = _CmpAttribute.ResourceText("IDS_TXT_REBARSHOW");

            this.grpBoxHoopFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_HOOPFRAME");
            this.lblHoopExplain.Text = _CmpAttribute.ResourceText("IDS_TXT_HOOPFRAMEEXPLAIN");
            this.lblHoopFrameTitle.Text = _CmpAttribute.ResourceText("IDS_TXT_HOOPFRAMETITLE");
            this.lblHoopSeparator.Text = _CmpAttribute.ResourceText("IDS_TXT_SEPARATOR");

            this.grpBoxWriteParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_WRITEPARAMETER");
            this.lblWriteParameter.Text = _CmpAttribute.ResourceText("IDS_TXT_WRITEPARAMETEREXPLAIN");
            this.lblKaburi_Kaku.Text = _CmpAttribute.ResourceText("IDS_TXT_KABURI_KAKU");
            this.lblKaburi_En.Text = _CmpAttribute.ResourceText("IDS_TXT_KABURI_EN");
            this.lbl2ndRebarCornerSetFlag.Text = _CmpAttribute.ResourceText("IDS_TXT_2NDREBARFLAG");
            this.lblmm.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.lblmm2.Text = _CmpAttribute.ResourceText("IDS_TXT_MM");
            this.chkBox2ndRebarFlag.Text = "";

            this.btnToSettingFromColumnListSetting.Text = _CmpAttribute.ResourceText("IDS_TXT_COMMONSETTING");
            this.btnToBeamListSetting1FromColumnListSetting.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAMLISTSETTING");
            this.btnOverWriteSave.Text = _CmpAttribute.ResourceText("IDS_TXT_OVERWRITESAVE");
            this.btnSaveAs.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVEAS");
            this.btnEnd.Text = _CmpAttribute.ResourceText("IDS_TXT_END");
        }

        /// ================================================================================
        /// <summary>設定値設定</summary>
        ///
        /// <history><p>2013/04/11 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void SetSettingValue()
        {
            GetSettingValue();

            this.txtBoxColumnListLeftGap.Text = _ColumnLeftSpace;
            this.txtBoxColumnListRightGap.Text = _ColumnRightSpace;
            this.txtBoxColumnListTopGap.Text = _ColumnTopSpace;
            this.txtBoxColumnListBottomGap.Text = _ColumnBottomSpace;
            this.cmbBoxHoopBracket.SelectedIndex = _HoopBracketShow;
            this.txtBoxColumnAddFrameNumber.Text = _ColumnAddFrameNumber;
            if (_ColumnRebarShow == 0) {
                this.rdoBtnColumnRebarNoShow.Checked = true;
            }
            else if (_ColumnRebarShow == 1) {
                this.rdoBtnColumnRebarPartitionShow.Checked = true;
            }
            else if (_ColumnRebarShow == 2) {
                this.rdoBtnColumnRebarShow.Checked = true;
            }
            this.txtBoxHoopFrameTitle.Text = _HoopFrameTitle;
            this.txtBoxHoopSeparator.Text = _HoopFrameSpaceSymbol;

            this.txtBoxKaburi_Kaku.Text = _ColumnProtectThick;
            this.txtBoxKaburi_En.Text = _CylinderProtectThick;

            if (_2ndRebarCornerSetFlag == "0") {
                this.chkBox2ndRebarFlag.Checked = false;
            }
            else if (_2ndRebarCornerSetFlag == "1") {
                this.chkBox2ndRebarFlag.Checked = true;
            }
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

            if (System.IO.File.Exists(fullName)) {
                string[] strAry = System.IO.File.ReadAllLines(fullName, enc);

                //if (strAry.Length == 61 || strAry.Length == 62)
                {
                    _ColumnLeftSpace = strAry[22];
                    _ColumnRightSpace = strAry[23];
                    _ColumnTopSpace = strAry[24];
                    _ColumnBottomSpace = strAry[25];
                    int.TryParse(strAry[26], out _HoopBracketShow);
                    _ColumnAddFrameNumber = strAry[27];
                    int.TryParse(strAry[28], out _ColumnRebarShow);
                    _HoopFrameTitle = strAry[29];
                    _HoopFrameSpaceSymbol = strAry[30];

                    _ColumnProtectThick = strAry[59];
                    _2ndRebarCornerSetFlag = strAry[60];
                    _CylinderProtectThick = strAry[61];
                }
            }
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定</summary>
        ///
        /// <history><p>2013/02/26 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private bool IsDoubleString(string strVal)
        {
            bool ret = false;

            double outDouble = 0;

            if (double.TryParse(strVal, out outDouble)) {
                if (outDouble != 0 && outDouble != 0.0 && outDouble <= 100) {
                    ret = true;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>文字列の小数値判定</summary>
        ///
        /// <history><p>2019/10/2 Created Applied Technology</p></history>
        /// ================================================================================
        private bool IsDoubleStr(string strVal)
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
        /// <param name="mode">0のとき、0を整数に含む</param>
        ///
        /// <history><p>2013/02/26 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private bool IsIntString(string strVal, int mode)
        {
            bool ret = false;

            int outInt = 0;

            if (int.TryParse(strVal, out outInt)) {
                if (outInt > 10) {
                    ret = false;
                    return ret;
                }

                if (mode == 0) {
                    ret = true;
                }
                else {
                    if (outInt != 0) {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ボタン使用可否切り替え</summary>
        ///
        /// <history><p>2013/02/26 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private void BtnEnabledChange(bool enableBool)
        {
            this.btnOverWriteSave.Enabled = enableBool;
            this.btnToSettingFromColumnListSetting.Enabled = enableBool;
            this.btnToBeamListSetting1FromColumnListSetting.Enabled = enableBool;
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

            if (ctrl is System.Windows.Forms.TextBox) {
                System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                if (txtBox.Text == "" || txtBox.Text == null) {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.ComboBox) {
                System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                if (cmbBox.SelectedItem == null) {
                    ret = false;
                }
            }
            else if (ctrl is System.Windows.Forms.RadioButton) {
                // 親をとって、親に含まれるラジオボタンを取得し、チェック
                System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                System.Windows.Forms.Control.ControlCollection ctrlCollection = rdoBtn.Parent.Controls;

                bool check = false;

                foreach (System.Windows.Forms.Control cont in ctrlCollection) {
                    if (cont is System.Windows.Forms.RadioButton) {
                        System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)cont;
                        check = rb.Checked;

                        if (check == true) {
                            break;
                        }
                    }
                }

                if (check == false) {
                    ret = false;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>全コントロール</summary>
        ///
        /// <history><p>2013/02/27 Created GSA,Inc Ryo Kuroda</p></history>
        /// ================================================================================
        private System.Windows.Forms.Control[] GetCtrls(System.Windows.Forms.Control ctrl)
        {
            Collections.ArrayList ret = new Collections.ArrayList();

            foreach (System.Windows.Forms.Control c in ctrl.Controls) {
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
            if (this.txtBoxColumnListLeftGap.Text == "" || this.txtBoxColumnListLeftGap.Text == null) {
                this.txtBoxColumnListLeftGap.Text = "12";
            }
            if (this.txtBoxColumnListRightGap.Text == "" || this.txtBoxColumnListRightGap.Text == null) {
                this.txtBoxColumnListRightGap.Text = "12";
            }
            if (this.txtBoxColumnListTopGap.Text == "" || this.txtBoxColumnListTopGap.Text == null) {
                this.txtBoxColumnListTopGap.Text = "11";
            }
            if (this.txtBoxColumnListBottomGap.Text == "" || this.txtBoxColumnListBottomGap.Text == null) {
                this.txtBoxColumnListBottomGap.Text = "11";
            }

            if (this.cmbBoxHoopBracket.SelectedItem == null) {
                this.cmbBoxHoopBracket.SelectedIndex = 0;
                this.lblHoopSample.Text = _CmpAttribute.ResourceText("IDS_TXT_REBARWITHBRACKET");
            }

            if (this.txtBoxColumnAddFrameNumber.Text == "" || this.txtBoxColumnAddFrameNumber.Text == null) {
                this.txtBoxColumnAddFrameNumber.Text = "0";
            }

            if (this.rdoBtnColumnRebarNoShow.Checked == false && this.rdoBtnColumnRebarPartitionShow.Checked == false && this.rdoBtnColumnRebarShow.Checked == false) {
                this.rdoBtnColumnRebarNoShow.Checked = true;
            }

            if (this.txtBoxHoopFrameTitle.Text == "" || this.txtBoxHoopFrameTitle.Text == null) {
                this.txtBoxHoopFrameTitle.Text = "帯筋";
            }

            if (this.txtBoxHoopSeparator.Text == "" || this.txtBoxHoopSeparator.Text == null) {
                this.txtBoxHoopSeparator.Text = "-";
            }

            if (this.txtBoxKaburi_Kaku.Text == "" || this.txtBoxKaburi_Kaku.Text == null) {
                this.txtBoxKaburi_Kaku.Text = "40";
            }

            if (this.txtBoxKaburi_En.Text == "" || this.txtBoxKaburi_En.Text == null) {
                this.txtBoxKaburi_En.Text = "40";
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

            Bitmap bmp = Resources.Image.GapSetting_Column;

            // 倍率
            double coefficientX = dpiX / 96;
            double coefficientY = dpiY / 96;

            this.pictureBoxColumn.SizeMode = PictureBoxSizeMode.AutoSize;

            Bitmap newBmp = new Bitmap((int)(bmp.Width * coefficientX), (int)(bmp.Height * coefficientY));
            this.pictureBoxColumn.Image = newBmp;
            Graphics g = Graphics.FromImage(this.pictureBoxColumn.Image);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.Clear(this.pictureBoxColumn.BackColor);

            g.DrawImage(bmp, 0, 0, (float)(bmp.Width * coefficientX), (float)(bmp.Height * coefficientY));
            this.pictureBoxColumn.Refresh();
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>全項目の入力判定</summary>
        /// ================================================================================
        private bool AllInputJudge
        {
            get
            {
                bool ret = true;

                // 1つでも未入力ならfalse

                foreach (System.Windows.Forms.Control ctrl in GetCtrls(this)) {
                    // テキストボックス
                    if (ctrl is System.Windows.Forms.TextBox) {
                        System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)ctrl;

                        if (txtBox.Text == "" || txtBox.Text == null) {
                            ret = false;
                            break;
                        }
                    }
                    // コンボボックス
                    else if (ctrl is System.Windows.Forms.ComboBox) {
                        System.Windows.Forms.ComboBox cmbBox = (System.Windows.Forms.ComboBox)ctrl;

                        if (cmbBox.SelectedItem == null) {
                            ret = false;
                            break;
                        }
                    }
                    // ラジオボタン
                    else if (ctrl is System.Windows.Forms.RadioButton) {
                        // 親をとって、親に含まれるラジオボタンを取得し、チェック
                        System.Windows.Forms.RadioButton rdoBtn = (System.Windows.Forms.RadioButton)ctrl;

                        System.Windows.Forms.Control.ControlCollection ctrls = rdoBtn.Parent.Controls;

                        bool check = false;

                        foreach (System.Windows.Forms.Control cont in ctrls) {
                            if (cont is System.Windows.Forms.RadioButton) {
                                System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)cont;
                                check = rb.Checked;

                                if (check == true) {
                                    break;
                                }
                            }
                        }

                        if (check == false) {
                            ret = false;
                            break;
                        }
                    }
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>操作結果 - 柱リスト設定</summary>
        /// ================================================================================
        public int ColumnListSettingResult
        {
            get
            {
                return _Result;
            }
        }

        /// ================================================================================
        /// <summary>設定値 - 柱リスト設定</summary>
        /// ================================================================================
        public Collections.Generic.IList<string> SettingValues_Column
        {
            get
            {
                SetUnWrite();

                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                ret.Add(this.txtBoxColumnListLeftGap.Text);
                ret.Add(this.txtBoxColumnListRightGap.Text);
                ret.Add(this.txtBoxColumnListTopGap.Text);
                ret.Add(txtBoxColumnListBottomGap.Text);
                ret.Add(this.cmbBoxHoopBracket.SelectedIndex.ToString());
                ret.Add(txtBoxColumnAddFrameNumber.Text);
                if (this.rdoBtnColumnRebarNoShow.Checked == true) {
                    ret.Add("0");
                }
                else if (this.rdoBtnColumnRebarPartitionShow.Checked == true) {
                    ret.Add("1");
                }
                else if (this.rdoBtnColumnRebarShow.Checked == true) {
                    ret.Add("2");
                }
                ret.Add(this.txtBoxHoopFrameTitle.Text);
                ret.Add(this.txtBoxHoopSeparator.Text);

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>設定値 - 柱リスト設定 - 描画パラメータ</summary>
        /// ================================================================================
        public Collections.Generic.IList<string> SettingValues_WriteParam
        {
            get
            {
                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                ret.Add(this.txtBoxKaburi_Kaku.Text);

                if (this.chkBox2ndRebarFlag.Checked == false) {
                    ret.Add("0");
                }
                else {
                    ret.Add("1");
                }

                ret.Add(this.txtBoxKaburi_En.Text);

                return ret;
            }
        }

        #endregion Properties

        // イベント
        #region Events

        //ロード
        private void FormColumnListSetting_Load(object sender, EventArgs e)
        {
            SetText();
            SetSettingValue();

            this.txtBoxColumnListLeftGap.Select();

            // 未設定の場合
            SetUnWrite();

            BtnEnabledChange(AllInputJudge);

            SetDPISizing();
        }

        #region 入力文字制限 - 数字

        private void txtBoxColumnListLeftGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxColumnListRightGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxColumnListTopGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxColumnListBottomGap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.') {
                e.Handled = true;
            }
        }

        private void txtBoxKaburi_Kaku_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void txtBoxKaburi_En_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        #endregion 入力文字制限 - 数字

        #region ボタン

        // 共通設定へ
        private void btnToSettingFromColumnListSetting_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 0;

            this.Close();
        }

        // 梁リスト設定へ
        private void btnToBeamListSettingFromColumnListSetting_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 2;

            this.Close();
        }

        // 上書き保存
        private void btnOverWriteSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 8;

            this.Close();
        }

        // 名前を付けて保存
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _Result = 9;

            this.Close();
        }

        // 終了
        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion ボタン

        #region Changed イベント

        private void cmbBoxBracket_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBoxHoopBracket.SelectedIndex == 0) {
                this.lblHoopSample.Text = _CmpAttribute.ResourceText("IDS_TXT_REBARWITHBRACKETNON");
            }
            if (this.cmbBoxHoopBracket.SelectedIndex == 1) {
                this.lblHoopSample.Text = _CmpAttribute.ResourceText("IDS_TXT_REBARWITHBRACKET");
            }
        }

        private void txtBoxColumnListLeftGap_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxColumnListLeftGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListLeftGap, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        private void txtBoxColumnListRightGap_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxColumnListRightGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListRightGap, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        private void txtBoxColumnListTopGap_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxColumnListTopGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListTopGap, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        private void txtBoxColumnListBottomGap_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleString(this.txtBoxColumnListBottomGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListBottomGap, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        private void txtBoxColumnAddFrameNumber_TextChanged(object sender, EventArgs e)
        {
            if (IsIntString(this.txtBoxColumnAddFrameNumber.Text, 0)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnAddFrameNumber, "");

                if (AllInputJudge) {
                    BtnEnabledChange(true);
                }
            }
        }

        private void txtBoxHoopFrameTitle_TextChanged(object sender, EventArgs e)
        {
            if (AllInputJudge) {
                BtnEnabledChange(true);
            }
            else {
                BtnEnabledChange(false);
            }
        }

        private void txtBoxHoopSeparator_TextChanged(object sender, EventArgs e)
        {
            if (AllInputJudge) {
                BtnEnabledChange(true);
            }
            else {
                BtnEnabledChange(false);
            }
        }

        private void txtBoxKaburi_Kaku_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleStr(this.txtBoxKaburi_Kaku.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi_Kaku, "");

                if (AllInputJudge)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        private void txtBoxKaburi_En_TextChanged(object sender, EventArgs e)
        {
            if (IsDoubleStr(this.txtBoxKaburi_En.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi_En, "");

                if (AllInputJudge)
                {
                    BtnEnabledChange(true);
                }
            }
        }

        #endregion Changed イベント

        #region コントロールが離れたとき

        // 左の空き
        private void txtBoxColumnListLeftGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxColumnListLeftGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListLeftGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                 "\r\n" +
                                                                                 _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxColumnListLeftGap.Select();
                this.txtBoxColumnListLeftGap.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListLeftGap, "");
            }
        }

        // 右の空き
        private void txtBoxColumnListRightGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxColumnListRightGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListRightGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                  "\r\n" +
                                                                                  _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxColumnListRightGap.Select();
                this.txtBoxColumnListRightGap.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListRightGap, "");
            }
        }

        // 上の空き
        private void txtBoxColumnListTopGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxColumnListTopGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListTopGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                "\r\n" +
                                                                                _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxColumnListTopGap.Select();
                this.txtBoxColumnListTopGap.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListTopGap, "");
            }
        }

        // 下の空き
        private void txtBoxColumnListBottomGap_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleString(this.txtBoxColumnListBottomGap.Text)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListBottomGap, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                   "\r\n" +
                                                                                   _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTDOUBLE"));
                this.txtBoxColumnListBottomGap.Select();
                this.txtBoxColumnListBottomGap.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxColumnListBottomGap, "");
            }
        }

        // 追加枠数
        private void txtBoxColumnAddFrameNumber_Leave(object sender, EventArgs e)
        {
            if (!IsIntString(this.txtBoxColumnAddFrameNumber.Text, 0)) {
                this.errorProviderInvalid.SetError(this.txtBoxColumnAddFrameNumber, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                   "\r\n" +
                                                                                   _CmpAttribute.ResourceText("IDS_ERR_NOINT"));
                this.txtBoxColumnAddFrameNumber.Select();
                this.txtBoxColumnAddFrameNumber.SelectAll();

                BtnEnabledChange(false);
            }
            else {
                this.errorProviderInvalid.SetError(this.txtBoxColumnAddFrameNumber, "");
            }
        }

        private void txtBoxKaburi_Kaku_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleStr(this.txtBoxKaburi_Kaku.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi_Kaku, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                   "\r\n" +
                                                                                   _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTGREATERZERO"));
                this.txtBoxKaburi_Kaku.Select();
                this.txtBoxKaburi_Kaku.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi_Kaku, "");
            }
        }

        private void txtBoxKaburi_En_Leave(object sender, EventArgs e)
        {
            if (!IsDoubleStr(this.txtBoxKaburi_En.Text))
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi_En, _CmpAttribute.ResourceText("IDS_ERR_INVALIDVALUE") +
                                                                                   "\r\n" +
                                                                                   _CmpAttribute.ResourceText("IDS_TXT_PLEASEINPUTGREATERZERO"));
                this.txtBoxKaburi_En.Select();
                this.txtBoxKaburi_En.SelectAll();

                BtnEnabledChange(false);
            }
            else
            {
                this.errorProviderInvalid.SetError(this.txtBoxKaburi_En, "");
            }
        }

        #endregion コントロールが離れたとき

        #endregion Events
    }
}